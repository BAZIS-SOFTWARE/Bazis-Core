#!/usr/bin/env bash
# Скачивает ресурсы по манифестам и проверяет их SHA-256.
set -euo pipefail
export PATH="${BASH%/*}:$PATH"

# Определяем корень репозитория независимо от текущего каталога запуска.
script_dir="$(dirname -- "${BASH_SOURCE[0]}")"
repo_root="$(cd "$script_dir/.." && pwd)"
endpoint="https://storage.yandexcloud.net"
aws_profile="${AWS_PROFILE:-yandex}"
tmp_file=""
tmp_manifest=""
aws_cli="${AWS_CLI:-}"

# Ищем AWS CLI в PATH или в стандартном каталоге установки для Windows.
if [[ -z "$aws_cli" ]]; then
    aws_cli="$(command -v aws || true)"
fi
if [[ -z "$aws_cli" && -n "${LOCALAPPDATA:-}" ]]; then
    candidate="$LOCALAPPDATA/Programs/Amazon/AWSCLIV2/aws.exe"
    if [[ -x "$candidate" ]]; then
        aws_cli="$candidate"
    fi
fi

# При ошибке удаляем только незавершённые временные файлы.
trap 'if [[ -n "$tmp_file" ]]; then rm -f -- "$tmp_file"; fi; if [[ -n "$tmp_manifest" ]]; then rm -f -- "$tmp_manifest"; fi' EXIT

if ! command -v sha256sum >/dev/null 2>&1; then
    echo "sha256sum is required." >&2
    exit 1
fi

manifest_count=0
while IFS= read -r -d '' relative_manifest; do
    manifest="$repo_root/$relative_manifest"
    if [[ ! -f "$manifest" ]]; then
        continue
    fi
    manifest_count=$((manifest_count + 1))
    # Облачные файлы GmshApi — источник текущей версии.
    if [[ "$relative_manifest" == GmshApi/gmsh.dll.manifest || "$relative_manifest" == GmshApi/libgmsh.so.manifest ]]; then
        remote_authoritative=1
        bucket="outlibs"
        relative_file="${relative_manifest%.manifest}"
        object_key="${relative_file##*/}"
    else
        # Для остальных ресурсов сохраняем прежнюю схему <корзина>/<путь>.manifest.
        remote_authoritative=0
        bucket="${relative_manifest%%/*}"
        relative_file="${relative_manifest#*/}"
        relative_file="${relative_file%.manifest}"
        manifest_value="$(<"$manifest")"
        if [[ ! "$manifest_value" =~ ^sha256=([0-9a-f]{64})$ ]]; then
            echo "Invalid SHA-256 manifest: $manifest" >&2
            exit 1
        fi
        expected_hash="${BASH_REMATCH[1]}"
        object_key="$relative_file/$expected_hash"
    fi
    target="$repo_root/$relative_file"

    if [[ "$remote_authoritative" -eq 1 ]]; then
        if [[ -z "$aws_cli" ]]; then
            echo "AWS CLI is required to check $relative_file." >&2
            exit 1
        fi

        remote_etag="$("$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3api head-object --bucket "$bucket" --key "$object_key" --query ETag --output text)"
        remote_etag="${remote_etag#\"}"
        remote_etag="${remote_etag%\"}"
        if [[ -z "$remote_etag" || "$remote_etag" == "None" ]]; then
            echo "Missing ETag for $relative_file." >&2
            exit 1
        fi

        manifest_hash="$(sed -n '1s/^sha256=//p' "$manifest")"
        manifest_etag="$(sed -n '2s/^etag=//p' "$manifest")"
        manifest_hash="${manifest_hash%$'\r'}"
        manifest_etag="${manifest_etag%$'\r'}"
        if [[ "$manifest_hash" =~ ^[0-9a-f]{64}$ && "$manifest_etag" == "$remote_etag" && -f "$target" ]]; then
            local_hash="$(sha256sum -- "$target" | cut -c1-64)"
            if [[ "$local_hash" == "$manifest_hash" ]]; then
                echo "Verified $relative_file against cloud ETag"
                continue
            fi
        fi
    fi

    # Для остальных ресурсов скачивание нужно только при несовпадении с манифестом.
    if [[ "$remote_authoritative" -eq 0 && -f "$target" ]]; then
        actual_hash="$(sha256sum -- "$target" | cut -c1-64)"
        if [[ "$actual_hash" == "$expected_hash" ]]; then
            echo "Verified $relative_file"
            continue
        fi
    fi

    if [[ -z "$aws_cli" ]]; then
        echo "AWS CLI is required to download $relative_file." >&2
        exit 1
    fi

    object_uri="s3://$bucket/$object_key"
    # Сначала скачиваем рядом с целевым файлом, затем сверяем хеш и заменяем файл.
    mkdir -p -- "$(dirname -- "$target")"
    tmp_file="$(mktemp "${target}.tmp.XXXXXX")"
    "$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3 cp "$object_uri" "$tmp_file" --only-show-errors

    downloaded_hash="$(sha256sum -- "$tmp_file" | cut -c1-64)"
    if [[ "$remote_authoritative" -eq 1 ]]; then
        latest_etag="$("$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3api head-object --bucket "$bucket" --key "$object_key" --query ETag --output text)"
        latest_etag="${latest_etag#\"}"
        latest_etag="${latest_etag%\"}"
        if [[ "$latest_etag" != "$remote_etag" ]]; then
            echo "Cloud object changed during download: $relative_file" >&2
            exit 1
        fi

        local_hash=""
        if [[ -f "$target" ]]; then
            local_hash="$(sha256sum -- "$target" | cut -c1-64)"
        fi
        if [[ "$local_hash" == "$downloaded_hash" ]]; then
            rm -f -- "$tmp_file"
            tmp_file=""
            echo "Verified $relative_file against cloud version"
        else
            mv -f -- "$tmp_file" "$target"
            tmp_file=""
            echo "Updated $relative_file from cloud"
        fi

        manifest_value="$(<"$manifest")"
        expected_manifest="$(printf 'sha256=%s\netag=%s' "$downloaded_hash" "$remote_etag")"
        if [[ "$manifest_value" != "$expected_manifest" ]]; then
            tmp_manifest="$(mktemp "${manifest}.tmp.XXXXXX")"
            printf 'sha256=%s\netag=%s\n' "$downloaded_hash" "$remote_etag" > "$tmp_manifest"
            mv -f -- "$tmp_manifest" "$manifest"
            tmp_manifest=""
            echo "Updated manifest for $relative_file"
        fi
        continue
    fi

    if [[ "$downloaded_hash" != "$expected_hash" ]]; then
        echo "SHA-256 mismatch for $relative_file: expected $expected_hash, got $downloaded_hash" >&2
        exit 1
    fi

    mv -f -- "$tmp_file" "$target"
    tmp_file=""
    echo "Downloaded and verified $relative_file"
done < <(git -C "$repo_root" ls-files --cached --others --exclude-standard -z -- ':(glob)**/*.manifest')

if [[ "$manifest_count" -eq 0 ]]; then
    echo "No large-file manifests found in $repo_root." >&2
    exit 1
fi
