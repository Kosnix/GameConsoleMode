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

namespace Settings
{
    public partial class shortcuts : Form
    {
        public shortcuts()
        {
          
                InitializeComponent(); // Standard-Initialisierung
          
        }

        private void LoadPictureFromFile(string relativePath,string text,string maintext)
        {
            // Determine the path based on the project directory
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDir, relativePath);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("The image was not found: " + fullPath);
                return;
            }

            Image image = Image.FromFile(fullPath);

            // Draw Image to Panel
            picture_controller_layout.Image = image;
            picture_controller_layout.BackgroundImageLayout = ImageLayout.Stretch; // or other layouts like Center, etc.

            label_shortcut_information.Text = text;
            label_shortcut_overview.Text = maintext;
        }


        private void shortcut_Load(object sender, EventArgs e)
        {
         
        }

        private void button_switch_window_Click(object sender, EventArgs e)
        {
            LoadPictureFromFile("Resources/switchwin.png", "Switches through the windows, <br>very useful when your launcher window disappears <br> in the background", "SWITCH THROUGH WINDOWS (ALT-TAB)");
        }

        private void button_start_gcm_Click(object sender, EventArgs e)
        {
            LoadPictureFromFile("Resources/start_gcm.png", "This allows you to start gaming mode <br> with your controller alone", "START GAMING CONSOLE MODE");
        }

    }
}
