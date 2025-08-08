# AetherFM - Radio Streaming for Final Fantasy XIV

**Note:** AetherFM is only supported on Windows. Due to its reliance on the NAudio library, audio playback is not available on Linux or non-Windows versions of Dalamud.

<div align="center">

![AetherFM Icon](https://raw.githubusercontent.com/SalvatoreDevelopment/AetherFM/master/images/icon.png)

**Listen to thousands of real radio stations directly in-game while playing Final Fantasy XIV!**

[![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)](https://github.com/SalvatoreDevelopment/AetherFM/releases)
[![License](https://img.shields.io/badge/license-AGPL--3.0--or--later-green.svg)](LICENSE)
[![Dalamud](https://img.shields.io/badge/Dalamud-Plugin-orange.svg)](https://github.com/goatcorp/Dalamud)

*Modern UI, instant playback, and seamless integration with your FFXIV experience*

</div>

---

## 🎵 Features

### 🎧 **Real-time Radio Streaming**
- **Thousands of Stations:** Access to a vast collection of radio stations from around the world
- **Automatic Loading:** All stations loaded from Radio Browser API (HTTP MP3 streams only)
- **Instant Playback:** Click and play - no setup or configuration required
- **Custom URLs:** Enter any direct stream URL for stations not in the database

### 🔍 **Advanced Search & Discovery**
- **Smart Search:** Search by station name, genre, or country
- **Multiple Filters:** Filter by country, genre, or language
- **Duplicate Prevention:** Automatic filtering of duplicate stations (by URL and Name+URL)
- **Real-time Results:** Instant search results as you type

### 🎨 **Modern User Interface**
- **Resizable Window:** Adjust the interface to your preference
- **Professional Table:** Clean columns showing Name, Country, Genre, and Play/Stop controls
- **Visual Feedback:** Clear indication of currently playing station
- **Error Handling:** User-friendly error messages for stream issues
- **Accessibility:** High contrast, tooltips, and keyboard navigation

### 🎛️ **Audio Controls**
- **Volume Slider:** Adjust volume directly from the interface
- **Play/Stop Toggle:** Simple one-click control for each station
- **State Management:** Robust handling of play/stop states
- **Stream Recovery:** Automatic retry for failed connections

### 🌍 **Multilingual Support**
- **English & Italian:** Full interface support for both languages
- **Localized UI:** All text elements properly translated
- **Cultural Adaptation:** Interface adapted for different regions

### 🛠️ **Developer-Friendly**
- **Local Development:** Rapid testing with devPlugins deployment
- **Repository Testing:** Easy switch between local and repository versions
- **Build Automation:** PowerShell script for all deployment scenarios
- **Version Management:** Seamless workflow from development to release

---

## 📋 Requirements

- **Final Fantasy XIV** (any platform)
- **XIVLauncher** with **Dalamud** installed
- **.NET 7.0** or higher
- **Internet connection** for streaming and station updates

---

## 🚀 Installation

### For Users

1. **Add the Custom Repository:**
   - Open XIVLauncher
   - Go to `Settings → Experimental → Custom Plugin Repositories`
   - Add this URL:
     ```
     https://raw.githubusercontent.com/SalvatoreDevelopment/AetherFM/main/pluginmaster.json
     ```

2. **Install the Plugin:**
   - Search for "AetherFM" in the plugin installer
   - Click Install and restart the game if prompted

3. **Start Listening:**
   - Use `/aetherfm` command or click the plugin button in-game
   - Browse, search, and enjoy your favorite radio stations!

### For Developers

See the [Development & Release Workflow](#-development--release-workflow) section below.

---

## 🎮 Usage Guide

### Basic Controls
- **Open Radio Player:** `/aetherfm` command or plugin button
- **Search Stations:** Use the search bar at the top
- **Filter by Country:** Use the country dropdown
- **Play/Stop:** Click the button next to any station
- **Volume Control:** Use the slider at the top of the window
- **Custom Stream:** Enter any direct stream URL in the custom URL field

### Advanced Features
- **Station Information:** Hover over stations for additional details
- **Error Recovery:** Failed streams show clear error messages
- **Responsive Design:** Window adapts to different screen sizes
- **Keyboard Navigation:** Use Tab and Enter keys for accessibility

---

## 🛠️ Development & Release Workflow

### Prerequisites
- Visual Studio 2022 or VS Code with C# extensions
- PowerShell 5.1 or higher
- Git for version control

### Local Development (devPlugins)
For rapid development and testing:
```powershell
.\build.ps1 -DeployLocal
```
- Copies the plugin to `%APPDATA%\XIVLauncher\devPlugins\AetherFM`
- XIVLauncher automatically loads this version (overrides repository version)
- Perfect for fast iteration and debugging

### Test Repository Version
To test the plugin as end users would experience it:
```powershell
.\build.ps1 -SwitchToRepo
```
- Removes the local devPlugins version
- XIVLauncher loads the version from your repository
- Essential for user experience testing

### Create Release Package
To prepare a release for distribution:
```powershell
.\build.ps1 -Zip
```
- Creates `AetherFM.zip` ready for GitHub release
- Includes all necessary files and assets
- Upload this ZIP to your GitHub release

### Complete Release Process

1. **Update Version Numbers:**
   - Update version in `AetherFM.json`
   - Update version in `pluginmaster.json`
   - Update version in `CHANGELOG.md`

2. **Build and Test:**
   ```powershell
   .\build.ps1 -DeployLocal    # Test locally
   .\build.ps1 -SwitchToRepo   # Test repository version
   .\build.ps1 -Zip            # Create release package
   ```

3. **Publish Release:**
   - Create new release on GitHub
   - Upload the `AetherFM.zip` file
   - Update `pluginmaster.json` with new version info
   - Push changes to repository

### Best Practices

⚠️ **Critical:** Never leave both devPlugins and repository versions active simultaneously
- **devPlugins always takes priority** over repository versions
- Use `-SwitchToRepo` before testing repository functionality
- Use `-DeployLocal` for development iterations

**Development Workflow:**
- Use `-DeployLocal` for rapid development and testing
- Use `-SwitchToRepo` for user experience validation
- Use `-Zip` for release preparation
- Always update version numbers before releasing

---

## 🎨 Customization

### Adding Custom Icons
1. Place your icon (256x256 PNG recommended) in the `images/` folder
2. Upload to GitHub and get the RAW link
3. Update `pluginmaster.json` with the icon URL
4. Update this README with the icon link

### Repository Configuration
The `pluginmaster.json` file contains:
- Plugin metadata (name, version, description)
- Download links for different Dalamud versions
- Icon and changelog URLs
- Author and repository information

---

## 📝 Changelog

See [CHANGELOG.md](CHANGELOG.md) for complete version history and detailed feature descriptions.

### Recent Highlights
- **v1.0.0:** Initial release with Radio Browser API integration
- Modern UI with search, filters, and volume controls
- Automatic station loading and duplicate filtering
- Comprehensive error handling and user feedback
- Developer workflow automation with build scripts

---

## 🐛 Troubleshooting

### Common Issues

**Plugin not loading:**
- Ensure XIVLauncher and Dalamud are up to date
- Check that the repository URL is correct
- Verify .NET 7.0+ is installed

**No stations appearing:**
- Check your internet connection
- Radio Browser API might be temporarily unavailable
- Try refreshing the plugin

**Audio not playing:**
- Verify your system audio is working
- Check if the stream URL is still valid
- Some stations may have geo-restrictions

**Build script errors:**
- Ensure PowerShell execution policy allows scripts
- Verify all required files are present
- Check that XIVLauncher is not running during deployment

---

## 📄 License

This project is licensed under the **AGPL-3.0-or-later** License - see the [LICENSE](LICENSE) file for details.

---

## 🤝 Contributing

We welcome contributions! Here's how you can help:

### Ways to Contribute
- **Bug Reports:** Open an issue with detailed reproduction steps
- **Feature Requests:** Suggest new features or improvements
- **Code Contributions:** Submit pull requests for bug fixes or features
- **Documentation:** Help improve this README or add code comments
- **Testing:** Test new releases and report issues

### Development Setup
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes and test thoroughly
4. Commit your changes: `git commit -m 'Add amazing feature'`
5. Push to the branch: `git push origin feature/amazing-feature`
6. Open a Pull Request

### Code Style
- Follow C# coding conventions
- Add comments for complex logic
- Include error handling
- Test your changes thoroughly

---

## 📞 Support & Community

### Getting Help
- **GitHub Issues:** [Open an issue](https://github.com/SalvatoreDevelopment/AetherFM/issues) for bugs or feature requests
- **Documentation:** Check this README and the [CHANGELOG.md](CHANGELOG.md)

### Community Guidelines
- Be respectful and helpful to other users
- Search existing issues before creating new ones
- Provide detailed information when reporting bugs
- Share your favorite radio stations with the community!

---

## 🙏 Acknowledgments

- **Dalamud Team:** For the amazing plugin framework
- **Radio Browser API:** For providing the station database
- **FFXIV Community:** For testing and feedback
- **Open Source Contributors:** For inspiration and tools

---

<div align="center">

**Happy listening in Eorzea!** 🎵✨

*May your adventures be accompanied by the perfect soundtrack*

[![GitHub stars](https://img.shields.io/github/stars/SalvatoreDevelopment/AetherFM?style=social)](https://github.com/SalvatoreDevelopment/AetherFM)
[![GitHub forks](https://img.shields.io/github/forks/SalvatoreDevelopment/AetherFM?style=social)](https://github.com/SalvatoreDevelopment/AetherFM)

</div>
