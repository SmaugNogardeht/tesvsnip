namespace TESVSnip.Translator
{
  partial class FormLongText
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
      this.btnSave = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.panel4 = new System.Windows.Forms.Panel();
      this.txtLongTextTarget = new System.Windows.Forms.TextBox();
      this.panel3 = new System.Windows.Forms.Panel();
      this.txtLongTextSource = new System.Windows.Forms.TextBox();
      this.btnCopy = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel4.SuspendLayout();
      this.panel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSave
      // 
      this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.btnSave.Location = new System.Drawing.Point(12, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(108, 45);
      this.btnSave.TabIndex = 0;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Controls.Add(this.btnSave);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 617);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(838, 51);
      this.panel1.TabIndex = 2;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.ForeColor = System.Drawing.Color.Red;
      this.btnCancel.Location = new System.Drawing.Point(126, 3);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(108, 45);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.panel4);
      this.panel2.Controls.Add(this.panel3);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.Location = new System.Drawing.Point(0, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(838, 617);
      this.panel2.TabIndex = 3;
      // 
      // panel4
      // 
      this.panel4.Controls.Add(this.txtLongTextTarget);
      this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel4.Location = new System.Drawing.Point(0, 287);
      this.panel4.Name = "panel4";
      this.panel4.Size = new System.Drawing.Size(838, 330);
      this.panel4.TabIndex = 4;
      // 
      // txtLongTextTarget
      // 
      this.txtLongTextTarget.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtLongTextTarget.Location = new System.Drawing.Point(0, 0);
      this.txtLongTextTarget.Multiline = true;
      this.txtLongTextTarget.Name = "txtLongTextTarget";
      this.txtLongTextTarget.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtLongTextTarget.Size = new System.Drawing.Size(838, 330);
      this.txtLongTextTarget.TabIndex = 1;
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.txtLongTextSource);
      this.panel3.Controls.Add(this.btnCopy);
      this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel3.Location = new System.Drawing.Point(0, 0);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(838, 287);
      this.panel3.TabIndex = 3;
      // 
      // txtLongTextSource
      // 
      this.txtLongTextSource.BackColor = System.Drawing.SystemColors.InactiveBorder;
      this.txtLongTextSource.Dock = System.Windows.Forms.DockStyle.Top;
      this.txtLongTextSource.Location = new System.Drawing.Point(0, 0);
      this.txtLongTextSource.Multiline = true;
      this.txtLongTextSource.Name = "txtLongTextSource";
      this.txtLongTextSource.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtLongTextSource.Size = new System.Drawing.Size(838, 264);
      this.txtLongTextSource.TabIndex = 0;
      // 
      // btnCopy
      // 
      this.btnCopy.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.btnCopy.Location = new System.Drawing.Point(0, 264);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Size = new System.Drawing.Size(838, 23);
      this.btnCopy.TabIndex = 2;
      this.btnCopy.Text = "Copy Source to Target";
      this.btnCopy.UseVisualStyleBackColor = true;
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      // 
      // FormLongText
      // 
      this.AcceptButton = this.btnSave;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(838, 668);
      this.ControlBox = false;
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.KeyPreview = true;
      this.Name = "FormLongText";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Editor for Long Text";
      this.TopMost = true;
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel4.ResumeLayout(false);
      this.panel4.PerformLayout();
      this.panel3.ResumeLayout(false);
      this.panel3.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.TextBox txtLongTextSource;
    private System.Windows.Forms.TextBox txtLongTextTarget;
    private System.Windows.Forms.Button btnCopy;
    private System.Windows.Forms.Panel panel4;
    private System.Windows.Forms.Panel panel3;
  }
}