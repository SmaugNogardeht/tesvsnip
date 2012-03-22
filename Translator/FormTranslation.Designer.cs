namespace TESVSnip.Translator
{
  partial class FormTranslation
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
      this.txtTextTarget = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.panel4 = new System.Windows.Forms.Panel();
      this.txtTextSource = new System.Windows.Forms.TextBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.panel3 = new System.Windows.Forms.Panel();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnTranslate = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.panel4.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel3.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtTextTarget
      // 
      this.txtTextTarget.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtTextTarget.Location = new System.Drawing.Point(0, 0);
      this.txtTextTarget.Multiline = true;
      this.txtTextTarget.Name = "txtTextTarget";
      this.txtTextTarget.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtTextTarget.Size = new System.Drawing.Size(887, 321);
      this.txtTextTarget.TabIndex = 1;
      // 
      // btnSave
      // 
      this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.btnSave.Location = new System.Drawing.Point(3, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(108, 45);
      this.btnSave.TabIndex = 0;
      this.btnSave.Text = "Save";
      this.toolTip1.SetToolTip(this.btnSave, "Save translated text to Translation/New Text");
      this.btnSave.UseVisualStyleBackColor = true;
      // 
      // panel4
      // 
      this.panel4.Controls.Add(this.txtTextTarget);
      this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel4.Location = new System.Drawing.Point(0, 287);
      this.panel4.Name = "panel4";
      this.panel4.Size = new System.Drawing.Size(887, 321);
      this.panel4.TabIndex = 4;
      // 
      // txtTextSource
      // 
      this.txtTextSource.BackColor = System.Drawing.SystemColors.InactiveBorder;
      this.txtTextSource.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtTextSource.HideSelection = false;
      this.txtTextSource.Location = new System.Drawing.Point(0, 0);
      this.txtTextSource.Multiline = true;
      this.txtTextSource.Name = "txtTextSource";
      this.txtTextSource.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtTextSource.Size = new System.Drawing.Size(887, 287);
      this.txtTextSource.TabIndex = 0;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.panel4);
      this.panel2.Controls.Add(this.panel3);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.Location = new System.Drawing.Point(0, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(887, 608);
      this.panel2.TabIndex = 5;
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.txtTextSource);
      this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel3.Location = new System.Drawing.Point(0, 0);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(887, 287);
      this.panel3.TabIndex = 3;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnTranslate);
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Controls.Add(this.btnSave);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 608);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(887, 51);
      this.panel1.TabIndex = 4;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.ForeColor = System.Drawing.Color.Red;
      this.btnCancel.Location = new System.Drawing.Point(772, 3);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(108, 45);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnTranslate
      // 
      this.btnTranslate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnTranslate.ForeColor = System.Drawing.Color.Navy;
      this.btnTranslate.Location = new System.Drawing.Point(117, 3);
      this.btnTranslate.Name = "btnTranslate";
      this.btnTranslate.Size = new System.Drawing.Size(108, 45);
      this.btnTranslate.TabIndex = 2;
      this.btnTranslate.Text = "Translate";
      this.btnTranslate.UseVisualStyleBackColor = true;
      this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
      // 
      // FormTranslation
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(887, 659);
      this.ControlBox = false;
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "FormTranslation";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Translation with Web Engine";
      this.TopMost = true;
      this.panel4.ResumeLayout(false);
      this.panel4.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel3.ResumeLayout(false);
      this.panel3.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox txtTextTarget;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Panel panel4;
    private System.Windows.Forms.TextBox txtTextSource;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnTranslate;
    private System.Windows.Forms.ToolTip toolTip1;
  }
}