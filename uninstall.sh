#!/bin/bash

set -e

echo "[*] - Uninstalling NShell..."

echo "[*] - Setting default shell back to /bin/bash..."
chsh -s /bin/bash || {
    echo "[-] - Failed to change default shell."
    exit 1
}

USER_PROFILE=$(eval echo ~$USER)
N_SHELL_DIR="$USER_PROFILE/.nshell"
BIN_PATH="/usr/local/bin/nshell"

if [ -f "$BIN_PATH" ]; then
    echo "[*] - Removing $BIN_PATH & related folders/files..."
    sudo rm -rf "$BIN_PATH" "$N_SHELL_DIR"
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

echo "[+] - NShell has been fully uninstalled."
echo "[*] - Restart your terminal to return to bash."
