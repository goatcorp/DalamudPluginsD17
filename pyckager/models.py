from re import compile
from typing import Optional

from pydantic import BaseModel, validator


NUMBERS_ONLY = compile(r'^\d+$')
VERSION = compile(r'^(\d+\.)+\d+$')

class Plugin(BaseModel):
    repository: str
    commit: str
    owners: list[str]
    project_path: Optional[str]
    changelog: Optional[str]
    version: Optional[str]

    @validator('owners')
    def validate_owners(cls, owners):
        for owner in owners:
            if NUMBERS_ONLY.match(owner):
                raise ValueError(f'Value "{owner}" must be a github username; ids are not allowed')
        return owners

    @validator('version')
    def validate_version(cls, version):
        version_match = VERSION.match(version)
        if version_match:
            segments = tuple(int(x) for x in version_match[0].split('.'))
            if 1 < len(segments) < 5:
                return segments
        raise ValueError(f'Value "{version}" is not a valid version')


class Manifest(BaseModel):
    plugin: Plugin
