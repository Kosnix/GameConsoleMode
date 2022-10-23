## GameConsoleMode
this program allows to switch from explorer to playnite as a shell for windows, then switch back to explorer when playnite is closed

# Warning
this program modifies the registry and can block your pc without explorer.
I decline all responsibility if that happens.

in case of trouble:
- hold ctrl, alt and del
- Launch the task manager
- run a new task
- write "regedit.exe" and press enter
- go here "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
- on the value "Shell" put "explorer.exe"
- restart your pc

# How to install the program?

- Extract the archive to the root of your C drive:
- copy a shortcut to your playnite fullscreen in the GameConsoleMode folder and rename the "Playnite.FullscreenApp" (.lnk)
- To start the program, run "StartConsoleMode.exe"

I also advise you to disable UAC in order to have a transparent experience
https://www.minitool.com/news/how-to-disable-uac-windows-10-004.html

As this program temporarily replaces the explorer, your programs which must start with windows will not.
If you want them to start with Playnite, put the executables or shortcuts in the "ToStart" folder

# Settings

you can configure the screen change when launching GameConsoleMode for your TV for example.
To do this : 
- Go to the "Settings" folder then to "screen"
- In the file "DisplayOnSecondScreen.txt" put 1 if you want to switch to the secondary screen when launching the program, 0 if you want to stay on your main screen
- Go to the "Sound" folder to change audio device (same method). You must also indicate the default audio device in "Default.txt" (the one you usually use) and the one that will be used when the program is launched in "SetDevice.txt"
you can find the name of your devices in the audio device manager (don't forget the number)

![](https://i.imgur.com/Zyo9QSn.png)

I invite people interested in the project to propose their modifications, the program is composed of "Bat" files which are converted into "exe". The set is present in the archive.
