namespace spaar.ModLoader.Installer
{
  partial class FormInstaller
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
      this.txtBesiegeLocation = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnBrowse = new System.Windows.Forms.Button();
      this.btnInstall = new System.Windows.Forms.Button();
      this.btnUninstall = new System.Windows.Forms.Button();
      this.cobVersion = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.cbDeveloper = new System.Windows.Forms.CheckBox();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.tsLblStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtBesiegeLocation
      // 
      this.txtBesiegeLocation.Location = new System.Drawing.Point(12, 29);
      this.txtBesiegeLocation.Name = "txtBesiegeLocation";
      this.txtBesiegeLocation.Size = new System.Drawing.Size(546, 20);
      this.txtBesiegeLocation.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(92, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Besiege Location:";
      // 
      // btnBrowse
      // 
      this.btnBrowse.Location = new System.Drawing.Point(564, 27);
      this.btnBrowse.Name = "btnBrowse";
      this.btnBrowse.Size = new System.Drawing.Size(88, 23);
      this.btnBrowse.TabIndex = 2;
      this.btnBrowse.Text = "Browse";
      this.btnBrowse.UseVisualStyleBackColor = true;
      this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
      // 
      // btnInstall
      // 
      this.btnInstall.Enabled = false;
      this.btnInstall.Location = new System.Drawing.Point(12, 89);
      this.btnInstall.Name = "btnInstall";
      this.btnInstall.Size = new System.Drawing.Size(317, 57);
      this.btnInstall.TabIndex = 3;
      this.btnInstall.Text = "Install Mod Loader";
      this.btnInstall.UseVisualStyleBackColor = true;
      this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // btnUninstall
      // 
      this.btnUninstall.Enabled = false;
      this.btnUninstall.Location = new System.Drawing.Point(335, 89);
      this.btnUninstall.Name = "btnUninstall";
      this.btnUninstall.Size = new System.Drawing.Size(317, 57);
      this.btnUninstall.TabIndex = 4;
      this.btnUninstall.Text = "Uninstall Mod Loader";
      this.btnUninstall.UseVisualStyleBackColor = true;
      this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
      // 
      // cobVersion
      // 
      this.cobVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cobVersion.FormattingEnabled = true;
      this.cobVersion.Location = new System.Drawing.Point(64, 55);
      this.cobVersion.Name = "cobVersion";
      this.cobVersion.Size = new System.Drawing.Size(265, 21);
      this.cobVersion.TabIndex = 5;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 58);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(45, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Version:";
      // 
      // cbDeveloper
      // 
      this.cbDeveloper.AutoSize = true;
      this.cbDeveloper.Location = new System.Drawing.Point(335, 57);
      this.cbDeveloper.Name = "cbDeveloper";
      this.cbDeveloper.Size = new System.Drawing.Size(110, 17);
      this.cbDeveloper.TabIndex = 7;
      this.cbDeveloper.Text = "Developer Edition";
      this.cbDeveloper.UseVisualStyleBackColor = true;
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblStatus});
      this.statusStrip1.Location = new System.Drawing.Point(0, 156);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(664, 22);
      this.statusStrip1.TabIndex = 8;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // tsLblStatus
      // 
      this.tsLblStatus.Name = "tsLblStatus";
      this.tsLblStatus.Size = new System.Drawing.Size(48, 17);
      this.tsLblStatus.Text = "Ready...";
      // 
      // FormInstaller
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(664, 178);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.cbDeveloper);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.cobVersion);
      this.Controls.Add(this.btnUninstall);
      this.Controls.Add(this.btnInstall);
      this.Controls.Add(this.btnBrowse);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtBesiegeLocation);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormInstaller";
      this.Text = "spaar\'s Mod Loader Installer";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormInstaller_FormClosed);
      this.Load += new System.EventHandler(this.FormInstaller_Load);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtBesiegeLocation;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.Button btnInstall;
    private System.Windows.Forms.Button btnUninstall;
    private System.Windows.Forms.ComboBox cobVersion;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox cbDeveloper;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel tsLblStatus;
  }
}

