#!/usr/bin/env bash
set -euo pipefail
export PATH="${BASH%/*}:$PATH"

script_dir="$(dirname -- "${BASH_SOURCE[0]}")"
repo_root="$(cd "$script_dir/.." && pwd)"
bucket="outlibs"
endpoint="https://storage.yandexcloud.net"
tmp_file=""
aws_cli="$(command -v aws || true)"

if [[ -z "$aws_cli" && -n "${LOCALAPPDATA:-}" ]] && command -v cygpath >/dev/null 2>&1; then
    local_app_data="$(cygpath -u "$LOCALAPPDATA")"
    candidate="$local_app_data/Programs/Amazon/AWSCLIV2/aws.exe"
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
    object_uri="s3://$bucket/GmshApi/$filename/$hash"
    "$aws_cli" --endpoint-url "$endpoint" s3 cp "$source_file" "$object_uri" --only-show-errors

    uploaded_hash="$(sha256sum -- "$source_file" | cut -c1-64)"
    if [[ "$uploaded_hash" != "$hash" ]]; then
        echo "File changed during upload: $source_file" >&2
        exit 1
    fi

    tmp_file="$(mktemp "${manifest}.tmp.XXXXXX")"
    printf 'sha256=%s\n' "$hash" > "$tmp_file"
    mv -f -- "$tmp_file" "$manifest"
    tmp_file=""
    echo "Uploaded $filename and updated its manifest"
done
