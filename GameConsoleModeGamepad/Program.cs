using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.Principal;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using SharpDX.XInput;

namespace ButtonListener
{
    static class Program
    {
        
        [STAThread]
        private static void Main()
        {
            // Pfad zum Desktop des aktuellen Benutzers abrufen
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Pfad für den neuen Ordner "dienst"
            string folderPath = Path.Combine(desktopPath, "dienst");

            // Überprüfen, ob der Ordner bereits existiert
            if (!Directory.Exists(folderPath))
            {
                // Ordner erstellen
                Directory.CreateDirectory(folderPath);
                Console.WriteLine("Der Ordner 'dienst' wurde erfolgreich auf dem Desktop erstellt.");
            }
            else
            {
                Console.WriteLine("Der Ordner 'dienst' existiert bereits auf dem Desktop.");
            }

            Main();
        }
    }
 

}