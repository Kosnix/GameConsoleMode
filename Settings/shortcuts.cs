using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using IO = System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Settings
{
    public partial class shortcuts : Form
    {
        public shortcuts()
        {
          
                InitializeComponent(); // standard initialization
               
        }

        private void shortcuts_Load(object sender, EventArgs e)
        {
            string joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");

            if (File.Exists(joyxoffExePath))
            {
                MessageBox.Show("Joyxoff is already installed and will now start.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start(joyxoffExePath);
                return;
            }
            else
            {
                // Zeigt die MessageBox mit einer Yes/No-Auswahl an
                DialogResult result = MessageBox.Show(
                    "It was detected that you do not have JoyXoff installed. Would you like to install it now and configure it with GCM?",
                    "JoyXoff Installation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                // Überprüft die Antwort des Benutzers
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Check if Joyxoff is already installed
                       
                        if (File.Exists(joyxoffExePath))
                        {
                            MessageBox.Show("Joyxoff is already installed and will now start.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start(joyxoffExePath);
                            return;
                        }

                        // Get current directory
                        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                        // Path to the MSI file
                        string msiPath = Path.Combine(currentDirectory, "Joyxoff.msi");

                        // Check if the MSI file exists
                        if (!File.Exists(msiPath))
                        {
                            MessageBox.Show($"The file {msiPath} was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Start process to install the MSI file with the passive parameter
                        Process process = new Process();
                        process.StartInfo.FileName = "msiexec";
                        process.StartInfo.Arguments = $"/i \"{msiPath}\" /passive";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;

                        // Start process
                        process.Start();
                        process.WaitForExit();

                        // Check if the installation was successful
                        if (process.ExitCode == 0)
                        {
                            MessageBox.Show("Installation completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Check if Joyxoff is already installed
                            joyxoffExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Joyxoff", "Joyxoff.exe");
                            if (File.Exists(joyxoffExePath))
                            {
                                Process.Start(joyxoffExePath);
                                return;
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show($"Installation failed. Error code: {process.ExitCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (result == DialogResult.No)
                {

                }
            }
        }
    }
}
