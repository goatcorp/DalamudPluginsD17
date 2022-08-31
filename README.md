# DalamudPluginsD17

Hi! This is the plugin repository for the [Dalamud plugin framework for Final Fantasy XIV](https://github.com/goatcorp/Dalamud). This repository is a successor to [DalamudPlugins](https://github.com/goatcorp/DalamudPlugins) and implements [DIP17](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md) to make the submission process easier and faster.

## Publishing your plugin

### Preparing your repository
- Ensure your plugin is on a publically accessible Git repo (GitHub, GitLab, any self-hosted Git instance that allows HTTP clones without authentication)
- Update your `.csproj`
  - Set `<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>` in a `PropertyGroup`
  - Use `$(DalamudLibPath)` if you aren't already, see <https://github.com/goatcorp/SamplePlugin/blob/master/SamplePlugin/SamplePlugin.csproj#L29-L63>
- Build your plugin in Release, commit your `.csproj` + the newly generated lock file

### Submitting
- Fork this repository, or use the GitHub web editor (press `.` in the repo, or press the ✏ icon on an existing manifest)
- In your fork, make `stable/(plugin name)/manifest.toml` (or `testing/live/(plugin name)/manifest.toml`). For more information, [see here](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md#guide-level-explanation).
  ```toml
  [plugin]
  repository = "https://github.com/goatcorp/SamplePlugin.git"
  commit = "765d9bb434ac99a27e9a3f2ba0a555b55fe6269d"
  owners = ["goaaats"]
  project_path = "SamplePlugin"
  changelog = "Added Herobrine"
  ```
- Place the images for your plugin in an `images` subfolder: `stable/(plugin name)/images`.
  - Please note this will be [streamlined at some point in the future](https://github.com/goatcorp/DIPs/pull/45). 
- Make the PR. If you're using the GitHub web editor, this will be automatic.

You'll also need to be using DalamudPackager; please check the SamplePlugin for an example. If you need help, please reach out.

## Updating your plugin

Just edit the commit hash in your manifest. Please always make your updates from a new branch, to make it cleaner for us to review.

## Rebuilding in a PR

If you want to trigger a re-build of your PR, just post a comment with the content "bleatbot, rebuild".


