param(
    [Parameter(Mandatory)]
    [ValidateSet('Debug','Release')]
    [System.String]$Target,
    
    [Parameter(Mandatory)]
    [System.String]$TargetPath,
    
    [Parameter(Mandatory)]
    [System.String]$TargetAssembly,

    [Parameter(Mandatory)]
    [System.String]$ValheimPath,

    [Parameter(Mandatory)]
    [System.String]$ProjectPath,
    
    [System.String]$DeployPath
)

# Make sure Get-Location is the script path
Push-Location -Path (Split-Path -Parent $MyInvocation.MyCommand.Path)

# Test some preliminaries
("$TargetPath",
 "$ValheimPath",
 "$(Get-Location)\..\libraries"
) | % {
    if (!(Test-Path "$_")) {Write-Error -ErrorAction Stop -Message "$_ folder is missing"}
}

# Plugin name without ".dll"
$name = "$TargetAssembly" -Replace('.dll')

# Create the mdb file
$pdb = "$TargetPath\$name.pdb"
if (Test-Path -Path "$pdb") {
    Write-Host "Create mdb file for plugin $name"
    Invoke-Expression "& `"$(Get-Location)\..\libraries\Debug\pdb2mdb.exe`" `"$TargetPath\$TargetAssembly`""
}

# Main Script
Write-Host "Publishing for $Target from $TargetPath"

if ($Target.Equals("Debug")) {
    if ($DeployPath.Equals("")){
      $DeployPath = "$ValheimPath\BepInEx\plugins"
    }

    $legacyPlug = Join-Path $DeployPath "Monostar"
    if (Test-Path $legacyPlug) {
        @("Monostar.dll", "Monostar.pdb", "Monostar.dll.mdb") | ForEach-Object {
            Remove-Item -Path (Join-Path $legacyPlug $_) -Force -ErrorAction SilentlyContinue
        }
        if ((Get-ChildItem -Path $legacyPlug -Force | Measure-Object).Count -eq 0) {
            Remove-Item -Path $legacyPlug -Force
        }
    }
    
    $plug = New-Item -Type Directory -Path "$DeployPath\$name" -Force
    Write-Host "Copy $TargetAssembly to $plug"
    Copy-Item -Path "$TargetPath\$name.dll" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.pdb" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.dll.mdb" -Destination "$plug" -Force
}

if($Target.Equals("Release")) {
    Write-Host "Packaging for ThunderStore..."
    $Package="Package"
    $PackagePath="$ProjectPath\$Package"

    Remove-Item -Path "$TargetPath\Monostar.dll", "$TargetPath\Monostar.pdb", "$TargetPath\Monostar.dll.mdb", "$TargetPath\Monostar.zip" -Force -ErrorAction SilentlyContinue
    Remove-Item -Path "$PackagePath\plugins\Monostar.dll" -Force -ErrorAction SilentlyContinue
    Write-Host "$PackagePath\$TargetAssembly"
    New-Item -Type Directory -Path "$PackagePath\plugins" -Force
    Copy-Item -Path "$TargetPath\$TargetAssembly" -Destination "$PackagePath\plugins\$TargetAssembly" -Force
    Copy-Item -Path "$ProjectPath\README.md" -Destination "$PackagePath\README.md" -Force
    Compress-Archive -Path "$PackagePath\*" -DestinationPath "$TargetPath\$name.zip" -Force
}

# Pop Location
Pop-Location