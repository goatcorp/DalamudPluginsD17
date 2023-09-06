# DalamudPluginsD17

Hi! This is the plugin repository for the [Dalamud plugin framework for Final Fantasy XIV](https://github.com/goatcorp/Dalamud). This repository is a successor to [DalamudPlugins](https://github.com/goatcorp/DalamudPlugins) and implements [DIP17](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md) to make the submission process easier and faster.

## Publishing your plugin

### Preparing your repository

- Ensure your plugin is on a publically accessible Git repo (GitHub, GitLab, any self-hosted Git instance that allows HTTP clones without authentication)
- Update your `.csproj`
  - Set `<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>` in a `PropertyGroup`
  - Use `$(DalamudLibPath)` if you aren't already, see <https://github.com/goatcorp/SamplePlugin/blob/master/SamplePlugin/SamplePlugin.csproj#L29-L63>
- Build your plugin in Release, commit your `.csproj` + the newly generated lock file

### Approval criteria

When the plugin approval group checks your plugin, they will check for the following:

- Does it meet [our guidelines](https://dalamud.dev/plugin-development/restrictions#what-am-i-allowed-to-do-in-my-plugin), as agreed upon by multiple members of the group?
- Does it feature any combat elements? If so, are they purely informational, and show only information the player would normally know?
- Does it pass an informal code review?
- Does it install cleanly?
- Does the configuration window (if present) behave correctly?
- Does the base functionality of the plugin work (if testable easily)?
- Does it have no obvious technical issues?
- Is its JSON correctly formatted? (We hope to [make this unnecessary in future](https://github.com/goatcorp/DalamudPackager/issues/8))
- If it's a new plugin, is it in the testing channel and not the stable channel? We want new plugins to be in testing to make it easier for the group to test, as well as reducing the impact of any unforeseen issues.
- Does it meet the [Technical criteria](#technical-criteria)?

These criteria are intended to prevent issues for users. We're happy to work with you to get you across the line; just reach out in the Discord.

### Technical criteria

There are a few technical things that you must do before submitting your plugin here. They will make your plugin nicer to use.
- Your plugin **must have** an `icon.png` that is no larger than 512x512 and no smaller than 64x64 in `images/`.
- For regular ImGui windows that don't do anything special, like settings and utility windows, you should use the [Dalamud Windowing API](https://dalamud.dev/api/Dalamud.Interface.Windowing/). It enhances windows with a few nice features, like integration into the native UI closing-order.
- Your plugin's version/assembly version **must not** be based on a timestamp or continually increasing build number. Every time your plugin is built with a specific commit, no matter the time or date, should produce the same version.

### Submitting

- Fork this repository, or use the GitHub web editor (press `.` in the repo, or press the ✏ icon on an existing manifest)
- In your fork, make `stable/(plugin name)/manifest.toml` (or `testing/live/(plugin name)/manifest.toml` - note that we prefer that new plugins go to `testing/live`, so that the wrinkles can be worked out before they go out to the wider audience). For more information, [see here](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md#guide-level-explanation).

  ```toml
  [plugin]
  repository = "https://github.com/goatcorp/SamplePlugin.git"
  commit = "765d9bb434ac99a27e9a3f2ba0a555b55fe6269d"
  owners = ["goaaats"]
  project_path = "SamplePlugin"
  changelog = "Added Herobrine"
  ```

- Place the images for your plugin in an `images` subfolder: `stable/(plugin name)/images`.
  - Please note this will be [streamlined at some point in the future](https://github.com/goatcorp/DIPs/pull/45). This has not been [implemented yet](https://github.com/goatcorp/DalamudPackager/issues/9). If you can help, we'd love to hear from you!
- Make the PR. If you're using the GitHub web editor, this will be automatic.

You'll also need to be using DalamudPackager; please check the SamplePlugin for an example. If you need help, please reach out.

## Updating your plugin

Just edit the commit hash in your manifest. Please always make your updates from a new branch, to make it cleaner for us to review.

## Rebuilding in a PR

If you want to trigger a re-build of your PR, just post a comment with the content "bleatbot, rebuild".

## Secrets

If your build process requires secrets, or you want to include a secret in your plugin, use [this page](https://goatcorp.github.io/plogon-secrets/) to encrypt the secret, to be included via your manifest. It will then be made available to your plugin's MSBuild/build script via environment variables, as per the instructions on the page.

---

When submitting a plugin, please consider our [Acceptable Use Policy](<https://github.com/goatcorp/FFXIVQuickLauncher/wiki/Acceptable-Use-Policy-(Official-Plugin-Repository)>) & [Terms of Service](<https://github.com/goatcorp/FFXIVQuickLauncher/wiki/Terms-and-Conditions-of-Use-(XIVLauncher,-Dalamud-&-Official-Plugin-Repository)>), which, for example, detail the rights you need to grant us when uploading a plugin to this repository. 

Please review the [plugin adoption policy](https://github.com/goatcorp/faq/blob/main/development.md#adoption) to understand what happens if you abandon your plugin. The FAQ also provides instructions on how to submit a plugin if taking over from another developer.
