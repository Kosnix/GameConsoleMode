using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

class Program
{
    

    static void Main()
    {
        StartupControl.RestoreStartupApps();
    }

  

}

public static class StartupControl
{
    private const string HKCU_Run = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string HKLM_Run = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string HKCU_Backup = @"Software\gcm\BackupStartup\HKCU";
    private const string HKLM_Backup = @"Software\gcm\BackupStartup\HKLM";
    private static readonly string StartupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    private static readonly string StartupBackupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gcm\\StartupBackup");

    public static void DisableAllStartupApps()
    {
        DisableRegistryStartup(Registry.CurrentUser, HKCU_Run, HKCU_Backup);
        DisableRegistryStartup(Registry.LocalMachine, HKLM_Run, HKLM_Backup);
        BackupAndClearStartupFolder();
    }

    public static void RestoreStartupApps()
    {
        RestoreRegistryStartup(Registry.CurrentUser, HKCU_Run, HKCU_Backup);
        RestoreRegistryStartup(Registry.LocalMachine, HKLM_Run, HKLM_Backup);
        RestoreStartupFolder();
    }

    private static void DisableRegistryStartup(RegistryKey root, string runPath, string backupPath)
    {
        using RegistryKey runKey = root.OpenSubKey(runPath, writable: true);
        using RegistryKey backupKey = root.CreateSubKey(backupPath);
        if (runKey == null || backupKey == null) return;

        foreach (string valueName in runKey.GetValueNames())
        {
            object value = runKey.GetValue(valueName);
            RegistryValueKind kind = runKey.GetValueKind(valueName);
            backupKey.SetValue(valueName, value, kind);
            runKey.DeleteValue(valueName);
        }
    }

    private static void RestoreRegistryStartup(RegistryKey root, string runPath, string backupPath)
    {
        using RegistryKey runKey = root.OpenSubKey(runPath, writable: true);
        using RegistryKey backupKey = root.OpenSubKey(backupPath, writable: true);
        if (runKey == null || backupKey == null) return;

        foreach (string valueName in backupKey.GetValueNames())
        {
            object value = backupKey.GetValue(valueName);
            RegistryValueKind kind = backupKey.GetValueKind(valueName);
            runKey.SetValue(valueName, value, kind);
        }
        root.DeleteSubKeyTree(backupPath);
    }

    private static void BackupAndClearStartupFolder()
    {
        if (!Directory.Exists(StartupFolder)) return;
        Directory.CreateDirectory(StartupBackupFolder);

        foreach (string file in Directory.GetFiles(StartupFolder))
        {
            string destFile = Path.Combine(StartupBackupFolder, Path.GetFileName(file));
            File.Move(file, destFile, overwrite: true);
        }
    }

    private static void RestoreStartupFolder()
    {
        if (!Directory.Exists(StartupBackupFolder)) return;
        Directory.CreateDirectory(StartupFolder);

        foreach (string file in Directory.GetFiles(StartupBackupFolder))
        {
            string destFile = Path.Combine(StartupFolder, Path.GetFileName(file));
            File.Move(file, destFile, overwrite: true);
        }

        Directory.Delete(StartupBackupFolder, recursive: true);
    }
}
