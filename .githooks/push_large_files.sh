#!/usr/bin/env bash
# Загружает файлы всех проектов по ключам с SHA-256 и обновляет их манифесты.
set -euo pipefail
export PATH="${BASH%/*}:$PATH"

# Определяем корень репозитория независимо от текущего каталога запуска.
script_dir="$(dirname -- "${BASH_SOURCE[0]}")"
repo_root="$(cd "$script_dir/.." && pwd)"
manifest_root="$repo_root/LargeFiles"
bucket="outlibs"
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

# При ошибке удаляем только незавершённый временный манифест.
trap 'if [[ -n "$tmp_file" ]]; then rm -f -- "$tmp_file"; fi' EXIT

if ! command -v sha256sum >/dev/null 2>&1; then
    echo "sha256sum is required." >&2
    exit 1
fi

if [[ -z "$aws_cli" ]]; then
    echo "AWS CLI is required to upload large files." >&2
    exit 1
fi

if [[ ! -d "$manifest_root" ]]; then
    echo "Missing manifest directory: $manifest_root" >&2
    exit 1
fi

manifest_count=0
while IFS= read -r -d '' manifest; do
    manifest_count=$((manifest_count + 1))
    # Путь манифеста определяет путь исходного файла и ключ объекта в бакете.
    relative_manifest="${manifest#"$manifest_root"/}"
    relative_file="${relative_manifest%.manifest}"
    source_file="$repo_root/$relative_file"

    if [[ ! -f "$source_file" ]]; then
        echo "Missing file: $source_file" >&2
        exit 1
    fi

    hash="$(sha256sum -- "$source_file" | cut -c1-64)"
    object_key="$relative_file/$hash"
    object_uri="s3://$bucket/$object_key"

    # Не загружаем повторно объект с тем же содержимым.
    if "$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3api head-object --bucket "$bucket" --key "$object_key" >/dev/null 2>&1; then
        echo "Already uploaded $relative_file"
    else
        "$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3 cp "$source_file" "$object_uri" --only-show-errors
        echo "Uploaded $relative_file"
    fi

    # Изменившийся во время передачи файл не должен попасть в манифест.
    uploaded_hash="$(sha256sum -- "$source_file" | cut -c1-64)"
    if [[ "$uploaded_hash" != "$hash" ]]; then
        echo "File changed during upload: $source_file" >&2
        exit 1
    fi

    # Обновляем манифест только после успешной загрузки, через временный файл.
    tmp_file="$(mktemp "${manifest}.tmp.XXXXXX")"
    printf 'sha256=%s\n' "$hash" > "$tmp_file"
    mv -f -- "$tmp_file" "$manifest"
    tmp_file=""
    echo "Updated manifest for $relative_file"
done < <(find "$manifest_root" -type f -name '*.manifest' -print0)

if [[ "$manifest_count" -eq 0 ]]; then
    echo "No manifests found in $manifest_root." >&2
    exit 1
fi
