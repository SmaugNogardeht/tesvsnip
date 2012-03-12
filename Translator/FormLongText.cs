using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TESVSnip.Translator
{
  public partial class FormLongText : Form
  {
    public FormLongText()
    {
      InitializeComponent();
    }

    public void SetTextBox(string sourceText, string targetText)
    {
      txtLongTextSource.Text = sourceText;
      txtLongTextTarget.Text = targetText;
    }

    public string GetLongTextTarget()
    {
      return txtLongTextTarget.Text;
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      txtLongTextTarget.Text = txtLongTextSource.Text;
    }
  }
}
