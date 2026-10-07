# Native Gmsh libraries

`GmshApi/gmsh.dll` and `GmshApi/libgmsh.so` are stored in the Yandex Object Storage bucket `outlibs`. Git contains their SHA-256 manifests, not the binaries. Each object has an immutable, content-addressed key:

```text
s3://outlibs/GmshApi/<filename>/<sha256>
```

Install and configure the AWS CLI for Yandex Object Storage, then run from any directory:

```bash
bash /path/to/BazisCore/.githooks/pull_large_files.sh
```

The download script checks the local files first. It downloads only missing or mismatched files and verifies SHA-256 before replacing them.

To download automatically after `git pull` and branch switches, enable the versioned Git hooks once per clone:

```bash
git config --local core.hooksPath .githooks
```

The `post-merge`, `post-rewrite`, and `post-checkout` hooks cover merge, rebase, and branch checkout. Git does not enable versioned hooks automatically in a new clone, so run `bash .githooks/pull_large_files.sh` once after enabling them. If a download fails, run it again after fixing AWS CLI access.

After changing either binary, upload both local binaries and update the manifests:

```bash
bash /path/to/BazisCore/.githooks/push_large_files.sh
```

Commit the changed `.manifest` files after a successful upload. The AWS CLI uses your configured credentials or `AWS_PROFILE`; do not put access keys in this repository. Objects referenced by older manifests must remain in the bucket if older commits need to be reproducible.
