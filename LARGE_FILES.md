# Large files in Yandex Object Storage

The repository reads large files from Yandex Object Storage. The upload process runs outside this repository. Configure AWS CLI access to the `outlibs` and `outcommon` buckets before downloading.

The following files use stable object keys and keep their manifests beside the local files. Each manifest has exactly two lines: `sha256=<64 lowercase hex digits>` and `etag=<cloud ETag>`.

| Local files | Bucket | Object keys |
| --- | --- | --- |
| `GmshApi/gmsh.dll`, `GmshApi/libgmsh.so` | `outlibs` | `gmsh.dll`, `libgmsh.so` |
| `TestModelCore/Examples/STL/*.STL`, `*.stl.bin.stl` with manifests | `outcommon` | `TestModelCore/Examples/STL/<filename>` |
| `TestProject/MaterialDB/Data/functions.jsf` | `outcommon` | `TestProject/FunctionDB/functions.jsf` |
| `TestProject/MaterialDB/Data/Materials_v7.jsf` | `outcommon` | `TestProject/MaterialDB/Materials_v7.jsf` |

For these files, the script reads the cloud ETag with `head-object` on every run. If the ETag matches the manifest and the local SHA-256 matches its `sha256=` entry, no download is needed. Otherwise, the script downloads to a temporary file, calculates its SHA-256, checks the cloud ETag again, and replaces the local file only when its content differs. It then updates the adjacent manifest with `sha256=` and `etag=`. A changed cloud object therefore leaves a modified tracked manifest to review and commit. The ETag identifies an object version for this check; it is not treated as a SHA-256 checksum.

The script rejects manifests without both fields and paths without an explicit bucket and object-key mapping. It ignores manifests in the former local `outcommon/` directory when they are still present in an older branch; content-addressed cloud keys are no longer supported.

Run the downloader manually from any directory:

```bash
bash /path/to/BazisCore/.githooks/pull_large_files.sh
```

To run it after a merge, rebase, or branch checkout, enable the versioned Git hooks once per clone:

```bash
git config --local core.hooksPath .githooks
```

The existing `post-merge`, `post-rewrite`, and `post-checkout` hooks all call the same downloader. Git does not enable versioned hooks automatically in a new clone. A `git pull` that makes no merge or rebase change does not necessarily invoke a hook; run the script manually when an immediate cloud check is needed. The script uses AWS CLI profile `yandex` by default; set `AWS_PROFILE` to select another profile. Git Bash is required on Windows.
