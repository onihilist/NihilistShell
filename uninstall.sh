#!/bin/bash

echo "[*] - Uninstalling NihilistShell..."

echo "[*] - Setting default shell back to /bin/bash..."
chsh -s /bin/bash || { echo "❌ Failed to change default shell."; exit 1; }

if [ -f "/usr/local/bin/nihilistshell" ]; then
    echo "[*] - Removing /usr/local/bin/nihilistshell..."
    sudo rm /usr/local/bin/neonshell
else
    echo "[-] - No executable found at /usr/local/bin/nihilistshell"
fi

if grep -Fxq "/usr/local/bin/nihilistshell" /etc/shells; then
    echo "[*] - Cleaning /etc/shells..."
    sudo sed -i '\|/usr/local/bin/nihilistshell|d' /etc/shells
else
    echo "[-] - Entry not found in /etc/shells"
fi

read -p "[?] - Do you also want to delete the ./publish folder? [y/N]: " confirm
if [[ "$confirm" =~ ^[Yy]$ ]]; then
    rm -rf ./publish
    echo "✅ ./publish folder deleted."
fi

echo "[+] - NihilistShell has been fully uninstalled."
echo "[*] - Restart your terminal to return to bash."
