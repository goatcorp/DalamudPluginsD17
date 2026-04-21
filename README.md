# DalamudPluginsD17

This is the plugin repository for the [Dalamud plugin framework for Final Fantasy XIV](https://github.com/goatcorp/Dalamud). This repository implements [DIP17](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md) to make the submission process easier and faster.

---

## Table of Contents

- [Publishing your plugin](#publishing-your-plugin)
  - [Preparing your repository](#preparing-your-repository)
  - [Approval criteria](#approval-criteria)
  - [Technical criteria](#technical-criteria)
  - [Submitting](#submitting)
- [Updating your plugin](#updating-your-plugin)
- [Rebuilding in a PR](#rebuilding-in-a-pr)
- [Secrets](#secrets)
- [Policies & Guidelines](#policies-and-guidelines)

---

## Publishing your plugin

### Preparing your repository

Before submitting, make sure your plugin repository is in order:

1. Ensure your plugin is on a publicly accessible Git repo (GitHub, GitLab, or any self-hosted Git instance that allows HTTP clones without authentication).
2. Update your `.csproj`:
   - **(Preferred for most plugins)** Set your SDK to the latest `Dalamud.NET.Sdk` version. See the [sample plugin](https://github.com/goatcorp/SamplePlugin/blob/master/SamplePlugin/SamplePlugin.csproj#L2) for reference.
   - Alternatively, use `$(DalamudLibPath)` and [DalamudPackager](https://github.com/goatcorp/DalamudPackager) from NuGet for more advanced plugin needs.
3. Build your plugin in Release mode, then commit your `.csproj` and the newly generated lock file.

---

### Approval criteria

When the plugin approval group reviews your plugin, they will check for the following:

- Does it meet [our guidelines](https://dalamud.dev/plugin-publishing/restrictions), as agreed upon by multiple members of the group?
- Does it feature any combat elements? If so, are they purely informational, and do they show only information the player would normally know?
- Does it pass an informal code review?
- Does it install cleanly?
- Does the configuration window (if present) behave correctly?
- Does the base functionality of the plugin work (if testable easily)?
- Does it have no obvious technical issues?
- Is its JSON correctly formatted? (We hope to [make this unnecessary in future](https://github.com/goatcorp/DalamudPackager/issues/8).)
- If it is a new plugin, is it in the testing channel and not the stable channel? We want new plugins to be in testing to make it easier for the group to test, as well as reducing the impact of any unforeseen issues.
- Does it meet the [Technical criteria](#technical-criteria)?

These criteria are intended to prevent issues for users. We are happy to work with you to get you across the line - just reach out in the Discord.

---

### Technical criteria

There are a few technical requirements that must be met before submitting your plugin. They will make your plugin nicer to use.

- Your plugin **must have** an `icon.png` that is no larger than 512x512 and no smaller than 64x64, located in `images/`.
- For regular windows, such as settings and utility windows, you should use the [Dalamud Windowing API](https://dalamud.dev/api/Dalamud.Interface.Windowing/). It enhances windows with features like integration into the native UI closing order, pinning, and opacity controls. If it looks like a window, it should use the Windowing API. We will not reject updates to existing plugins for this, but we encourage everyone to upgrade.
- Your plugin's version and assembly version **must not** be based on a timestamp or a continually increasing build number. Every time your plugin is built from a specific commit, regardless of the time or date, should produce the same version.

---

### Submitting

1. Fork this repository, or use the GitHub web editor (press `.` in the repo, or press the pencil icon on an existing manifest).

2. In your fork, create one of the following files depending on your target channel:
   - `stable/(plugin name)/manifest.toml`
   - `testing/live/(plugin name)/manifest.toml`

   We prefer that new plugins go to `testing/live` so that any issues can be resolved before reaching a wider audience. For more information, [see the DIP17 spec](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md#guide-level-explanation).

   In the manifest, `maintainers` are GitHub usernames allowed to push updates to this plugin. `owners` are GitHub usernames considered an authority over the plugin — they are automatically maintainers and are the point of contact for any inquiries.

   ```toml
   [plugin]
   repository = "https://github.com/goatcorp/SamplePlugin.git"
   commit = "765d9bb434ac99a27e9a3f2ba0a555b55fe6269d"
   owners = ["plo"]
   maintainers = ["gon", "goaaats"]
   project_path = "SamplePlugin"
   changelog = "Added Herobrine"
   ```

3. Place the images for your plugin in an `images` subfolder next to your manifest:
   - `stable/(plugin name)/images/`
   - `testing/live/(plugin name)/images/`

   Please note this structure will be [streamlined in the future](https://github.com/goatcorp/DIPs/pull/45). If you can help, we would love to hear from you.

4. Open the PR. If you are using the GitHub web editor, this will happen automatically.
   - If you used AI tooling at any point, review the [AI Usage Policy](https://dalamud.dev/plugin-publishing/ai-policy) and disclose your level of AI use in the PR description. Entirely AI-generated submissions will be rejected, and undisclosed AI use may result in a ban.

You will also need to be using DalamudPackager - please check the SamplePlugin for an example. If you need help, please reach out.

---

## Updating your plugin

Edit the commit hash in your manifest to point to the new commit. Always make updates from a new branch to make the review process cleaner.

---

## Rebuilding in a PR

To trigger a re-build of your PR, post a comment with the content:

```
bleatbot, rebuild
```

---

## Secrets

If your build process requires secrets, or you want to include a secret in your plugin, use [this page](https://goatcorp.github.io/plogon/secrets/) to encrypt the secret for inclusion in your manifest. It will then be made available to your plugin's MSBuild or build script via environment variables, as described in the instructions on that page.

---

## Policies and guidelines

When submitting a plugin, please review the following:

- [Acceptable Use Policy](https://github.com/goatcorp/FFXIVQuickLauncher/wiki/Acceptable-Use-Policy-(Official-Plugin-Repository))
- [Terms of Service](https://github.com/goatcorp/FFXIVQuickLauncher/wiki/Terms-and-Conditions-of-Use-(XIVLauncher,-Dalamud-&-Official-Plugin-Repository)) - this includes the rights you grant us when uploading a plugin to this repository
- [Code of Conduct](https://dalamud.dev/code-of-conduct) - governs all participation in this repository, including issues, PRs, and reviews
- [AI Usage Policy](https://dalamud.dev/plugin-publishing/ai-policy) - governs the use of AI to assist with plugin creation and must be followed
- [Plugin Adoption Policy](https://dalamud.dev/faq/adoption) - explains what happens if you abandon your plugin, and includes instructions for taking over a plugin from another developer
