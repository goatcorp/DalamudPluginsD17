"""CLI tooling for working with package manifest files"""
from sys import exit, stderr
from argparse import ArgumentParser, ArgumentTypeError
from pathlib import Path

import toml

from pyckager.models import Manifest
from pydantic import ValidationError


def to_manifest_path(str_path: str) -> Path | None:
    """Helper for converting str to path in argparse arguments"""
    try:
        path = Path(str_path)
    except Exception:
        raise ArgumentTypeError(f"Path '{str_path}' has issues")

    match path.suffix:
        case '.toml':
            return path
        case _:
            raise ArgumentTypeError(f'Extension {path.suffix} not supported for {path}')


def create_parser() -> ArgumentParser:
    """Creates the ArgumentParser"""
    parser = ArgumentParser()
    subparsers = parser.add_subparsers(help='action to perform', dest='action')

    # validating manifests
    validate_parser = subparsers.add_parser('validate')
    validate_parser.add_argument('manifests', type=to_manifest_path, nargs='+', help='Path to the manifest to validate')

    # print out json schema; don't need to add options for it at this time
    # may consider adding an indent for niceness though
    subparsers.add_parser('schema')

    return parser


def validate_manifests(**kwargs) -> bool:
    """Takes a manifest and validates it
    
    :param Path manifest: 

    """
    manifests: list[Path] = kwargs.get('manifests')

    for manifest in manifests:
        with open(manifest, 'r', encoding='utf-8') as f:
            manifest_dict = toml.loads(f.read())
        try:
            manifest = Manifest(**manifest_dict)
        except ValidationError as e:
            print(e, file=stderr)
            return False

    return True


def main() -> int:
    parser = create_parser()
    args = parser.parse_args()

    match args.action:
        case 'validate':
            if validate_manifests(**vars(args)) is False:
                return 1
        case 'schema':
            print(Manifest.schema_json(indent=2))
            return 0
        case _:
            parser.print_help()
    return 0


if __name__ == '__main__':
    exit(main())
