
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
- ✅ Handle customs commands, but load also `/usr/bin`, `/usr/sbin`..ect commands.
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

### 🎨 Custom themes

This is a little exemple of an custom theme.
If you are using `format_top/bottom` & `corner_top/bottom`, `format` will be ignored.
For making a single line prompt use `format`, and double line `format_top/bottom`.
Exemple :
```json
{
  "name": "test",
  "format": "[bold green][[{user}@{host}]][/][white][[{cwd}]] >>[/]",
  "format_top": "[bold cyan]{user}@{host}[/]",
  "format_bottom": "[bold yellow]{cwd}[/]",
  "corner_top": "[bold magenta]\u250c[/]",
  "corner_bottom": "[bold magenta]\u2514[/]",
  "ls_colors": "di=34:fi=37:ln=36:pi=33:so=35:ex=32"
}
```

The name of the theme is `test`, no matter what the file is named.
So enter the command : `settheme test`.
This is the result :

![Preview Test Theme](https://github.com/user-attachments/assets/c54efcb9-c0a8-48e2-88c9-644c1bd7ccf5)

---

### 📡 Roadmap v1.0.0

- [OPEN] Plugin support (dynamic loading)
- [OPEN] Fix neofetch shell version
- [OPEN] Fix interactive commands/scripts running configuration
- [PROGRESS] Autocomplete
- [PROGRESS] Command history
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
