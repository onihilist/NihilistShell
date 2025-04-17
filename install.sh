#!/bin/bash

echo "🔧 Compiling NeonShell..."
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish || exit 1

echo "🛡️  Applying executable permissions..."
chmod +x ./publish/NeonShell

echo "📦 Copying to /usr/local/bin..."
sudo cp ./publish/NeonShell /usr/local/bin/nihilistshell

echo "📜 Adding to /etc/shells..."
if ! grep -Fxq "/usr/local/bin/nihilistshell" /etc/shells; then
    echo "/usr/local/bin/nihilistshell" | sudo tee -a /etc/shells
fi

echo "👤 Setting NihilistShell as the default shell for user $USER..."
chsh -s /usr/local/bin/nihilistshell
export SHELL=/usr/local/bin/nihilistshell

echo "✅ Installation complete. Restart your session or run: /usr/local/bin/nihilistshell"
