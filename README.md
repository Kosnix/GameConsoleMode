# GameConsoleMode (GCM) 🎮

**GameConsoleMode** (GCM) is a powerful C# application designed to transform your PC into a fully functional gaming console while retaining the flexibility and capabilities of the Windows operating system. GCM achieves this by temporarily replacing the Windows shell with a custom gaming environment, hiding the complexities of Windows, and launching your chosen game launcher for a streamlined gaming experience.

With a variety of advanced features, GCM caters to gamers who want a console-like interface without sacrificing the versatility of their PC. Whether you're a Steam enthusiast, a Playnite user, or prefer your own custom launcher, GCM is here to redefine your gaming setup.

---

## ⚠️ Important Notice

- This software is **Beta** and modifies critical parts of the Windows environment, including the registry.
- Use it **at your own risk**, as improper use or unexpected issues could impact your system.
- The developer assumes no responsibility for any damages or data loss resulting from the use of this software.
- We recomend to disable UAC it reduces security but makes the experience seamless.

---

## ✨ Features

### 🌟 Core Functionalities

#### **1. Shell Replacement**
GCM replaces the Windows Explorer shell with its own interface, effectively hiding the Windows desktop, taskbar, and other elements. This creates a console-like experience where only your game launcher is visible.

#### **2. Support for Multiple Launchers**
- Seamlessly integrates with popular launchers like:
  - **Steam**
  - **Playnite**
  - Custom launchers
- Automatically detects and launches the configured game launcher upon startup.

#### **3. Mouse Cursor Management**
- Dynamically hides or shows the mouse cursor based on the state of the launcher or game.
- Ensures a clean and immersive gaming experience without unnecessary distractions.

#### **4. Screen Management**
- Recognizes multi-screen setups and manages screen configurations during startup and shutdown.
- Restores your original screen layout after exiting GCM.

#### **5. Audio Management**
- Lets you define a **default audio output device** that automatically activates when GCM starts.
- Perfect for setups with multiple audio devices, ensuring your games always use the desired output.

#### **6. Startup Video Integration**
- Customize your experience with a **startup video** that plays during GCM initialization.
- Creates an immersive transition into your gaming environment while allowing time for your launcher to load.

#### **7. Controller Shortcuts**
- Launch GCM or return to Windows using shortcuts mapped to your game controller.
- Adds convenience and flexibility to your gaming sessions.

---

### 🚀 Latest Features and Improvements

#### **1. Advanced Mouse Controller Replacement with Joyxoff**
- GCM now integrates **Joyxoff**, a highly advanced software solution that replaces older Mouse Controller functionalities.
- Joyxoff offers improved precision, functionality, and compatibility with GCM.

#### **2. CSS Loader for Steam Big Picture**
- Customize the **Steam Big Picture** mode using a CSS Loader.
- GCM allows you to install the CSS Loader directly, detects it during startup, and launches it automatically alongside the Steam Big Picture mode.

#### **3. Full Integration with DisplayFusion**
- Manage advanced screen configurations with **DisplayFusion**:
  - Install DisplayFusion via GCM.
  - Set up custom profiles for different screen layouts or resolutions.
  - GCM detects and handles these profiles seamlessly during startup and shutdown.

#### **4. Enhanced Audio Management**
- Select a **default audio output device** that is automatically activated when GCM launches.
- Ensure consistent audio behavior across sessions, especially in multi-device setups.

---

## 🛠️ Configuration

### **Using `Settings.exe`**
To configure GameConsoleMode, follow these steps:

1. Navigate to Desktop `GCM-Settings`.
2. Launch `GCM-Settings`.
3. Configure the following options:
   - **Launcher Settings**:
     - Select your preferred launcher: Steam, Playnite, or a custom executable.
     - Specify the file path to your game launcher.
   - **Startup Video**:
     - Choose a video file to play during GCM startup.
   - **Audio Settings**:
     - Select a default audio output device.
   - **Screen Management**:
     - Adjust screen layout preferences for multi-monitor setups.
   - **Additional Features**:
     - Enable or disable advanced features like CSS Loader or DisplayFusion integration.
4. Save your changes and restart GCM to apply them.

---

## 📷 Settings Interface

Below are screenshots of the configuration interface for better clarity:

### **General Settings**
![General Settings](https://github.com/Kosnix/GameConsoleMode/blob/master/settings.png)

### **Startup Settings**
![Startup Settings](https://github.com/Kosnix/GameConsoleMode/blob/master/startup.png)

### **Additional Features**
![Additional Features](https://github.com/Kosnix/GameConsoleMode/blob/master/additional.png)

---

## 🚀 Usage Guide

### **1. Launching GCM**
- Run the `GameConsoleMode.exe` application.
- GCM will initialize, verify required files, and configure logging.

### **2. Entering Gaming Console Mode**
- Once GCM starts, the Windows interface will be hidden.
- Your configured game launcher will launch automatically.

### **3. Immersive Gaming**
- Enjoy your games with no Windows interface distractions.
- GCM handles all necessary system adjustments for a seamless experience.

### **4. Returning to Windows**
- Close your game launcher, and GCM will automatically restore the Windows desktop environment.
- Alternatively, use a mapped controller shortcut to exit.

---

## 🤝 Contributing

GameConsoleMode is open to contributions from the community.  
- Special thanks to [toonymak1993] for active participation and valuable contributions.
- To contribute:
  - Submit **issues** for bug reports or feature requests.
  - Open **pull requests** with proposed improvements or fixes.

---

## 📞 Contact

For inquiries or support, please reach out via Discord: **`.kosnix`**

---

GameConsoleMode continues to evolve, bringing new features and improvements with each release.  
Try it out, and let us know how we can make it even better! 🎉
