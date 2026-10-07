# Large files in Yandex Object Storage

Large binaries are stored in the Yandex Object Storage bucket `outlibs`. Git contains their SHA-256 manifests under `LargeFiles/<project>/`, not the binaries. For example, `LargeFiles/GmshApi/gmsh.dll.manifest` corresponds to `GmshApi/gmsh.dll`. Each object has an immutable, content-addressed key:

```text
s3://outlibs/<project>/<filename>/<sha256>
```

Install and configure the AWS CLI for Yandex Object Storage, then run from any directory:

```bash
bash /path/to/BazisCore/.githooks/pull_large_files.sh
```

The scripts find all `*.manifest` files under `LargeFiles/`, including nested project paths. The manifest path without the `LargeFiles/` prefix and `.manifest` suffix is the local file path; its SHA-256 is the final object-key component. The download script checks local files first. It downloads only missing or mismatched files and verifies SHA-256 before replacing them.

To download automatically after `git pull` and branch switches, enable the versioned Git hooks once per clone:

```bash
git config --local core.hooksPath .githooks
```

The `post-merge`, `post-rewrite`, and `post-checkout` hooks cover merge, rebase, and branch checkout. Git does not enable versioned hooks automatically in a new clone, so run `bash .githooks/pull_large_files.sh` once after enabling them. If a download fails, run it again after fixing AWS CLI access.

The `pre-push` hook uploads missing objects before a GitHub push. It stops the GitHub push if an upload changes a manifest: commit the updated manifest, then push again. The scripts use the AWS CLI profile `yandex` by default; set `AWS_PROFILE` to select another profile. Visual Studio's bundled Git has `sh` but no Bash, so the hooks use Git Bash installed at its standard Windows location when `bash` is not on `PATH`.

To register a new file, create an empty manifest at `LargeFiles/<project>/<filename>.manifest`, add the corresponding binary to `.gitignore`, and run the upload script. The script calculates the current SHA-256 and writes `sha256=<64 lowercase hex digits>` to the manifest after uploading. To upload changed files:

```bash
bash /path/to/BazisCore/.githooks/push_large_files.sh
```

Commit the changed `.manifest` files after a successful upload. The AWS CLI uses your configured credentials or `AWS_PROFILE`; do not put access keys in this repository. Objects referenced by older manifests must remain in the bucket if older commits need to be reproducible.
