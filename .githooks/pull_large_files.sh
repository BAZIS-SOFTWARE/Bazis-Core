#!/usr/bin/env bash
# Скачивает файлы всех проектов по манифестам и проверяет их SHA-256.
set -euo pipefail
export PATH="${BASH%/*}:$PATH"

# Определяем корень репозитория независимо от текущего каталога запуска.
script_dir="$(dirname -- "${BASH_SOURCE[0]}")"
repo_root="$(cd "$script_dir/.." && pwd)"
endpoint="https://storage.yandexcloud.net"
aws_profile="${AWS_PROFILE:-yandex}"
tmp_file=""
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

# При ошибке удаляем только незавершённый временный файл.
trap 'if [[ -n "$tmp_file" ]]; then rm -f -- "$tmp_file"; fi' EXIT

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
    # <корзина>/<проект>/<файл>.manifest соответствует <проект>/<файл> в репозитории.
    bucket="${relative_manifest%%/*}"
    relative_file="${relative_manifest#*/}"
    relative_file="${relative_file%.manifest}"
    target="$repo_root/$relative_file"

    manifest_value="$(<"$manifest")"
    if [[ ! "$manifest_value" =~ ^sha256=([0-9a-f]{64})$ ]]; then
        echo "Invalid SHA-256 manifest: $manifest" >&2
        exit 1
    fi
    expected_hash="${BASH_REMATCH[1]}"

    # Совпадающий локальный файл не скачиваем повторно.
    if [[ -f "$target" ]]; then
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

    object_uri="s3://$bucket/$relative_file/$expected_hash"
    # Сначала скачиваем рядом с целевым файлом, затем сверяем хеш и заменяем файл.
    mkdir -p -- "$(dirname -- "$target")"
    tmp_file="$(mktemp "${target}.tmp.XXXXXX")"
    "$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3 cp "$object_uri" "$tmp_file" --only-show-errors

    actual_hash="$(sha256sum -- "$tmp_file" | cut -c1-64)"
    if [[ "$actual_hash" != "$expected_hash" ]]; then
        echo "SHA-256 mismatch for $relative_file: expected $expected_hash, got $actual_hash" >&2
        exit 1
    fi

    mv -f -- "$tmp_file" "$target"
    tmp_file=""
    echo "Downloaded and verified $relative_file"
done < <(git -C "$repo_root" ls-files --cached --others --exclude-standard -z -- ':(glob)**/*.manifest')

if [[ "$manifest_count" -eq 0 ]]; then
    echo "No manifests found under bucket directories in $repo_root." >&2
    exit 1
fi
