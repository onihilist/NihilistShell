
![NShellBanner](https://github.com/user-attachments/assets/f4feb3d9-3105-459f-b9da-c37df1b67446)

> A custom C# interactive shell, customizable.

![NihilistShell Preview](https://github.com/user-attachments/assets/88d01dd0-cae6-4535-a85f-202e30b67a14)


---

### 🧠 What is NihilistShell?

**NihilistShell** is a lightweight, extensible shell written in C# using [.NET 8.0+].  
It's designed for hackers, shell lovers, and those who enjoy boot sequences.

⚠️ This is an **alpha build** — many features are experimental, and some commands might fail gloriously.

---

### ✨ Features

- ✅ Custom interactive shell interface
- ✅ Built-in command loader with registration system
- ✅ Command metadata (`Description`, `IsInteractive`, `RequiresRoot`, etc.)
- ✅ Bash-like `cd`, `nano`, and fallback to real system commands
- ✅ Spectre.Console markup support for colors, glitches, animations
- ✅ Full AOT support (with manual command registration)
- ✅ Future-proof extensibility (plugin-style architecture)

---

### 🚀 Installation

Clone the repo and install:

```bash
git clone https://github.com/your-username/NihilistShell.git
cd NihilistShell
chmod +x install.sh
./install.sh
```

---

### 📡 Roadmap v1.0.0

- [OPEN] Plugin support (dynamic loading)
- [PROGRESS] Autocomplete
- [PROGRESS] Command history
- [OPEN] Fix neofetch shell version
- [OK] Profiles and theme switching
- [OK] Remove Bash FallBack
- [OK] Themes & ThemeLoader

---

### 🔧 Troubleshooting

If you have any problem with **NihilistShell**, or it locks you out of a proper shell,  
you can forcefully switch back to `bash` like this:

```bash
sudo sed -i "s|/usr/local/bin/nihilistshell|/bin/bash|" /etc/passwd
```

If you got the error "bad interpreter" when running `install.sh` try to run this :

```bash
sudo apt update
sudo apt install dos2unix
dos2unix install.sh
```
