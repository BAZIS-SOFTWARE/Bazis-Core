#!/usr/bin/env bash
set -euo pipefail
export PATH="${BASH%/*}:$PATH"

script_dir="$(dirname -- "${BASH_SOURCE[0]}")"
repo_root="$(cd "$script_dir/.." && pwd)"
bucket="outlibs"
endpoint="https://storage.yandexcloud.net"
aws_profile="${AWS_PROFILE:-yandex}"
tmp_file=""
aws_cli="${AWS_CLI:-}"

if [[ -z "$aws_cli" ]]; then
    aws_cli="$(command -v aws || true)"
fi
if [[ -z "$aws_cli" && -n "${LOCALAPPDATA:-}" ]]; then
    candidate="$LOCALAPPDATA/Programs/Amazon/AWSCLIV2/aws.exe"
    if [[ -x "$candidate" ]]; then
        aws_cli="$candidate"
    fi
fi

trap 'if [[ -n "$tmp_file" ]]; then rm -f -- "$tmp_file"; fi' EXIT

if ! command -v sha256sum >/dev/null 2>&1; then
    echo "sha256sum is required." >&2
    exit 1
fi

if [[ -z "$aws_cli" ]]; then
    echo "AWS CLI is required to upload large files." >&2
    exit 1
fi

for filename in gmsh.dll libgmsh.so; do
    source_file="$repo_root/GmshApi/$filename"
    manifest="$source_file.manifest"

    if [[ ! -f "$source_file" ]]; then
        echo "Missing file: $source_file" >&2
        exit 1
    fi

    hash="$(sha256sum -- "$source_file" | cut -c1-64)"
    object_key="GmshApi/$filename/$hash"
    object_uri="s3://$bucket/$object_key"

    if "$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3api head-object --bucket "$bucket" --key "$object_key" >/dev/null 2>&1; then
        echo "Already uploaded $filename"
    else
        "$aws_cli" --profile "$aws_profile" --endpoint-url "$endpoint" s3 cp "$source_file" "$object_uri" --only-show-errors
        echo "Uploaded $filename"
    fi

    uploaded_hash="$(sha256sum -- "$source_file" | cut -c1-64)"
    if [[ "$uploaded_hash" != "$hash" ]]; then
        echo "File changed during upload: $source_file" >&2
        exit 1
    fi

    tmp_file="$(mktemp "${manifest}.tmp.XXXXXX")"
    printf 'sha256=%s\n' "$hash" > "$tmp_file"
    mv -f -- "$tmp_file" "$manifest"
    tmp_file=""
    echo "Updated manifest for $filename"
done
