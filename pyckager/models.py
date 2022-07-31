from re import compile
from typing import Optional

from pydantic import BaseModel, validator


NUMBERS_ONLY = compile(r'^\d+$')

class Plugin(BaseModel):
    repository: str
    commit: str
    owners: list[str]
    project_path: Optional[str]
    changelog: Optional[str]

    @validator('owners')
    def validate_owners(cls, owners):
        for owner in owners:
            if NUMBERS_ONLY.match(owner):
                raise ValueError(f'Value "{owner}" must be a github username; ids are not allowed')
        return owners

class Manifest(BaseModel):
    plugin: Plugin
