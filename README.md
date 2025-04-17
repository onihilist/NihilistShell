# NihilistShell 🟢

> A custom C# interactive shell, customizable.

![NihilistShell Preview](https://github.com/user-attachments/assets/6a22fa38-21db-4ae2-abd4-44d34239044e)

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

### 📡 Roadmap

- [ ] Plugin support (dynamic loading)
- [ ] Autocomplete
- [ ] Command history
- [ ] Profiles and theme switching
- [ ] AI shell assistant (`shizuka-ai`)

---

### 🔧 Troubleshooting

If you have any problem with **NihilistShell**, or it locks you out of a proper shell,  
you can forcefully switch back to `bash` like this:

```bash
sudo sed -i 's|/usr/local/bin/nihilistshell|/bin/bash|' /etc/passwd
```