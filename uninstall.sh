#!/bin/bash

echo "🚨 Uninstalling NeonShell..."

# 1. Switch back to bash (to avoid being locked out)
echo "🔁 Setting default shell back to /bin/bash..."
chsh -s /bin/bash || { echo "❌ Failed to change default shell."; exit 1; }

# 2. Remove the executable
if [ -f "/usr/local/bin/neonshell" ]; then
    echo "🗑️ Removing /usr/local/bin/neonshell..."
    sudo rm /usr/local/bin/neonshell
else
    echo "ℹ️ No executable found at /usr/local/bin/neonshell"
fi

# 3. Clean up /etc/shells
if grep -Fxq "/usr/local/bin/neonshell" /etc/shells; then
    echo "🧽 Cleaning /etc/shells..."
    sudo sed -i '\|/usr/local/bin/neonshell|d' /etc/shells
else
    echo "ℹ️ Entry not found in /etc/shells"
fi

# 4. Optionally delete the publish folder
read -p "❓ Do you also want to delete the ./publish folder? [y/N]: " confirm
if [[ "$confirm" =~ ^[Yy]$ ]]; then
    rm -rf ./publish
    echo "✅ ./publish folder deleted."
fi

echo "✅ NeonShell has been fully uninstalled."
echo "🔁 Restart your terminal to return to bash."
