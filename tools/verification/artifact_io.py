"""Atomic UTF-8 artifact writes shared by offline generators."""
import os
import tempfile
from pathlib import Path


def write_generated(path, content):
    path = Path(path)
    if path.exists() and path.read_text(encoding="utf-8") == content:
        return
    path.parent.mkdir(parents=True, exist_ok=True)
    temporary = None
    try:
        with tempfile.NamedTemporaryFile(mode="w", encoding="utf-8", newline="\n",
                                         dir=path.parent, delete=False) as stream:
            temporary = Path(stream.name)
            stream.write(content)
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, path)
    finally:
        if temporary is not None:
            temporary.unlink(missing_ok=True)


def new_report_directory(path, repository):
    """Reports go to a new external directory; existing evidence is never replaced."""
    path = Path(path).resolve()
    if path.is_relative_to(Path(repository).resolve()):
        raise ValueError("Report output must be outside the repository.")
    path.mkdir(parents=True, exist_ok=False)
    return path
