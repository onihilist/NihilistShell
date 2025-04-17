#!/bin/bash

set -e

echo "[*] - Uninstalling NihilistShell..."

echo "[*] - Setting default shell back to /bin/bash..."
chsh -s /bin/bash || {
    echo "[-] - Failed to change default shell."
    exit 1
}

BIN_PATH="/usr/local/bin/nihilistshell"

if [ -f "$BIN_PATH" ]; then
    echo "[*] - Removing $BIN_PATH..."
    sudo rm "$BIN_PATH"
else
    echo "[-] - No executable found at $BIN_PATH"
fi

if grep -Fxq "$BIN_PATH" /etc/shells; then
    echo "[*] - Cleaning /etc/shells..."
    sudo sed -i "\|$BIN_PATH|d" /etc/shells
else
    echo "[-] - Entry not found in /etc/shells"
fi

read -p "[?] - Do you also want to delete the ./publish folder? [y/N]: " confirm
if [[ "$confirm" =~ ^[Yy]$ ]]; then
    rm -rf ./publish
    echo "[+] - ./publish folder deleted."
fi

echo "[+] - NihilistShell has been fully uninstalled."
echo "[*] - Restart your terminal to return to bash."
