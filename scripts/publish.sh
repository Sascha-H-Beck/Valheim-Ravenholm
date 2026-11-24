#!/bin/sh

target="Debug"
targetPath="JotunnModStub/bin/$target/net48"
targetAssembly="OdinsVision.dll"
valheimPath=""
bepinexPath=""
deployPath=""
projectPath="./JotunnModStub"

while [ "$#" -gt 0 ]; do
  case "$1" in
    --target)
        target="$2"; shift 2 ;;
    --target-path)
        targetPath="$2"; shift 2 ;;
    --target-assembly)
        targetAssembly="$2"; shift 2 ;;
    --valheim-path)
        valheimPath="$2"; shift 2 ;;
    --bepinex-path)
        bepinexPath="$2"; shift 2 ;;
    --deploy-path)
        deployPath="$2"; shift 2 ;;
    --project-path)
        projectPath="$2"; shift 2 ;;
    *)
        echo "Warning: Unknown argument $1" >&2; shift ;;
  esac
done

# order of precedence: MOD_DEPLOYPATH > BEPINEX_PATH > VALHEIM_INSTALL > default path
if [ -z "$deployPath" ]; then
    if [ -z "$bepinexPath" ]; then
        if [ -z "$valheimPath" ]; then
            deployPath="$HOME/.local/share/Steam/steamapps/common/Valheim/BepInEx/plugins"
        else
            deployPath="$valheimPath/BepInEx/plugins"
        fi
    else
        deployPath="$bepinexPath/plugins"
    fi
fi

# strip .dll extension
name=$(echo "$targetAssembly" | sed 's/\.dll//')

if [ "$target" = "Debug" ]; then
    legacyPlug="$deployPath/Monostar"
    if [ -d "$legacyPlug" ]; then
        rm -f "$legacyPlug/Monostar.dll" "$legacyPlug/Monostar.pdb" "$legacyPlug/Monostar.dll.mdb"
        rmdir "$legacyPlug" 2>/dev/null || true
    fi
    plug="$deployPath/$name"
    echo "Copying $targetAssembly to $plug"

    mkdir -p "$plug"
    cp "$targetPath/$targetAssembly" "$plug"
    # copy if it exists
    [ -e "$targetPath/$name.pdb" ] && cp "$targetPath/$name.pdb" "$plug"
fi

if [ "$target" = "Release" ]; then
    packagePath="$projectPath/Package"
    mkdir -p "$packagePath/plugins"
    rm -f "$targetPath/Monostar.dll" "$targetPath/Monostar.pdb" "$targetPath/Monostar.dll.mdb" "$targetPath/Monostar.zip" "$projectPath/Monostar.zip"
    rm -f "$packagePath/plugins/Monostar.dll"
    cp "$targetPath/$targetAssembly" "$packagePath/plugins/"
    cp "$projectPath/README.md" "$packagePath/"

    if command -v zip > /dev/null; then
        [ -e "$name.zip" ] && rm "$name.zip"
        cd "$packagePath"
        zip -r "../$name.zip" . > /dev/null
        echo "Build successful, your zip is ready for upload at $(realpath ../$name.zip)."
    else
        echo "Skipping plugin zipping, zip command isn't available."
    fi
fi
