using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using RTF;

using TESVSnip;
using TESVSnip.Data;
using TESVSnip.Docking;
using TESVSnip.Forms;
using TESVSnip.Properties;
using TESVSnip.ObjectControls;
using TESVSnip.Model;
using TESVSnip.Translator;
using TESVSnip.Collections;
using TESVSnip.Collections.Generic;
using BrightIdeasSoftware;

namespace TESVSnip.Docking
{
  public partial class TranslatorHelper : BaseDockContent
  {

    internal MainView MainViewTH = null;
    public TESVSnip.Plugin PluginTree = null;

    public DataTable tblPlugInHeader = new DataSetTH.T_PlugInHeaderDataTable();

    public DataTable tblPlugInStringsLoad = new DataSetTH.T_PlugInStringsDataTable();
    public DataTable tblPlugInStringsProject = new DataSetTH.T_PlugInStringsDataTable();

    public DataTable tblSkyrimSourceStrings = new DataSetTH.T_SkyrimStringsDataTable();
    public DataTable tblSkyrimTargetStrings = new DataSetTH.T_SkyrimStringsDataTable();
    public DataTable tblSkyrimEsmDict = new DataSetTH.T_StringsDictDataTable();
    private DataTable tblSkyrimStrings = new DataSetTH.T_SkyrimStringsDataTable();

    private string PluginLocation = String.Empty;

    private System.Collections.Generic.List<ObjStrings> listViewStrings = null;
    private System.Collections.Generic.List<ObjStrings> listViewStringsDL = null;
    private System.Collections.Generic.List<ObjStrings> listViewStringsIL = null;
    private System.Collections.Generic.List<ObjStrings> listViewStringsOther = null;

    private System.Collections.Generic.List<ObjStringsDict> listViewSkyrimDict = null;

    private System.Collections.Generic.List<ObjOtherSkyrimStrings> listViewOtherSkyrimStringsSource = null;
    private System.Collections.Generic.List<ObjOtherSkyrimStrings> listViewOtherSkyrimStringsTarget = null;

    private DataView dvPlugIn = new DataView();

    private TESVSnip.Docking.ObjStrings lastOLVListItem = null; //for update object in listview

    private string LastLoadedSkyrimEsmDictionnary = String.Empty;

    public string GetSourceLanguage() { return cboxSourceLanguage.Text; }
    public string GetTargetLanguage() { return cboxTargetLanguage.Text; }

    bool populateListViewStringsInProgress = false;

    /// <summary>
    /// Constructor TranslatorHelper
    /// </summary>
    public TranslatorHelper()
    {

      InitializeComponent();

      tabControlTranslatorHelper.Dock = DockStyle.Fill;
      this.olvTHStrings.AddDecoration(new EditingCellBorderDecoration(true));
      this.olvTHDLStrings.AddDecoration(new EditingCellBorderDecoration(true));
      CreateListViewColumn();
      PopulateLanguageComboBox();
      cboxSourceLanguage.Text = TESVSnip.Properties.Settings.Default.THSourceLanguage;
      cboxTargetLanguage.Text = TESVSnip.Properties.Settings.Default.THTargetLanguage;
      ClearTextBoxControl(this);
      dvPlugIn.Table = tblPlugInStringsProject;
      dvPlugIn.Sort = "FormIDHexa, StringType, RecordTypeTH";
    }

    /// <summary>
    /// SetTextAsString
    /// </summary>
    private void SetTextAsString(ArraySegment<byte> data, ref string desc, ref string stringID)
    {
      desc = TypeConverter.GetString(data);
      stringID = 0.ToString("X8");
    }

    /// <summary>
    /// SetTextByID
    /// </summary>
    private void SetTextByID(ArraySegment<byte> data, ref string desc, ref string stringID)
    {
      uint id = TypeConverter.h2i(data);
      stringID = id.ToString("X8");
      desc = String.Empty;
    }

    /// <summary>
    /// TranslatorHelper_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TranslatorHelper_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// olvTHStrings_ItemSelectionChanged          string[] lines = this.textBox1.Text.Split('\n');
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListView_Item_Click(object sender, EventArgs e)
    {
      ListViewItemSelection(((ObjectListView)sender).Name);
    }

    private void ListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      //ListViewItemSelection(((ObjectListView)sender).Name);
    }

    /// <summary>
    /// ListViewItemSelection
    /// </summary>
    /// <param name="sender"></param>
    private void ListViewItemSelection(string sender)
    {
      OLVListItem item = null; // (OLVListItem)e.Item;
      //return;
      string StringType;
      if (sender == "olvTHStrings")
      {
        StringType = "Strings";
        item = olvTHStrings.SelectedItem;
      }
      else
        if (sender == "olvTHDLStrings")
        {
          StringType = "DLStrings";
          item = olvTHDLStrings.SelectedItem;
        }
        else
          if (sender == "olvTHILStrings")
          {
            StringType = "ILStrings";
            item = olvTHILStrings.SelectedItem;
          }
          else
          {
            StringType = "OtherStrings";
            item = olvTHOtherStrings.SelectedItem;
          }

      if (item == null) return;

      TESVSnip.Docking.ObjStrings itemObj = (TESVSnip.Docking.ObjStrings)item.RowObject;

      DataRowView[] foundRows;
      try
      {
        foundRows = dvPlugIn.FindRows(new object[] { itemObj.FormID, StringType, itemObj.RecordType });

        if (foundRows.Length > 0)
        {
          populateListViewStringsInProgress = true;
          txtFormID.Text = itemObj.FormID;

          txtGroupName.Text = Convert.ToString(foundRows[0]["GroupName"]);
          txtFormID.Text = Convert.ToString(foundRows[0]["FormIDHexa"]);
          txtStringSkyrimEditorID.Text = Convert.ToString(foundRows[0]["EditorID"]);
          txtSkyrimStringID.Text = Convert.ToString(foundRows[0]["SkyrimStringIDHexa"]);
          txtStringSkyrimDescSource.Text = Convert.ToString(foundRows[0]["SkyrimItemDescSourceLang"]);
          txtStringSkyrimDescTarget.Text = Convert.ToString(foundRows[0]["SkyrimItemDescTargetLang"]);
          txtSkyrimRecordType.Text = StringType;
          txtSkyrimRecordTypeTH.Text = Convert.ToString(foundRows[0]["RecordTypeTH"]);
  
          txtSourceStringsID.Text = Convert.ToString(foundRows[0]["SourceStringIDHexa"]);
          txtSourceEditorID.Text = Convert.ToString(foundRows[0]["SourceEditorID"]);
          txtSourceStringNew.Text = Convert.ToString(foundRows[0]["SourceItemDesc"]);
          txtSourceStringOld.Text = Convert.ToString(foundRows[0]["SourceItemDescOld"]);

          txtTargetStringsID.Text = Convert.ToString(foundRows[0]["TargetStringIDHexa"]);
          txtTargetEditorID.Text = Convert.ToString(foundRows[0]["TargetEditorID"]);
          txtTargetStringNew.Text = Convert.ToString(foundRows[0]["TargerItemDesc"]);
          txtTargetStringOld.Text = Convert.ToString(foundRows[0]["TargerItemDescOld"]);

          if (Convert.ToBoolean(foundRows[0]["WriteStringInPlugIn"]))
            chkboxNewTextTranslate.CheckState = System.Windows.Forms.CheckState.Checked;
          else
            chkboxNewTextTranslate.CheckState = System.Windows.Forms.CheckState.Unchecked;

          if (txtStringSkyrimDescSource.Text != txtSourceStringNew.Text)
          {
            txtStringSkyrimDescSource.BackColor = System.Drawing.Color.MistyRose;
            txtSourceStringNew.BackColor = System.Drawing.Color.MistyRose;
          }
          else
          {
            txtStringSkyrimDescSource.BackColor = System.Drawing.Color.White; // Lavender;
            txtSourceStringNew.BackColor = System.Drawing.Color.White; // LemonChiffon;
          }

          if (txtStringSkyrimDescTarget.Text != txtTargetStringNew.Text)
          {
            txtStringSkyrimDescTarget.BackColor = System.Drawing.Color.Moccasin;
            txtTargetStringNew.BackColor = System.Drawing.Color.Moccasin;
          }
          else
          {
            txtStringSkyrimDescTarget.BackColor = System.Drawing.Color.White; // Lavender; 
            txtTargetStringNew.BackColor = System.Drawing.Color.White; // LemonChiffon;
          }

          lastOLVListItem = itemObj;

          FindOtherPossibleStringTranslation();

          populateListViewStringsInProgress = false;
        }
      }
      catch (Exception ex)
      {
        edtMemo.Text += ex.Message + Environment.NewLine;
      }
    }

    /// <summary>
    /// CellToolTipShowing 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListView_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
    {
      OLVColumn col = e.Column ?? e.ListView.GetColumn(0);
      string stringValue = col.GetStringValue(e.Model);
      e.IsBalloon = !ObjectListView.IsVistaOrLater; // balloons don't work reliably on vista
      e.ToolTipControl.SetMaxWidth(400);
      e.Title = String.Empty; //"Strings";
      e.StandardIcon = ToolTipControl.StandardIcons.InfoLarge;
      e.BackColor = System.Drawing.Color.AliceBlue;
      e.ForeColor = System.Drawing.Color.IndianRed;
      e.AutoPopDelay = 15000;
      e.Font = new System.Drawing.Font("Tahoma", 10.0f);
      e.Text = stringValue;
    }

    /// <summary>
    /// ListView_FormatRow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListView_FormatRow(object sender, FormatRowEventArgs e)
    {
      try
      {
        ObjStrings item = (ObjStrings)e.Model;

        float fontSize = e.Item.Font.Size;
        FontFamily fontFamily = e.Item.Font.FontFamily;
        FontStyle fontStyle = System.Drawing.FontStyle.Regular;
        Color foreColor = Color.Black;
        Color backColor = Color.Transparent;

        if (!item.WriteStringInPlugIn)
        {
          fontStyle = System.Drawing.FontStyle.Italic;
          foreColor = Color.OrangeRed;
        }

        if (item.StringStatus == "Del")
        {
          if (fontStyle == System.Drawing.FontStyle.Italic)
            fontStyle = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Strikeout;
          else
            fontStyle = System.Drawing.FontStyle.Strikeout;
        }


        e.Item.Font = new Font(fontFamily, fontSize, fontStyle);
        e.Item.ForeColor = foreColor;

      }
      catch (Exception ex)
      {
        edtMemo.Text += ex.Message + Environment.NewLine;
      }
    }

    /// <summary>
    /// txtTargetStringNew_Validated
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtTargetStringNew_Validated(object sender, EventArgs e)
    {
      if (!populateListViewStringsInProgress) SaveChange();
    }

    /// <summary>
    /// Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkboxNewTextTranslate_CheckedChanged(object sender, EventArgs e)
    {
      if (!populateListViewStringsInProgress) SaveChange();
    }

    private void SaveChange()
    {
      string FormID = txtFormID.Text;
      string RecordType = txtSkyrimRecordType.Text;
      string RecordName = txtSkyrimRecordTypeTH.Text;

      DataRowView[] foundRows;
      foundRows = dvPlugIn.FindRows(new object[] { FormID, RecordType, RecordName });

      if (foundRows.Length == 1)
      {
        foundRows[0].BeginEdit();
        foundRows[0].Row["TargerItemDesc"] = txtTargetStringNew.Text;
        foundRows[0].Row["WriteStringInPlugIn"] = chkboxNewTextTranslate.Checked;
        foundRows[0].EndEdit();
        lastOLVListItem.TargetItemDesc = txtTargetStringNew.Text;
        lastOLVListItem.WriteStringInPlugIn = chkboxNewTextTranslate.Checked;
      }
    }

    /// <summary>
    /// txtTargetStringNew_Validating
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtTargetStringNew_Validating(object sender, CancelEventArgs e)
    {
      string RecordType = txtSkyrimRecordType.Text;
      string[] lines = this.txtTargetStringNew.Text.Split('\n');

      if ((RecordType == "FULL") | (RecordType == "DESC"))
        if (lines.Length != 1)
        {
          MessageBox.Show("More one line of text. Validation aborted.", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          e.Cancel = true;
        }
    }

    /// <summary>
    /// txtTargetStringNew_KeyPress
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtTargetStringNew_KeyPress(object sender, KeyPressEventArgs e)
    {
      e.Handled = e.KeyChar == 13;//block return key because there is no default button
    }

    /// <summary>
    /// Search in list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtSearchInStringsList_TextChanged(object sender, EventArgs e)
    {
      if (!String.IsNullOrWhiteSpace(txtSearchInStringsList.Text))
      {
        TextMatchFilter filter = TextMatchFilter.Contains(this.olvTHStrings, txtSearchInStringsList.Text);
        this.olvTHStrings.ModelFilter = filter;
        this.olvTHStrings.DefaultRenderer = new HighlightTextRenderer(filter);
      }
    }

    /// <summary>
    /// txtSearchInDLStringsList_TextChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtSearchInDLStringsList_TextChanged(object sender, EventArgs e)
    {
      if (!String.IsNullOrWhiteSpace(txtSearchInDLStringsList.Text))
      {
        TextMatchFilter filter = TextMatchFilter.Contains(this.olvTHDLStrings, txtSearchInDLStringsList.Text);
        this.olvTHDLStrings.ModelFilter = filter;
        this.olvTHDLStrings.DefaultRenderer = new HighlightTextRenderer(filter);
      }
    }

    /// <summary>
    /// txtSearchInILStringsList_TextChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtSearchInILStringsList_TextChanged(object sender, EventArgs e)
    {
      if (!String.IsNullOrWhiteSpace(txtSearchInILStringsList.Text))
      {
        TextMatchFilter filter = TextMatchFilter.Contains(this.olvTHILStrings, txtSearchInILStringsList.Text);
        this.olvTHILStrings.ModelFilter = filter;
        this.olvTHILStrings.DefaultRenderer = new HighlightTextRenderer(filter);
      }
    }

    /// <summary>
    /// btnSearchDict_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearchDict_Click(object sender, EventArgs e)
    {
      if (!String.IsNullOrWhiteSpace(txtSearchInSkyrimString.Text))
      {
        TextMatchFilter filter = TextMatchFilter.Contains(this.olvSkyrimDict, txtSearchInSkyrimString.Text);
        this.olvSkyrimDict.ModelFilter = filter;
        this.olvSkyrimDict.DefaultRenderer = new HighlightTextRenderer(filter);
      }
    }

    /// <summary>
    /// comboxBox Language
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TranslatorHelper_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.THSourceLanguage = cboxSourceLanguage.Text;
      TESVSnip.Properties.Settings.Default.THTargetLanguage = cboxTargetLanguage.Text;
      TESVSnip.Properties.Settings.Default.Save();
    }

    /// <summary>
    /// Open long text
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtTargetStringNew_DoubleClick(object sender, EventArgs e)
    {
      bool process = false;

      string[] lines = this.txtTargetStringNew.Text.Split('\n');
      if (lines.Length > 1)
        process = true;
      else
      {
        lines = this.txtSourceStringNew.Text.Split('\n');
        if (lines.Length > 1) process = true;
      }

      if (process)
      {
        TESVSnip.Translator.FormLongText formLongText = new TESVSnip.Translator.FormLongText();
        formLongText.SetTextBox(txtSourceStringNew.Text, txtTargetStringNew.Text);
        if (formLongText.ShowDialog() == DialogResult.OK)
        {
          txtTargetStringNew.Text = formLongText.GetLongTextTarget();
          txtTargetStringNew_Validated(sender, e);
        }
        formLongText.Close();
        formLongText.Dispose();
        formLongText = null;
      }
    }

    /// <summary>
    /// cboxLanguage - SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cboxLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.MainViewTH == null) return;
      if (((ComboBox)sender).Text == "Japanese")
        this.MainViewTH.UpdateUTF8State(true);
      else
        this.MainViewTH.UpdateUTF8State(false);
    }

    private void olvTHSkyrimSourceStrings_DoubleClick(object sender, EventArgs e)
    {
      OLVListItem item = ((BrightIdeasSoftware.ObjectListView)sender).SelectedItem;
      TESVSnip.Docking.ObjOtherSkyrimStrings itemObj = (TESVSnip.Docking.ObjOtherSkyrimStrings)item.RowObject;
      txtSourceStringOld.Text = txtSourceStringNew.Text;
      txtSourceStringNew.Text = itemObj.SkyrimText;
    }

    private void olvTHSkyrimTargetStrings_DoubleClick(object sender, EventArgs e)
    {
      OLVListItem item = ((BrightIdeasSoftware.ObjectListView)sender).SelectedItem;
      TESVSnip.Docking.ObjOtherSkyrimStrings itemObj = (TESVSnip.Docking.ObjOtherSkyrimStrings)item.RowObject;
      txtTargetStringOld.Text = txtTargetStringNew.Text;
      txtTargetStringNew.Text = itemObj.SkyrimText;
    }


  }


  ///**************************************************************************************
  ///**************************************************************************************
  ///**************************************************************************************


  /// <summary>
  /// ObjStrings
  /// </summary>
  class ObjStrings
  {
    public ObjStrings() { }

    public ObjStrings(string GroupName, string StringStatus, string CompareStatusSource,
      string CompareStatusTarget, string RecordType, string FormIDHexa, string EditorID, string SourceStringIDHexa,
      string SourceItemDesc, string TargetItemDesc, bool WriteStringInPlugIn)
    {

      this.groupName = GroupName;
      this.stringStatus = StringStatus;
      this.compareStatusSource = CompareStatusSource;
      this.compareStatusTarget = CompareStatusTarget;
      this.recordType = RecordType;
      this.formID = FormIDHexa;
      this.editorID = EditorID;
      this.sourceStringIDHexa = SourceStringIDHexa;
      this.sourceItemDesc = SourceItemDesc;
      this.targetItemDesc = TargetItemDesc;
      this.WriteStringInPlugIn = WriteStringInPlugIn;
    }

    private string groupName;
    public string GroupName { get { return groupName; } set { groupName = value; } }

    private string stringStatus;
    public string StringStatus { get { return stringStatus; } set { stringStatus = value; } }

    private string compareStatusSource;
    public string CompareStatusSource { get { return compareStatusSource; } set { compareStatusSource = value; } }

    private string compareStatusTarget;
    public string CompareStatusTarget { get { return compareStatusTarget; } set { compareStatusTarget = value; } }

    private string recordType;
    public string RecordType { get { return recordType; } set { recordType = value; } }

    private string formID;
    public string FormID { get { return formID; } set { formID = value; } }

    private string editorID;
    public string EditorID { get { return editorID; } set { editorID = value; } }

    private string sourceStringIDHexa;
    public string SourceStringIDHexa { get { return sourceStringIDHexa; } set { sourceStringIDHexa = value; } }

    private string sourceItemDesc;
    public string SourceItemDesc { get { return sourceItemDesc; } set { sourceItemDesc = value; } }

    private string targetItemDesc;
    public string TargetItemDesc { get { return targetItemDesc; } set { targetItemDesc = value; } }

    private bool writeStringInPlugIn;
    public bool WriteStringInPlugIn { get { return writeStringInPlugIn; } set { writeStringInPlugIn = value; } }

  }

  /// <summary>
  /// ObjStringsDict
  /// </summary>
  class ObjStringsDict
  {
    public ObjStringsDict() { }

    public ObjStringsDict(string StringID, string SourceString, string TargetString)
    {
      this.stringID = StringID;
      this.sourceString = SourceString;
      this.targetString = TargetString;
    }

    private string stringID;
    public string StringID { get { return stringID; } set { stringID = value; } }

    private string sourceString;
    public string SourceString { get { return sourceString; } set { sourceString = value; } }

    private string targetString;
    public string TargetString { get { return targetString; } set { targetString = value; } }
  }

  /// <summary>
  /// ObjStringsSkyrim
  /// </summary>
  class ObjOtherSkyrimStrings
  {
    public ObjOtherSkyrimStrings() { }

    public ObjOtherSkyrimStrings(string StringID, string SkyrimText)
    {
      this.stringID = StringID;
      this.skyrimText = SkyrimText;
    }

    private string stringID;
    public string StringID { get { return stringID; } set { stringID = value; } }

    private string skyrimText;
    public string SkyrimText { get { return skyrimText; } set { skyrimText = value; } }
  }

}