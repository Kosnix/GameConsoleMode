using System;
using System.Windows.Forms;

namespace audiodevice
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            // Optional: zeige das Fenster nicht an
            this.Visible = true;

            Console.WriteLine("[AudioDevice] App gestartet. Lausche auf Lautstärketasten...");
        }

        // Catch system keys (Volume Up, Down, Mute)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Console.WriteLine($"[Key] Detected: {keyData}");

            if (keyData == Keys.VolumeUp)
                Console.WriteLine("[Key] Volume UP erkannt!");
            else if (keyData == Keys.VolumeDown)
                Console.WriteLine("[Key] Volume DOWN erkannt!");
            else if (keyData == Keys.VolumeMute)
                Console.WriteLine("[Key] Volume MUTE erkannt!");

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
