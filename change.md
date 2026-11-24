# Build changes

## OdinsVision Release

- Renamed the plugin assembly and package to `OdinsVision.dll` and `OdinsVision.zip`.
- Updated the BepInEx display name, assembly metadata, manifest, and package README to `OdinsVision`.
- Preserved the existing plugin GUID and input button identifier so existing BepInEx configuration and bindings remain compatible.
- Kept the gameplay features unchanged: press `F8` to toggle red vision highlighting for living non-player characters; the shortcut remains configurable as `ToggleVisionKey`.
- Updated publishing to remove the previous `Monostar` DLL artifacts to prevent old and new builds from being packaged or deployed together.
- Release build succeeded. The ZIP contains `plugins/OdinsVision.dll`, `icon.png`, `manifest.json`, and `README.md`.

## Clean Release rebuild

- Cleaned the Release build output and rebuilt the package without changing plugin features.
- Verified `OdinsVision.dll` and `OdinsVision.zip` were regenerated; the ZIP contains `plugins/OdinsVision.dll`, `icon.png`, `manifest.json`, and `README.md`.
- The Release build succeeded with 13 warnings, including unresolved Valheim assembly references and existing source warnings.

## Release rebuild

- Rebuilt the Release configuration with no gameplay or feature changes; the `F8` vision toggle and `ToggleVisionKey` configuration remain unchanged.
- Verified the regenerated `OdinsVision.zip` contains `plugins/OdinsVision.dll`, `icon.png`, `manifest.json`, and `README.md`.
- The build succeeded with 11 warnings, primarily unresolved Valheim assembly references.

## Release package README update

- Replaced the project README used by packaging with the requested OdinsVision alpha README, covering installation, F8 activation, `ToggleVisionKey`, changelog, known issues, and GitHub URL.
- Rebuilt successfully in Release mode and verified that `OdinsVision.zip` contains the requested README alongside `OdinsVision.dll`, `icon.png`, and `manifest.json`.
- The README says version `0.0.1`; the plugin and package manifest remain at `0.0.3` and were not changed.

## README version synchronization rebuild

- Updated the README version to match the current plugin and package manifest version, `0.0.3`.
- Rebuilt successfully in Release mode and verified the packaged README in `OdinsVision.zip` reports version `0.0.3`.
- The ZIP contains `plugins/OdinsVision.dll`, `icon.png`, `manifest.json`, and `README.md`; the build completed with 11 warnings.

## Markdown encoding rule and Release rebuild

- Added a Copilot instruction requiring Markdown files to be saved as UTF-8 without a BOM.
- Rebuilt the Release package without gameplay changes; verified all 5 repository Markdown files and the README inside `OdinsVision.zip` are BOM-free.
- The ZIP contains `plugins/OdinsVision.dll`, `icon.png`, `manifest.json`, and `README.md`. The build succeeded with 11 warnings.

## Thunderstore version bump

- Bumped the plugin, package manifest, and README version from `0.0.3` to `0.0.4` to avoid reusing the existing Thunderstore package version.
- Rebuilt successfully in Release mode and verified `OdinsVision.zip` contains matching README and manifest version `0.0.4` with `plugins/OdinsVision.dll`, `icon.png`, and `README.md`.
- No gameplay behavior changed. The build completed with 13 warnings.
