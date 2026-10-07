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

for filename in gmsh.dll libgmsh.so; do
    target="$repo_root/GmshApi/$filename"
    manifest="$target.manifest"

    if [[ ! -f "$manifest" ]]; then
        echo "Missing manifest: $manifest" >&2
        exit 1
    fi

    manifest_value="$(<"$manifest")"
    if [[ ! "$manifest_value" =~ ^sha256=([0-9a-f]{64})$ ]]; then
        echo "Invalid SHA-256 manifest: $manifest" >&2
        exit 1
    fi
    expected_hash="${BASH_REMATCH[1]}"

    if [[ -f "$target" ]]; then
        actual_hash="$(sha256sum -- "$target" | cut -c1-64)"
        if [[ "$actual_hash" == "$expected_hash" ]]; then
            echo "Verified $filename"
            continue
        fi
    fi

    if [[ -z "$aws_cli" ]]; then
        echo "AWS CLI is required to download $filename." >&2
        exit 1
    fi

    object_uri="s3://$bucket/GmshApi/$filename/$expected_hash"
    tmp_file="$(mktemp "${target}.tmp.XXXXXX")"
    "$aws_cli" --endpoint-url "$endpoint" s3 cp "$object_uri" "$tmp_file" --only-show-errors

    actual_hash="$(sha256sum -- "$tmp_file" | cut -c1-64)"
    if [[ "$actual_hash" != "$expected_hash" ]]; then
        echo "SHA-256 mismatch for $filename: expected $expected_hash, got $actual_hash" >&2
        exit 1
    fi

    mv -f -- "$tmp_file" "$target"
    tmp_file=""
    echo "Downloaded and verified $filename"
done
