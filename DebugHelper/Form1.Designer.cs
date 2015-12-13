namespace DebugHelper
{
  partial class Form1
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
      this.btnStart = new System.Windows.Forms.Button();
      this.btnReload = new System.Windows.Forms.Button();
      this.txtModLocation = new System.Windows.Forms.TextBox();
      this.btnBrowseBesiege = new System.Windows.Forms.Button();
      this.btnBrowseMod = new System.Windows.Forms.Button();
      this.txtModName = new System.Windows.Forms.TextBox();
      this.lblBesiegeLocation = new System.Windows.Forms.Label();
      this.lblModLocation = new System.Windows.Forms.Label();
      this.lblModName = new System.Windows.Forms.Label();
      this.btnReset = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtBesiegeLocation
      // 
      this.txtBesiegeLocation.Location = new System.Drawing.Point(130, 8);
      this.txtBesiegeLocation.Name = "txtBesiegeLocation";
      this.txtBesiegeLocation.Size = new System.Drawing.Size(285, 20);
      this.txtBesiegeLocation.TabIndex = 0;
      // 
      // btnStart
      // 
      this.btnStart.Location = new System.Drawing.Point(15, 101);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(132, 36);
      this.btnStart.TabIndex = 1;
      this.btnStart.Text = "Start";
      this.btnStart.UseVisualStyleBackColor = true;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // btnReload
      // 
      this.btnReload.Location = new System.Drawing.Point(153, 101);
      this.btnReload.Name = "btnReload";
      this.btnReload.Size = new System.Drawing.Size(132, 36);
      this.btnReload.TabIndex = 2;
      this.btnReload.Text = "Reload Mod";
      this.btnReload.UseVisualStyleBackColor = true;
      this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
      // 
      // txtModLocation
      // 
      this.txtModLocation.Location = new System.Drawing.Point(130, 34);
      this.txtModLocation.Name = "txtModLocation";
      this.txtModLocation.Size = new System.Drawing.Size(285, 20);
      this.txtModLocation.TabIndex = 3;
      // 
      // btnBrowseBesiege
      // 
      this.btnBrowseBesiege.Location = new System.Drawing.Point(421, 6);
      this.btnBrowseBesiege.Name = "btnBrowseBesiege";
      this.btnBrowseBesiege.Size = new System.Drawing.Size(29, 23);
      this.btnBrowseBesiege.TabIndex = 4;
      this.btnBrowseBesiege.Text = "...";
      this.btnBrowseBesiege.UseVisualStyleBackColor = true;
      this.btnBrowseBesiege.Click += new System.EventHandler(this.btnBrowseBesiege_Click);
      // 
      // btnBrowseMod
      // 
      this.btnBrowseMod.Location = new System.Drawing.Point(421, 32);
      this.btnBrowseMod.Name = "btnBrowseMod";
      this.btnBrowseMod.Size = new System.Drawing.Size(29, 23);
      this.btnBrowseMod.TabIndex = 5;
      this.btnBrowseMod.Text = "...";
      this.btnBrowseMod.UseVisualStyleBackColor = true;
      this.btnBrowseMod.Click += new System.EventHandler(this.btnBrowseMod_Click);
      // 
      // txtModName
      // 
      this.txtModName.Location = new System.Drawing.Point(130, 60);
      this.txtModName.Name = "txtModName";
      this.txtModName.Size = new System.Drawing.Size(285, 20);
      this.txtModName.TabIndex = 6;
      // 
      // lblBesiegeLocation
      // 
      this.lblBesiegeLocation.AutoSize = true;
      this.lblBesiegeLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblBesiegeLocation.Location = new System.Drawing.Point(12, 9);
      this.lblBesiegeLocation.Name = "lblBesiegeLocation";
      this.lblBesiegeLocation.Size = new System.Drawing.Size(112, 16);
      this.lblBesiegeLocation.TabIndex = 7;
      this.lblBesiegeLocation.Text = "Besiege location:";
      // 
      // lblModLocation
      // 
      this.lblModLocation.AutoSize = true;
      this.lblModLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblModLocation.Location = new System.Drawing.Point(36, 35);
      this.lblModLocation.Name = "lblModLocation";
      this.lblModLocation.Size = new System.Drawing.Size(88, 16);
      this.lblModLocation.TabIndex = 8;
      this.lblModLocation.Text = "Mod location:";
      // 
      // lblModName
      // 
      this.lblModName.AutoSize = true;
      this.lblModName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblModName.Location = new System.Drawing.Point(49, 61);
      this.lblModName.Name = "lblModName";
      this.lblModName.Size = new System.Drawing.Size(75, 16);
      this.lblModName.TabIndex = 9;
      this.lblModName.Text = "Mod name:";
      // 
      // btnReset
      // 
      this.btnReset.Location = new System.Drawing.Point(291, 101);
      this.btnReset.Name = "btnReset";
      this.btnReset.Size = new System.Drawing.Size(132, 36);
      this.btnReset.TabIndex = 10;
      this.btnReset.Text = "Reset";
      this.btnReset.UseVisualStyleBackColor = true;
      this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(464, 148);
      this.Controls.Add(this.btnReset);
      this.Controls.Add(this.lblModName);
      this.Controls.Add(this.lblModLocation);
      this.Controls.Add(this.lblBesiegeLocation);
      this.Controls.Add(this.txtModName);
      this.Controls.Add(this.btnBrowseMod);
      this.Controls.Add(this.btnBrowseBesiege);
      this.Controls.Add(this.txtModLocation);
      this.Controls.Add(this.btnReload);
      this.Controls.Add(this.btnStart);
      this.Controls.Add(this.txtBesiegeLocation);
      this.Name = "Form1";
      this.Text = "Besiege Modding Debug Helper";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtBesiegeLocation;
    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Button btnReload;
    private System.Windows.Forms.TextBox txtModLocation;
    private System.Windows.Forms.Button btnBrowseBesiege;
    private System.Windows.Forms.Button btnBrowseMod;
    private System.Windows.Forms.TextBox txtModName;
    private System.Windows.Forms.Label lblBesiegeLocation;
    private System.Windows.Forms.Label lblModLocation;
    private System.Windows.Forms.Label lblModName;
    private System.Windows.Forms.Button btnReset;
  }
}

