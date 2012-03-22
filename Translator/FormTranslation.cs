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
  public partial class FormTranslation : Form
  {
    int codePage;
    string sourceLang;
    string targetLang;
    TranslatorWebEngine translateWithWeb;

    public FormTranslation(int codePage, string sourceLang, string targetLang, ref TranslatorWebEngine translateWithWeb)
    {
      InitializeComponent();
      this.codePage = codePage;
      this.sourceLang = sourceLang;
      this.targetLang = targetLang;
      this.translateWithWeb = translateWithWeb;
      this.Text = "Translation with Web Engine - " + sourceLang + " to " + targetLang;
    }

    public void SetTextBox(int selectionStart, int selectionLength, string sourceText)
    {
      txtTextSource.Text = sourceText;
      if (selectionLength > 0) txtTextSource.Select(selectionStart, selectionLength);
    }

    public string GeTextTranslated()
    {
      return txtTextTarget.Text;
    }

    private void btnTranslate_Click(object sender, EventArgs e)
    {
      TranslateText();
    }

    public void TranslateText()
    {
      Cursor.Current = Cursors.WaitCursor;
      translateWithWeb.SourceLang = sourceLang;
      translateWithWeb.TargetLang = targetLang;
      if (txtTextSource.SelectionLength > 0)
        txtTextTarget.Text = translateWithWeb.TranslateText(txtTextSource.SelectedText);
      else
        txtTextTarget.Text = translateWithWeb.TranslateText(txtTextSource.Text);
      Cursor.Current = Cursors.Default;
    }

  }
}
