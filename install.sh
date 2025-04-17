#!/bin/bash

set -e

echo "[*] - Compiling NihilistShell..."
dotnet publish NihilistShell.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:PublishTrimmed=true \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    -o ./publish

BINARY="./publish/NihilistShell"

if [ ! -f "$BINARY" ]; then
    echo "[-] - Build failed or binary not found at $BINARY"
    exit 1
fi

echo "[*] - Applying executable permissions..."
chmod +x "$BINARY"

echo "[*] - Copying to /usr/local/bin/ as 'nihilistshell'..."
sudo cp "$BINARY" /usr/local/bin/nihilistshell

echo "[*] - Adding to /etc/shells if not already present..."
if ! grep -Fxq "/usr/local/bin/nihilistshell" /etc/shells; then
    echo "/usr/local/bin/nihilistshell" | sudo tee -a /etc/shells > /dev/null
fi

echo "[*] - Setting NihilistShell as the default shell for user $USER..."
chsh -s /usr/local/bin/nihilistshell
export SHELL=/usr/local/bin/nihilistshell

echo "[+] - Installation complete. Restart your session or run: nihilistshell"
