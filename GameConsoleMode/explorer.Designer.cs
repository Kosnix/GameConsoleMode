
namespace GameConsoleMode
{
    partial class explorer
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel_open_apps = new System.Windows.Forms.FlowLayoutPanel();
            this.timer_open_apps = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.55937F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.88126F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.55937F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel_open_apps, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(3818, 1421);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Visible = false;
            // 
            // flowLayoutPanel_open_apps
            // 
            this.flowLayoutPanel_open_apps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_open_apps.Location = new System.Drawing.Point(940, 476);
            this.flowLayoutPanel_open_apps.Name = "flowLayoutPanel_open_apps";
            this.flowLayoutPanel_open_apps.Size = new System.Drawing.Size(1936, 467);
            this.flowLayoutPanel_open_apps.TabIndex = 0;
            this.flowLayoutPanel_open_apps.Visible = false;
            // 
            // timer_open_apps
            // 
            this.timer_open_apps.Enabled = true;
            this.timer_open_apps.Interval = 30000;
            this.timer_open_apps.Tick += new System.EventHandler(this.timer_open_apps_Tick);
            // 
            // explorer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(3818, 1421);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "explorer";
            this.Text = "explorer";
            this.Load += new System.EventHandler(this.explorer_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Timer timer_open_apps;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_open_apps;
    }
}