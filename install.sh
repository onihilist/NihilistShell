#!/bin/bash

set -e

if ! command -v dotnet &> /dev/null; then
    echo "[-] - .NET SDK (dotnet) is not installed."
    read -p "[?] - Do you want to install .NET SDK now? (Y/n): " INSTALL_DOTNET
    INSTALL_DOTNET=${INSTALL_DOTNET:-Y}

    if [[ "$INSTALL_DOTNET" =~ ^[Yy]$ ]]; then
        echo "[*] - Installing .NET SDK for Ubuntu..."

        wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
        sudo dpkg -i packages-microsoft-prod.deb
        rm packages-microsoft-prod.deb

        sudo apt update
        sudo apt install -y apt-transport-https
        sudo apt update
        sudo apt install -y dotnet-sdk-8.0

        echo "[+] - .NET SDK installed successfully."
    else
        echo "[-] - .NET SDK is required. Aborting."
        exit 1
    fi
fi

USER_PROFILE=$(eval echo ~$USER)
N_SHELL_DIR="$USER_PROFILE/.nshell"
HISTORY_FILE="$N_SHELL_DIR/.nhistory"
THEMES_DIR="$N_SHELL_DIR/themes"
PLUGINS_DIR="$N_SHELL_DIR/plugins"
CONFIG_FILE="$N_SHELL_DIR/nshell.conf.json"

echo "[*] - Creating directories/files if they don't exist..."

mkdir -p "$THEMES_DIR"
mkdir -p "$PLUGINS_DIR"
touch "$HISTORY_FILE"
curl -L "https://gist.githubusercontent.com/onihilist/8bf7548dc7478f1b6af2db4bdc0c668d/raw/28f46aa5ebcb27b4f0edb302e2ff4347f49a2f83/nshell.conf.json" -o $CONFIG_FILE

echo "[+] - Directories/Files created or already exist: "
echo " - $THEMES_DIR"
echo " - $PLUGINS_DIR"
echo " - $HISTORY_FILE"
echo " - $CONFIG_FILE"

echo "[*] - Compiling NShell..."
dotnet publish NShell.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:PublishTrimmed=true \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    -o ./publish

BINARY="./publish/NShell"

if [ ! -f "$BINARY" ]; then
    echo "[-] - Build failed or binary not found at $BINARY"
    exit 1
fi

echo "[*] - Applying executable permissions..."
chmod +x "$BINARY"

echo "[*] - Copying to /usr/local/bin/ as 'nshell'..."
sudo cp "$BINARY" /usr/local/bin/nshell

echo "[*] - Adding /usr/local/bin/nshell to /etc/shells if not already present..."
if ! grep -qx "/usr/local/bin/nshell" /etc/shells; then
    echo "/usr/local/bin/nshell" | sudo tee -a /etc/shells > /dev/null
    echo "[+] - Added NShell to /etc/shells."
fi

echo "[*] - Setting NShell as the default shell for user $USER..."
chsh -s /usr/local/bin/nshell
export SHELL=/usr/local/bin/nshell

echo "[+] - Installation complete. Restart your session or run: nshell"
