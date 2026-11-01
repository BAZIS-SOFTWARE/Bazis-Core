"""Проверяет PublishVersion и выбирает новые версии пакетов для публикации."""

import argparse
import json
import re
import subprocess
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
VERSION_PATTERN = re.compile(r"\d+\.\d+\.\d+(?:\.\d+)?\Z")


def read_versions(content):
    versions = json.loads(content)
    if not isinstance(versions, dict) or not versions:
        raise ValueError("PublishVersion must contain a nonempty JSON object.")
    for project, version in versions.items():
        if not isinstance(project, str) or not project.endswith(".csproj"):
            raise ValueError("PublishVersion keys must be project paths.")
        if not isinstance(version, str) or not VERSION_PATTERN.fullmatch(version):
            raise ValueError(f"Invalid version for {project}: {version}.")
    return versions


def version_parts(version):
    return tuple(int(part) for part in version.split("."))


def compare_versions(current, previous):
    left = version_parts(current)
    right = version_parts(previous)
    width = max(len(left), len(right))
    left += (0,) * (width - len(left))
    right += (0,) * (width - len(right))
    return (left > right) - (left < right)


def previous_versions(before):
    if not before or set(before) == {"0"}:
        return None
    commit = subprocess.run(["git", "cat-file", "-e", f"{before}^{{commit}}"], cwd=ROOT, capture_output=True)
    if commit.returncode != 0:
        raise ValueError(f"Previous commit is not available: {before}.")
    result = subprocess.run(["git", "cat-file", "-e", f"{before}:PublishVersion"], cwd=ROOT, capture_output=True)
    if result.returncode != 0:
        return None
    result = subprocess.run(["git", "show", f"{before}:PublishVersion"], cwd=ROOT, capture_output=True, text=True, check=True)
    return read_versions(result.stdout)


def project_metadata(project):
    project_path = (ROOT / project).resolve()
    if not project_path.is_relative_to(ROOT) or not project_path.is_file():
        raise ValueError(f"Project does not exist: {project}.")
    root = ET.parse(project_path).getroot()
    properties = root.find("PropertyGroup")
    if properties is None:
        raise ValueError(f"Project has no properties: {project}.")
    version = properties.findtext("Version")
    if properties.findtext("GeneratePackageOnBuild", "").lower() != "true":
        raise ValueError(f"Project is not configured to create a package: {project}.")
    package_id = properties.findtext("PackageId") or properties.findtext("AssemblyName") or project_path.stem
    references = []
    for reference in root.iter("ProjectReference"):
        target = (project_path.parent / reference.attrib["Include"].replace("\\", "/")).resolve()
        references.append(target.relative_to(ROOT).as_posix())
    return version, package_id, references


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--before", default="")
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()

    current = read_versions((ROOT / "PublishVersion").read_text(encoding="utf-8"))
    previous = previous_versions(args.before)
    if previous is not None and previous.keys() - current.keys():
        raise ValueError("Projects cannot be removed from PublishVersion.")

    projects = {}
    for project, version in current.items():
        actual_version, package_id, references = project_metadata(project)
        if actual_version != version:
            raise ValueError(f"PublishVersion for {project} must equal project Version {actual_version}.")
        if previous is not None and project in previous and compare_versions(version, previous[project]) < 0:
            raise ValueError(f"Version decreased for {project}.")
        projects[project] = {"project": project, "id": package_id, "version": version, "references": references}

    changed = {project for project in current if previous is None or project not in previous or compare_versions(current[project], previous[project]) > 0}
    ordered = []
    while changed:
        ready = sorted(project for project in changed if not any(reference in changed for reference in projects[project]["references"]))
        if not ready:
            raise ValueError("Project references contain a cycle.")
        for project in ready:
            ordered.append({key: projects[project][key] for key in ("project", "id", "version")})
            changed.remove(project)

    print(f"Packages to publish: {len(ordered)}")
    for package in ordered:
        print(f"  {package['id']} {package['version']}")
    if args.output:
        with args.output.open("a", encoding="utf-8") as output:
            output.write(f"count={len(ordered)}\n")
            output.write(f"packages={json.dumps(ordered, separators=(',', ':'))}\n")


if __name__ == "__main__":
    main()
