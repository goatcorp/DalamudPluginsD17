from re import compile
from typing import Optional, Literal

from pydantic import BaseModel, Extra, validator, Field


NUMBERS_ONLY = compile(r'^\d+$')
VERSION = compile(r'^(\d+\.)+\d+$')

Type = Literal['file']
Image = Literal['base', 'extended']


class Needs(BaseModel):
    # Using a field alias here so we don't collide with the builtin `type()`
    type_: Type = Field(alias='type')
    # TODO: these are required if type=file, so further validation is needed
    url: Optional[str]
    dest: Optional[str]
    sha512: Optional[str] # TODO: validate

    class Config:
        extra = Extra.forbid


class Build(BaseModel):
    image: Image = 'base'
    needs: list[Needs]

    class Config:
        extra = Extra.forbid


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

    class Config:
        extra = Extra.forbid


class Manifest(BaseModel):
    build: Optional[Build]
    plugin: Plugin

    class Config:
        extra = Extra.forbid