# DalamudPluginsD17

Hi! This is the DalamudPlugins playground for [DIP17](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md).

## Publishing your plugin

- Ensure your plugin is on a publically accessible Git repo (GitHub, GitLab, any self-hosted Git instance that allows HTTP clones without authentication)
- Update your `.csproj`
  - Set `<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>` in a `PropertyGroup`
  - Use `$(DalamudLibPath)` if you aren't already, see <https://github.com/goatcorp/SamplePlugin/blob/master/SamplePlugin/SamplePlugin.csproj#L29-L63>
- Build your plugin in Release, commit your `.csproj` + the newly generated lock file
- Fork this reposistory
- In your fork, make `stable/(plugin name)/manifest.toml` (or `testing/api6/(plugin name)/manifest.toml`)
  - See [here](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md#guide-level-explanation) for details
  - Example provided below!
- Shove your images into that newly created folder
- Make the PR and pray our GHA doesn't fall apart

```toml
[plugin]
repository = "https://github.com/goatcorp/SamplePlugin.git"
commit = "765d9bb434ac99a27e9a3f2ba0a555b55fe6269d"
owners = [
    "goaaats",
]
project_path = "SamplePlugin"
changelog = "Added Herobrine"
```

You'll also need to be using DalamudPackager, please check the SamplePlugin for an example.
