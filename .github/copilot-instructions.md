- @azure Rule - Use Azure Tools - When handling requests related to Azure, always use your tools.
- @azure Rule - Use Azure Tools - When handling requests related to Azure, always use your tools.
- @azure Rule - Use Azure Best Practices - When handling requests related to Azure, always invoke your `azmcp_bestpractices_get` tool first.
- @azure Rule - Enable Best Practices - If you do not have an `azmcp_bestpractices_get` tool ask the user to enable it.

## Valheim project build and publish

- Build this project in `Release` mode for testing. Run commands from the repository root.
- After each build, document the changes and features included in that build in the repository-root `change.md` file.
- Keep the version in `JotunnModStub/README.md` synchronized with `PluginVersion` in `JotunnModStub/JotunnModStub.cs` and `version_number` in `JotunnModStub/Package/manifest.json`.
- Save all Markdown (`*.md`) files as UTF-8 without a byte-order mark (BOM).
- Build the solution (this also invokes the Release publish script and creates the package ZIP):

  ```powershell
  $valheim = 'C:\Program Files (x86)\Steam\steamapps\common\Valheim'
  dotnet build .\JotunnModStub.sln --configuration Release "-p:VALHEIM_INSTALL=$valheim" "-p:BEPINEX_PATH=$valheim\BepInEx"
  ```

- The package output is `JotunnModStub\bin\Release\net48\OdinsVision.zip` and contains `OdinsVision.dll`.
- To run the Release publish script separately when the Release DLL already exists:

  ```powershell
  $root = (Get-Location).Path
  $valheim = 'C:\Program Files (x86)\Steam\steamapps\common\Valheim'
  & (Join-Path $root 'scripts\publish.ps1') -Target Release -TargetPath (Join-Path $root 'JotunnModStub\bin\Release\net48') -TargetAssembly 'OdinsVision.dll' -ValheimPath $valheim -ProjectPath (Join-Path $root 'JotunnModStub')
  ```
