# GameConsoleMode (GCM) 🎮

**GameConsoleMode** (GCM) is a powerful C# application built with **WinUI 3**, designed to transform your PC into a fully functional gaming console while retaining the flexibility of the Windows operating system. GCM temporarily replaces the Windows shell with a custom gaming environment, providing a seamless console-like experience.

Whether you're a Steam enthusiast, a Playnite user, or prefer your own custom launcher, GCM offers a streamlined gaming experience without unnecessary Windows distractions.

Something like **SteamOS**, but powered by the **Windows engine**.

| ![start_gamemode](start_gamemode.png) | ![launcher](launcher.png) |
|--------------------------------|--------------------------------|
| ![functions2](functions2.png) | ![updated](updates.png) |

---

## ⚠️ Important Notice

- This software modifies critical parts of the Windows environment, including the registry.
- Use it **at your own risk**, as improper use or unexpected issues could impact your system.
- The developer assumes no responsibility for any damages or data loss resulting from the use of this software.
- It disable **User Account Control (UAC)** for a smoother experience, though this reduces security. (Only inside the gamemode)

---

## ✨ Features

### 🌟 Core Functionalities

#### **1. Shell Replacement**
GCM replaces the Windows Explorer shell with its own interface, hiding the Windows desktop, taskbar, and other elements, creating a pure gaming environment.

#### **2. Support for Multiple Launchers**
![General Settings](launcher.png)
- Seamlessly integrates with popular launchers like:
  - **Steam**
  - **Playnite**
  - Custom launchers
- Automatically launches the configured game launcher on startup.

- #### **3. Many Functions**
![General Settings](functions1.png)
- Functions like:
  - **CSSLOADER**
  - **JOYXOFF**
  - **WALLPAPER**
- and many more to come...
---

### 🚀 Latest Features and Improvements

#### **1. WinUI 3 Implementation**
- GCM is now built on **WinUI 3**, ensuring a modern and fluid user interface.
- Improved performance and responsiveness compared to older frameworks.

#### **2. Advanced Mouse Controller Replacement with Joyxoff**
- Integrated **Joyxoff** for superior mouse controller functionality.
- Offers better precision and compatibility with GCM.

#### **3. CSS Loader for Steam Big Picture**
- Customize the **Steam Big Picture** mode with a CSS Loader.
- GCM detects and launches it automatically alongside Steam Big Picture.

#### **4. Full Integration with DisplayFusion**
- Manage screen configurations using **DisplayFusion**:
  - Install and configure DisplayFusion directly through GCM.
  - Set up custom profiles for different screen layouts or resolutions.
  - GCM seamlessly handles profile switching during startup and shutdown.
 
#### **5. Update over GCM SETTINGS**

---

## 🫠 Configuration

### **Using `GCM SETTINGS`**
To configure GameConsoleMode, follow these steps:

1. Launch `GCM SETTINGS` after the installer. 
2. Configure the following options:
   - **Launcher Settings (Steam is standard)**:
     - Choose your preferred launcher (Steam, Playnite, or a custom executable).
     - Specify the file path to your game launcher.
   - **Additional Features in Functions**:
     - Enable or disable advanced features like CSS Loader or DisplayFusion integration.
3. Save your changes and start GCM at the Home Tab.

---

## 🖼️ Functions Interface

Below are screenshots of the Functions interface for better clarity:

![General Settings](functions1.png)
![Startup Settings](functions2.png)
![Additional Features](functions3.png)

---

## 🚀 Usage Guide

### **1. Launching GCM**
- Run the `GCM MODE` application.
- GCM will initialize, verify required files, and configure logging.

### **2. Entering Gaming Console Mode**
- Once GCM starts, the Windows interface will be hidden.
- Your configured game launcher will launch automatically.

### **3. Immersive Gaming**
- Enjoy your games without Windows interface distractions.
- GCM handles all necessary system adjustments for a seamless experience.

### **4. Returning to Windows**
- Close your game launcher, and GCM will automatically restore the Windows desktop environment.
---

## 🤝 Contributing

GameConsoleMode is open to contributions from the community.
- Special thanks to **toonymak1993** for active participation and valuable contributions.
- To contribute:
  - Submit **issues** for bug reports or feature requests.
  - Open **pull requests** with proposed improvements or fixes.

---

## 📞 Contact

For inquiries or support, reach out via Discord: **`.kosnix`** or **`.Toonymak`**

---

GameConsoleMode continues to evolve, bringing new features and improvements with each release.  
Try it out, and let us know how we can make it even better! 🎉

