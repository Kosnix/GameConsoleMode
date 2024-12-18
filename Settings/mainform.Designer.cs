namespace Settings
{
    partial class Settings
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.start_gamemode = new Guna.UI2.WinForms.Guna2Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.launcher = new System.Windows.Forms.TabPage();
            this.display = new System.Windows.Forms.TabPage();
            this.guna2GradientPanel8 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.button_shortcuts = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button4 = new Guna.UI2.WinForms.Guna2Button();
            this.button_startup = new Guna.UI2.WinForms.Guna2Button();
            this.button_additional = new Guna.UI2.WinForms.Guna2Button();
            this.button_launcher = new Guna.UI2.WinForms.Guna2Button();
            this.guna2AnimateWindow1 = new Guna.UI2.WinForms.Guna2AnimateWindow(this.components);
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.panel_main = new Guna.UI2.WinForms.Guna2Panel();
            this.picture_first = new System.Windows.Forms.PictureBox();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.button_minimize = new Guna.UI2.WinForms.Guna2Button();
            this.button_cancel = new Guna.UI2.WinForms.Guna2Button();
            this.guna2GradientPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.display.SuspendLayout();
            this.guna2GradientPanel8.SuspendLayout();
            this.panel_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_first)).BeginInit();
            this.guna2Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(21, 32);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(246, 192);
            this.flowLayoutPanel2.TabIndex = 29;
            // 
            // guna2GradientPanel1
            // 
            this.guna2GradientPanel1.Controls.Add(this.start_gamemode);
            this.guna2GradientPanel1.Controls.Add(this.tabControl1);
            this.guna2GradientPanel1.Controls.Add(this.button_shortcuts);
            this.guna2GradientPanel1.Controls.Add(this.guna2Button4);
            this.guna2GradientPanel1.Controls.Add(this.button_startup);
            this.guna2GradientPanel1.Controls.Add(this.button_additional);
            this.guna2GradientPanel1.Controls.Add(this.button_launcher);
            this.guna2GradientPanel1.CustomizableEdges.BottomLeft = false;
            this.guna2GradientPanel1.CustomizableEdges.TopLeft = false;
            this.guna2GradientPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.guna2GradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.guna2GradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.guna2GradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.guna2GradientPanel1.Name = "guna2GradientPanel1";
            this.guna2GradientPanel1.Size = new System.Drawing.Size(238, 750);
            this.guna2GradientPanel1.TabIndex = 1;
            // 
            // start_gamemode
            // 
            this.start_gamemode.Animated = true;
            this.start_gamemode.BackColor = System.Drawing.Color.Transparent;
            this.start_gamemode.BorderColor = System.Drawing.Color.White;
            this.start_gamemode.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.start_gamemode.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.start_gamemode.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.start_gamemode.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.start_gamemode.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.start_gamemode.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.start_gamemode.FocusedColor = System.Drawing.Color.Silver;
            this.start_gamemode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.start_gamemode.ForeColor = System.Drawing.Color.White;
            this.start_gamemode.Image = ((System.Drawing.Image)(resources.GetObject("start_gamemode.Image")));
            this.start_gamemode.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.start_gamemode.ImageSize = new System.Drawing.Size(50, 50);
            this.start_gamemode.IndicateFocus = true;
            this.start_gamemode.Location = new System.Drawing.Point(0, 680);
            this.start_gamemode.Name = "start_gamemode";
            this.start_gamemode.Size = new System.Drawing.Size(238, 70);
            this.start_gamemode.TabIndex = 28;
            this.start_gamemode.Text = "START GAMEMODE";
            this.start_gamemode.Click += new System.EventHandler(this.start_gamemode_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.launcher);
            this.tabControl1.Controls.Add(this.display);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.tabControl1.Location = new System.Drawing.Point(12, 356);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(947, 630);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 5;
            this.tabControl1.Visible = false;
            // 
            // launcher
            // 
            this.launcher.Location = new System.Drawing.Point(4, 22);
            this.launcher.Name = "launcher";
            this.launcher.Padding = new System.Windows.Forms.Padding(3);
            this.launcher.Size = new System.Drawing.Size(939, 604);
            this.launcher.TabIndex = 0;
            this.launcher.Text = "LAUNCHER";
            this.launcher.UseVisualStyleBackColor = true;
            // 
            // display
            // 
            this.display.Controls.Add(this.guna2GradientPanel8);
            this.display.Location = new System.Drawing.Point(4, 22);
            this.display.Name = "display";
            this.display.Padding = new System.Windows.Forms.Padding(3);
            this.display.Size = new System.Drawing.Size(939, 604);
            this.display.TabIndex = 1;
            this.display.Text = "DISPLAY";
            this.display.UseVisualStyleBackColor = true;
            // 
            // guna2GradientPanel8
            // 
            this.guna2GradientPanel8.Controls.Add(this.flowLayoutPanel2);
            this.guna2GradientPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2GradientPanel8.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.guna2GradientPanel8.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.guna2GradientPanel8.Location = new System.Drawing.Point(3, 3);
            this.guna2GradientPanel8.Name = "guna2GradientPanel8";
            this.guna2GradientPanel8.Size = new System.Drawing.Size(933, 598);
            this.guna2GradientPanel8.TabIndex = 1;
            // 
            // button_shortcuts
            // 
            this.button_shortcuts.Animated = true;
            this.button_shortcuts.BackColor = System.Drawing.Color.Transparent;
            this.button_shortcuts.BorderColor = System.Drawing.Color.White;
            this.button_shortcuts.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_shortcuts.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.button_shortcuts.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.button_shortcuts.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.button_shortcuts.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.button_shortcuts.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_shortcuts.FillColor = System.Drawing.Color.Empty;
            this.button_shortcuts.FocusedColor = System.Drawing.Color.Silver;
            this.button_shortcuts.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_shortcuts.ForeColor = System.Drawing.Color.White;
            this.button_shortcuts.Image = ((System.Drawing.Image)(resources.GetObject("button_shortcuts.Image")));
            this.button_shortcuts.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.button_shortcuts.ImageSize = new System.Drawing.Size(50, 50);
            this.button_shortcuts.IndicateFocus = true;
            this.button_shortcuts.Location = new System.Drawing.Point(0, 280);
            this.button_shortcuts.Name = "button_shortcuts";
            this.button_shortcuts.Size = new System.Drawing.Size(238, 70);
            this.button_shortcuts.TabIndex = 27;
            this.button_shortcuts.Text = "SHORTCUTS";
            this.button_shortcuts.Click += new System.EventHandler(this.button_shortcuts_Click);
            // 
            // guna2Button4
            // 
            this.guna2Button4.Animated = true;
            this.guna2Button4.BackColor = System.Drawing.Color.Transparent;
            this.guna2Button4.BorderColor = System.Drawing.Color.White;
            this.guna2Button4.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button4.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button4.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button4.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button4.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Button4.FillColor = System.Drawing.Color.Empty;
            this.guna2Button4.FocusedColor = System.Drawing.Color.Silver;
            this.guna2Button4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button4.ForeColor = System.Drawing.Color.White;
            this.guna2Button4.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button4.Image")));
            this.guna2Button4.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button4.ImageSize = new System.Drawing.Size(50, 50);
            this.guna2Button4.IndicateFocus = true;
            this.guna2Button4.Location = new System.Drawing.Point(0, 210);
            this.guna2Button4.Name = "guna2Button4";
            this.guna2Button4.Size = new System.Drawing.Size(238, 70);
            this.guna2Button4.TabIndex = 26;
            this.guna2Button4.Text = "SCREEN";
            this.guna2Button4.Click += new System.EventHandler(this.guna2Button4_Click);
            // 
            // button_startup
            // 
            this.button_startup.Animated = true;
            this.button_startup.BackColor = System.Drawing.Color.Transparent;
            this.button_startup.BorderColor = System.Drawing.Color.White;
            this.button_startup.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.button_startup.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.button_startup.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.button_startup.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.button_startup.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_startup.FillColor = System.Drawing.Color.Empty;
            this.button_startup.FocusedColor = System.Drawing.Color.Silver;
            this.button_startup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_startup.ForeColor = System.Drawing.Color.White;
            this.button_startup.Image = ((System.Drawing.Image)(resources.GetObject("button_startup.Image")));
            this.button_startup.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.button_startup.ImageSize = new System.Drawing.Size(50, 50);
            this.button_startup.IndicateFocus = true;
            this.button_startup.Location = new System.Drawing.Point(0, 140);
            this.button_startup.Name = "button_startup";
            this.button_startup.Size = new System.Drawing.Size(238, 70);
            this.button_startup.TabIndex = 25;
            this.button_startup.Text = "STARTUP";
            this.button_startup.Click += new System.EventHandler(this.button_startup_Click);
            // 
            // button_additional
            // 
            this.button_additional.Animated = true;
            this.button_additional.BackColor = System.Drawing.Color.Transparent;
            this.button_additional.BorderColor = System.Drawing.Color.White;
            this.button_additional.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.button_additional.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.button_additional.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.button_additional.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.button_additional.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_additional.FillColor = System.Drawing.Color.Empty;
            this.button_additional.FocusedColor = System.Drawing.Color.Silver;
            this.button_additional.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_additional.ForeColor = System.Drawing.Color.White;
            this.button_additional.Image = ((System.Drawing.Image)(resources.GetObject("button_additional.Image")));
            this.button_additional.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.button_additional.ImageSize = new System.Drawing.Size(50, 50);
            this.button_additional.IndicateFocus = true;
            this.button_additional.Location = new System.Drawing.Point(0, 70);
            this.button_additional.Name = "button_additional";
            this.button_additional.Size = new System.Drawing.Size(238, 70);
            this.button_additional.TabIndex = 24;
            this.button_additional.Text = "ADDITIONAL";
            this.button_additional.Click += new System.EventHandler(this.button_additional_Click);
            // 
            // button_launcher
            // 
            this.button_launcher.Animated = true;
            this.button_launcher.BackColor = System.Drawing.Color.Transparent;
            this.button_launcher.BorderColor = System.Drawing.Color.White;
            this.button_launcher.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.button_launcher.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.button_launcher.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.button_launcher.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.button_launcher.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_launcher.FillColor = System.Drawing.Color.Empty;
            this.button_launcher.FocusedColor = System.Drawing.Color.White;
            this.button_launcher.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_launcher.ForeColor = System.Drawing.Color.White;
            this.button_launcher.Image = ((System.Drawing.Image)(resources.GetObject("button_launcher.Image")));
            this.button_launcher.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.button_launcher.ImageSize = new System.Drawing.Size(50, 50);
            this.button_launcher.Location = new System.Drawing.Point(0, 0);
            this.button_launcher.Name = "button_launcher";
            this.button_launcher.Size = new System.Drawing.Size(238, 70);
            this.button_launcher.TabIndex = 23;
            this.button_launcher.Text = "LAUNCHER";
            this.button_launcher.Click += new System.EventHandler(this.button_launcher_Click);
            // 
            // guna2AnimateWindow1
            // 
            this.guna2AnimateWindow1.AnimationType = Guna.UI2.WinForms.Guna2AnimateWindow.AnimateWindowType.AW_BLEND;
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.AnimateWindow = true;
            this.guna2BorderlessForm1.AnimationType = Guna.UI2.WinForms.Guna2BorderlessForm.AnimateWindowType.AW_HOR_POSITIVE;
            this.guna2BorderlessForm1.BorderRadius = 25;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.ResizeForm = false;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // panel_main
            // 
            this.panel_main.Controls.Add(this.picture_first);
            this.panel_main.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_main.Location = new System.Drawing.Point(238, 33);
            this.panel_main.Name = "panel_main";
            this.panel_main.Size = new System.Drawing.Size(1228, 717);
            this.panel_main.TabIndex = 6;
            // 
            // picture_first
            // 
            this.picture_first.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picture_first.Image = ((System.Drawing.Image)(resources.GetObject("picture_first.Image")));
            this.picture_first.Location = new System.Drawing.Point(0, 0);
            this.picture_first.Name = "picture_first";
            this.picture_first.Size = new System.Drawing.Size(1228, 717);
            this.picture_first.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picture_first.TabIndex = 0;
            this.picture_first.TabStop = false;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.Controls.Add(this.button_minimize);
            this.guna2Panel1.Controls.Add(this.button_cancel);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.Location = new System.Drawing.Point(238, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(1228, 27);
            this.guna2Panel1.TabIndex = 7;
            // 
            // button_minimize
            // 
            this.button_minimize.Animated = true;
            this.button_minimize.BackColor = System.Drawing.Color.Transparent;
            this.button_minimize.BorderColor = System.Drawing.Color.White;
            this.button_minimize.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.button_minimize.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.button_minimize.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.button_minimize.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.button_minimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_minimize.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.button_minimize.FocusedColor = System.Drawing.Color.Silver;
            this.button_minimize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_minimize.ForeColor = System.Drawing.Color.White;
            this.button_minimize.Image = ((System.Drawing.Image)(resources.GetObject("button_minimize.Image")));
            this.button_minimize.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.button_minimize.ImageSize = new System.Drawing.Size(28, 28);
            this.button_minimize.IndicateFocus = true;
            this.button_minimize.Location = new System.Drawing.Point(1134, 0);
            this.button_minimize.Name = "button_minimize";
            this.button_minimize.Size = new System.Drawing.Size(47, 27);
            this.button_minimize.TabIndex = 27;
            this.button_minimize.Click += new System.EventHandler(this.button_minimize_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Animated = true;
            this.button_cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_cancel.BorderColor = System.Drawing.Color.White;
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.button_cancel.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.button_cancel.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.button_cancel.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.button_cancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_cancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.button_cancel.FocusedColor = System.Drawing.Color.Silver;
            this.button_cancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_cancel.ForeColor = System.Drawing.Color.White;
            this.button_cancel.Image = ((System.Drawing.Image)(resources.GetObject("button_cancel.Image")));
            this.button_cancel.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.button_cancel.ImageSize = new System.Drawing.Size(28, 28);
            this.button_cancel.IndicateFocus = true;
            this.button_cancel.Location = new System.Drawing.Point(1181, 0);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(47, 27);
            this.button_cancel.TabIndex = 26;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(1466, 750);
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.panel_main);
            this.Controls.Add(this.guna2GradientPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load_1);
            this.guna2GradientPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.display.ResumeLayout(false);
            this.guna2GradientPanel8.ResumeLayout(false);
            this.panel_main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picture_first)).EndInit();
            this.guna2Panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage launcher;
        private System.Windows.Forms.TabPage display;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel8;
        private Guna.UI2.WinForms.Guna2Button button_launcher;
        private Guna.UI2.WinForms.Guna2Button guna2Button4;
        private Guna.UI2.WinForms.Guna2Button button_startup;
        private Guna.UI2.WinForms.Guna2Button button_additional;
        private Guna.UI2.WinForms.Guna2Button button_shortcuts;
        private Guna.UI2.WinForms.Guna2Button start_gamemode;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow1;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2Panel panel_main;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Button button_cancel;
        private Guna.UI2.WinForms.Guna2Button button_minimize;
        private System.Windows.Forms.PictureBox picture_first;
    }
}
