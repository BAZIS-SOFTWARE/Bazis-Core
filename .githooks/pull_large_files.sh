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
    # Манифест лежит рядом с целевым файлом. Путь в корзине задаём явно:
    # облачный объект по постоянному ключу определяет актуальную версию.
    relative_file="${relative_manifest%.manifest}"
    if [[ "$relative_manifest" == GmshApi/gmsh.dll.manifest || "$relative_manifest" == GmshApi/libgmsh.so.manifest ]]; then
        remote_authoritative=1
        bucket="outlibs"
        relative_file="${relative_manifest%.manifest}"
        object_key="${relative_file##*/}"
    elif [[ "$relative_manifest" == TestModelCore/Examples/STL/*.manifest ]]; then
        # STL в outcommon сохраняют тот же путь, что и файлы в репозитории.
        bucket="outcommon"
        object_key="$relative_file"
    elif [[ "$relative_manifest" == TestProject/MaterialDB/Data/functions.jsf.manifest ]]; then
        # Функции тестов лежат локально рядом с материалами, а в облаке — в FunctionDB.
        bucket="outcommon"
        object_key="TestProject/FunctionDB/functions.jsf"
    elif [[ "$relative_manifest" == TestProject/MaterialDB/Data/Materials_v7.jsf.manifest ]]; then
        # Для материалов облачный ключ не содержит локальный каталог Data.
        bucket="outcommon"
        object_key="TestProject/MaterialDB/Materials_v7.jsf"
    else
        # Неизвестный манифест не должен превращаться в ключ старого формата.
        echo "Unsupported manifest path: $relative_manifest" >&2
        exit 1
    fi
        expected_hash="${BASH_REMATCH[1]}"
        object_key="$relative_file/$expected_hash"
    fi
    target="$repo_root/$relative_file"

    # Поддерживаем только двухстрочный манифест с SHA-256 и ETag.
    # Удаляем CR, чтобы одинаково читать LF и CRLF в Git Bash на Windows.
    mapfile -t manifest_lines < "$manifest"
    manifest_hash="${manifest_lines[0]-}"
    manifest_etag="${manifest_lines[1]-}"
    manifest_hash="${manifest_hash%$'\r'}"
    manifest_etag="${manifest_etag%$'\r'}"
    if [[ "${#manifest_lines[@]}" -ne 2 || ! "$manifest_hash" =~ ^sha256=[0-9a-f]{64}$ || ! "$manifest_etag" =~ ^etag=[^[:space:]]+$ ]]; then
        echo "Invalid SHA-256/ETag manifest: $manifest" >&2
        exit 1
    fi
    manifest_hash="${manifest_hash#sha256=}"
    manifest_etag="${manifest_etag#etag=}"

    if [[ -z "$aws_cli" ]]; then
        echo "AWS CLI is required to check $relative_file." >&2
        exit 1
    fi

    # HEAD не скачивает файл: ETag позволяет обнаружить замену объекта.
    remote_etag="$("$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3api head-object --bucket "$bucket" --key "$object_key" --query ETag --output text)"
    remote_etag="${remote_etag#\"}"
    remote_etag="${remote_etag%\"}"
    if [[ -z "$remote_etag" || "$remote_etag" == "None" ]]; then
        echo "Missing ETag for $relative_file." >&2
        exit 1
    fi

    # Совпадение ETag без проверки локального SHA-256 недостаточно:
    # файл мог быть изменён или повреждён уже после предыдущей загрузки.
    if [[ "$manifest_etag" == "$remote_etag" && -f "$target" ]]; then
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
    # Скачиваем во временный файл рядом с целевым: ошибка передачи не должна
    # оставлять частично записанный ресурс на месте рабочего файла.
    mkdir -p -- "$(dirname -- "$target")"
    tmp_file="$(mktemp "${target}.tmp.XXXXXX")"
    "$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3 cp "$object_uri" "$tmp_file" --only-show-errors

    downloaded_hash="$(sha256sum -- "$tmp_file" | cut -c1-64)"
    # Повторный HEAD защищает от смены облачной версии во время скачивания.
    latest_etag="$("$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3api head-object --bucket "$bucket" --key "$object_key" --query ETag --output text)"
    latest_etag="${latest_etag#\"}"
    latest_etag="${latest_etag%\"}"
    if [[ "$latest_etag" != "$remote_etag" ]]; then
        echo "Cloud object changed during download: $relative_file" >&2
        exit 1
    fi
    # При прежнем ETag известный SHA-256 обязан совпасть и у скачанной копии.
    # Иначе не заменяем рабочий файл и не переписываем проверенный манифест.
    if [[ "$remote_etag" == "$manifest_etag" && "$downloaded_hash" != "$manifest_hash" ]]; then
        echo "SHA-256 mismatch for unchanged cloud object: $relative_file" >&2
        exit 1
    fi

    local_hash=""
    if [[ -f "$target" ]]; then
        local_hash="$(sha256sum -- "$target" | cut -c1-64)"
    fi
    if [[ "$local_hash" == "$downloaded_hash" ]]; then
        # Объект мог получить новый ETag без изменения содержимого.
        rm -f -- "$tmp_file"
        tmp_file=""
        echo "Verified $relative_file against cloud version"
    else
        mv -f -- "$tmp_file" "$target"
        tmp_file=""
        echo "Updated $relative_file from cloud"
    fi

    # SHA-256 фиксирует содержимое проверенной копии, ETag — облачную версию.
    # При их изменении обновляем только соседний манифест.
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
