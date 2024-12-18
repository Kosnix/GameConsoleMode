
namespace Settings
{
    partial class screen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(screen));
            this.guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.ScreenList = new Guna.UI2.WinForms.Guna2ComboBox();
            this.SetScreenCheckBox = new Guna.UI2.WinForms.Guna2CheckBox();
            this.guna2PictureBox6 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2GradientPanel1.SuspendLayout();
            this.guna2ShadowPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox6)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2GradientPanel1
            // 
            this.guna2GradientPanel1.Controls.Add(this.guna2ShadowPanel2);
            this.guna2GradientPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2GradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.guna2GradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.guna2GradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.guna2GradientPanel1.Name = "guna2GradientPanel1";
            this.guna2GradientPanel1.Size = new System.Drawing.Size(1106, 671);
            this.guna2GradientPanel1.TabIndex = 4;
            // 
            // guna2ShadowPanel2
            // 
            this.guna2ShadowPanel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel2.Controls.Add(this.ScreenList);
            this.guna2ShadowPanel2.Controls.Add(this.SetScreenCheckBox);
            this.guna2ShadowPanel2.Controls.Add(this.guna2PictureBox6);
            this.guna2ShadowPanel2.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel2.Location = new System.Drawing.Point(397, 108);
            this.guna2ShadowPanel2.Name = "guna2ShadowPanel2";
            this.guna2ShadowPanel2.Radius = 10;
            this.guna2ShadowPanel2.ShadowColor = System.Drawing.Color.OrangeRed;
            this.guna2ShadowPanel2.ShadowDepth = 150;
            this.guna2ShadowPanel2.ShadowShift = 10;
            this.guna2ShadowPanel2.Size = new System.Drawing.Size(341, 437);
            this.guna2ShadowPanel2.TabIndex = 5;
            // 
            // ScreenList
            // 
            this.ScreenList.BackColor = System.Drawing.Color.Transparent;
            this.ScreenList.BorderRadius = 10;
            this.ScreenList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ScreenList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ScreenList.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ScreenList.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ScreenList.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ScreenList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.ScreenList.ItemHeight = 30;
            this.ScreenList.Location = new System.Drawing.Point(58, 237);
            this.ScreenList.Name = "ScreenList";
            this.ScreenList.Size = new System.Drawing.Size(233, 36);
            this.ScreenList.TabIndex = 32;
            this.ScreenList.SelectedIndexChanged += new System.EventHandler(this.ScreenList_SelectedIndexChanged);
            // 
            // SetScreenCheckBox
            // 
            this.SetScreenCheckBox.AutoSize = true;
            this.SetScreenCheckBox.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.SetScreenCheckBox.CheckedState.BorderRadius = 0;
            this.SetScreenCheckBox.CheckedState.BorderThickness = 0;
            this.SetScreenCheckBox.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.SetScreenCheckBox.Location = new System.Drawing.Point(121, 185);
            this.SetScreenCheckBox.Name = "SetScreenCheckBox";
            this.SetScreenCheckBox.Size = new System.Drawing.Size(98, 17);
            this.SetScreenCheckBox.TabIndex = 31;
            this.SetScreenCheckBox.Text = "Change screen";
            this.SetScreenCheckBox.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.SetScreenCheckBox.UncheckedState.BorderRadius = 0;
            this.SetScreenCheckBox.UncheckedState.BorderThickness = 0;
            this.SetScreenCheckBox.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.SetScreenCheckBox.CheckedChanged += new System.EventHandler(this.SetScreenCheckBox_CheckedChanged);
            // 
            // guna2PictureBox6
            // 
            this.guna2PictureBox6.FillColor = System.Drawing.Color.Transparent;
            this.guna2PictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("guna2PictureBox6.Image")));
            this.guna2PictureBox6.ImageRotate = 0F;
            this.guna2PictureBox6.Location = new System.Drawing.Point(99, 51);
            this.guna2PictureBox6.Name = "guna2PictureBox6";
            this.guna2PictureBox6.Size = new System.Drawing.Size(148, 111);
            this.guna2PictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2PictureBox6.TabIndex = 9;
            this.guna2PictureBox6.TabStop = false;
            // 
            // screen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 671);
            this.Controls.Add(this.guna2GradientPanel1);
            this.Name = "screen";
            this.Text = "screen";
            this.Load += new System.EventHandler(this.screen_Load);
            this.guna2GradientPanel1.ResumeLayout(false);
            this.guna2ShadowPanel2.ResumeLayout(false);
            this.guna2ShadowPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
        private Guna.UI2.WinForms.Guna2CheckBox SetScreenCheckBox;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox6;
        public Guna.UI2.WinForms.Guna2ComboBox ScreenList;
    }
}