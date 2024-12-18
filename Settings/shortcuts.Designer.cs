
namespace Settings
{
    partial class shortcuts
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
            this.components = new System.ComponentModel.Container();
            Guna.UI2.AnimatorNS.Animation animation3 = new Guna.UI2.AnimatorNS.Animation();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(shortcuts));
            this.shortcut_list = new System.Windows.Forms.ImageList(this.components);
            this.guna2Transition1 = new Guna.UI2.WinForms.Guna2Transition();
            this.guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.label_shortcut_information = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.picture_controller_layout = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.label_shortcut_overview = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2GradientPanel2 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button_switch_window = new Guna.UI2.WinForms.Guna2ImageButton();
            this.button_start_gcm = new Guna.UI2.WinForms.Guna2ImageButton();
            this.guna2ShadowPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_controller_layout)).BeginInit();
            this.guna2GradientPanel1.SuspendLayout();
            this.guna2GradientPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // shortcut_list
            // 
            this.shortcut_list.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.shortcut_list.ImageSize = new System.Drawing.Size(16, 16);
            this.shortcut_list.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // guna2Transition1
            // 
            this.guna2Transition1.Cursor = null;
            animation3.AnimateOnlyDifferences = true;
            animation3.BlindCoeff = ((System.Drawing.PointF)(resources.GetObject("animation3.BlindCoeff")));
            animation3.LeafCoeff = 0F;
            animation3.MaxTime = 1F;
            animation3.MinTime = 0F;
            animation3.MosaicCoeff = ((System.Drawing.PointF)(resources.GetObject("animation3.MosaicCoeff")));
            animation3.MosaicShift = ((System.Drawing.PointF)(resources.GetObject("animation3.MosaicShift")));
            animation3.MosaicSize = 0;
            animation3.Padding = new System.Windows.Forms.Padding(0);
            animation3.RotateCoeff = 0F;
            animation3.RotateLimit = 0F;
            animation3.ScaleCoeff = ((System.Drawing.PointF)(resources.GetObject("animation3.ScaleCoeff")));
            animation3.SlideCoeff = ((System.Drawing.PointF)(resources.GetObject("animation3.SlideCoeff")));
            animation3.TimeCoeff = 0F;
            animation3.TransparencyCoeff = 0F;
            this.guna2Transition1.DefaultAnimation = animation3;
            // 
            // guna2ShadowPanel1
            // 
            this.guna2ShadowPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ShadowPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel1.Controls.Add(this.label_shortcut_information);
            this.guna2ShadowPanel1.Controls.Add(this.picture_controller_layout);
            this.guna2Transition1.SetDecoration(this.guna2ShadowPanel1, Guna.UI2.AnimatorNS.DecorationType.None);
            this.guna2ShadowPanel1.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel1.ForeColor = System.Drawing.Color.Coral;
            this.guna2ShadowPanel1.Location = new System.Drawing.Point(228, 106);
            this.guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            this.guna2ShadowPanel1.Radius = 10;
            this.guna2ShadowPanel1.ShadowColor = System.Drawing.SystemColors.ControlText;
            this.guna2ShadowPanel1.ShadowShift = 50;
            this.guna2ShadowPanel1.Size = new System.Drawing.Size(853, 553);
            this.guna2ShadowPanel1.TabIndex = 0;
            // 
            // label_shortcut_information
            // 
            this.label_shortcut_information.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_shortcut_information.AutoSize = false;
            this.label_shortcut_information.BackColor = System.Drawing.Color.Transparent;
            this.guna2Transition1.SetDecoration(this.label_shortcut_information, Guna.UI2.AnimatorNS.DecorationType.None);
            this.label_shortcut_information.Font = new System.Drawing.Font("Nirmala UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_shortcut_information.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label_shortcut_information.Location = new System.Drawing.Point(207, 77);
            this.label_shortcut_information.Name = "label_shortcut_information";
            this.label_shortcut_information.Size = new System.Drawing.Size(439, 92);
            this.label_shortcut_information.TabIndex = 2;
            this.label_shortcut_information.Text = null;
            this.label_shortcut_information.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picture_controller_layout
            // 
            this.guna2Transition1.SetDecoration(this.picture_controller_layout, Guna.UI2.AnimatorNS.DecorationType.None);
            this.picture_controller_layout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.picture_controller_layout.Image = ((System.Drawing.Image)(resources.GetObject("picture_controller_layout.Image")));
            this.picture_controller_layout.ImageRotate = 0F;
            this.picture_controller_layout.Location = new System.Drawing.Point(0, 208);
            this.picture_controller_layout.Name = "picture_controller_layout";
            this.picture_controller_layout.Size = new System.Drawing.Size(853, 345);
            this.picture_controller_layout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picture_controller_layout.TabIndex = 0;
            this.picture_controller_layout.TabStop = false;
            // 
            // guna2GradientPanel1
            // 
            this.guna2GradientPanel1.Controls.Add(this.label_shortcut_overview);
            this.guna2GradientPanel1.Controls.Add(this.guna2GradientPanel2);
            this.guna2GradientPanel1.Controls.Add(this.guna2ShadowPanel1);
            this.guna2Transition1.SetDecoration(this.guna2GradientPanel1, Guna.UI2.AnimatorNS.DecorationType.None);
            this.guna2GradientPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2GradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.guna2GradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.guna2GradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.guna2GradientPanel1.Name = "guna2GradientPanel1";
            this.guna2GradientPanel1.Size = new System.Drawing.Size(1106, 671);
            this.guna2GradientPanel1.TabIndex = 2;
            // 
            // label_shortcut_overview
            // 
            this.label_shortcut_overview.BackColor = System.Drawing.Color.Transparent;
            this.guna2Transition1.SetDecoration(this.label_shortcut_overview, Guna.UI2.AnimatorNS.DecorationType.None);
            this.label_shortcut_overview.Font = new System.Drawing.Font("Nirmala UI", 25.75F);
            this.label_shortcut_overview.ForeColor = System.Drawing.Color.White;
            this.label_shortcut_overview.Location = new System.Drawing.Point(472, 40);
            this.label_shortcut_overview.Name = "label_shortcut_overview";
            this.label_shortcut_overview.Size = new System.Drawing.Size(3, 2);
            this.label_shortcut_overview.TabIndex = 3;
            this.label_shortcut_overview.Text = null;
            this.label_shortcut_overview.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // guna2GradientPanel2
            // 
            this.guna2GradientPanel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2GradientPanel2.BorderRadius = 10;
            this.guna2GradientPanel2.Controls.Add(this.flowLayoutPanel1);
            this.guna2GradientPanel2.CustomizableEdges.BottomLeft = false;
            this.guna2GradientPanel2.CustomizableEdges.TopLeft = false;
            this.guna2Transition1.SetDecoration(this.guna2GradientPanel2, Guna.UI2.AnimatorNS.DecorationType.None);
            this.guna2GradientPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.guna2GradientPanel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(40)))), ((int)(((byte)(56)))));
            this.guna2GradientPanel2.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.guna2GradientPanel2.Location = new System.Drawing.Point(0, 0);
            this.guna2GradientPanel2.Name = "guna2GradientPanel2";
            this.guna2GradientPanel2.Size = new System.Drawing.Size(200, 671);
            this.guna2GradientPanel2.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button_switch_window);
            this.flowLayoutPanel1.Controls.Add(this.button_start_gcm);
            this.guna2Transition1.SetDecoration(this.flowLayoutPanel1, Guna.UI2.AnimatorNS.DecorationType.None);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 671);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // button_switch_window
            // 
            this.button_switch_window.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button_switch_window.BackColor = System.Drawing.Color.Transparent;
            this.button_switch_window.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.guna2Transition1.SetDecoration(this.button_switch_window, Guna.UI2.AnimatorNS.DecorationType.None);
            this.button_switch_window.HoverState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.button_switch_window.HoverState.ImageSize = new System.Drawing.Size(40, 40);
            this.button_switch_window.Image = ((System.Drawing.Image)(resources.GetObject("button_switch_window.Image")));
            this.button_switch_window.ImageOffset = new System.Drawing.Point(0, 0);
            this.button_switch_window.ImageRotate = 0F;
            this.button_switch_window.ImageSize = new System.Drawing.Size(100, 75);
            this.button_switch_window.Location = new System.Drawing.Point(3, 3);
            this.button_switch_window.Name = "button_switch_window";
            this.button_switch_window.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.button_switch_window.PressedState.ImageSize = new System.Drawing.Size(40, 40);
            this.button_switch_window.Size = new System.Drawing.Size(194, 97);
            this.button_switch_window.TabIndex = 0;
            this.button_switch_window.UseTransparentBackground = true;
            this.button_switch_window.Click += new System.EventHandler(this.button_switch_window_Click);
            // 
            // button_start_gcm
            // 
            this.button_start_gcm.BackColor = System.Drawing.Color.Transparent;
            this.button_start_gcm.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.guna2Transition1.SetDecoration(this.button_start_gcm, Guna.UI2.AnimatorNS.DecorationType.None);
            this.button_start_gcm.HoverState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image2")));
            this.button_start_gcm.HoverState.ImageSize = new System.Drawing.Size(40, 40);
            this.button_start_gcm.Image = ((System.Drawing.Image)(resources.GetObject("button_start_gcm.Image")));
            this.button_start_gcm.ImageOffset = new System.Drawing.Point(0, 0);
            this.button_start_gcm.ImageRotate = 0F;
            this.button_start_gcm.ImageSize = new System.Drawing.Size(100, 75);
            this.button_start_gcm.IndicateFocus = true;
            this.button_start_gcm.Location = new System.Drawing.Point(3, 106);
            this.button_start_gcm.Name = "button_start_gcm";
            this.button_start_gcm.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image3")));
            this.button_start_gcm.PressedState.ImageSize = new System.Drawing.Size(40, 40);
            this.button_start_gcm.Size = new System.Drawing.Size(194, 97);
            this.button_start_gcm.TabIndex = 1;
            this.button_start_gcm.UseTransparentBackground = true;
            this.button_start_gcm.Click += new System.EventHandler(this.button_start_gcm_Click);
            // 
            // shortcuts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 671);
            this.Controls.Add(this.guna2GradientPanel1);
            this.guna2Transition1.SetDecoration(this, Guna.UI2.AnimatorNS.DecorationType.None);
            this.Name = "shortcuts";
            this.Text = "shortcut";
            this.Load += new System.EventHandler(this.shortcut_Load);
            this.guna2ShadowPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picture_controller_layout)).EndInit();
            this.guna2GradientPanel1.ResumeLayout(false);
            this.guna2GradientPanel1.PerformLayout();
            this.guna2GradientPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList shortcut_list;
        private Guna.UI2.WinForms.Guna2Transition guna2Transition1;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel label_shortcut_information;
        private Guna.UI2.WinForms.Guna2PictureBox picture_controller_layout;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel label_shortcut_overview;
        private Guna.UI2.WinForms.Guna2ImageButton button_start_gcm;
        private Guna.UI2.WinForms.Guna2ImageButton button_switch_window;
    }
}