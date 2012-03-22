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

    private DataTable tblSkyrimStrings = new DataSetTH.T_SkyrimStringsDataTable();

    private string PluginLocation = String.Empty;

    private DataView dvPlugIn = new DataView();

    private TESVSnip.Docking.ObjStrings lastOLVListItemObjStrings = null; //for update object in listview
    private OLVListItem lastOLVListItem = null;
    private TranslatorWebEngine translateWithWeb = new TranslatorWebEngine();

    public string GetSourceLanguage() { return cboxSourceLanguage.Text; }
    public string GetTargetLanguage() { return cboxTargetLanguage.Text; }

    bool populateListViewStringsInProgress = false;

    string olvTHStringsLastSelectedColumnName = String.Empty;
    SortOrder olvTHStringsLastSortOrder = SortOrder.None;

    string olvTHStringsDLLastSelectedColumnName = String.Empty;
    SortOrder olvTHStringsDLLastSortOrder = SortOrder.None;

    string olvTHStringsILLastSelectedColumnName = String.Empty;
    SortOrder olvTHStringsILLastSortOrder = SortOrder.None;

    string olvTHStringsOTHERLastSelectedColumnName = String.Empty;
    SortOrder olvTHStringsOTHERLastSortOrder = SortOrder.None;

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

      CreateListViewColumnTH("Strings", "EditorID", SortOrder.None);
      olvTHStringsLastSortOrder = SortOrder.Ascending; olvTHStringsLastSelectedColumnName = "EditorID";

      CreateListViewColumnTH("DLStrings", "EditorID", SortOrder.None);
      olvTHStringsDLLastSortOrder = SortOrder.Ascending; olvTHStringsDLLastSelectedColumnName = "EditorID";

      CreateListViewColumnTH("ILStrings", "EditorID", SortOrder.None);
      olvTHStringsILLastSortOrder = SortOrder.Ascending; olvTHStringsILLastSelectedColumnName = "EditorID";

      CreateListViewColumnTH("OtherStrings", "EditorID", SortOrder.None);
      olvTHStringsOTHERLastSortOrder = SortOrder.Ascending; olvTHStringsOTHERLastSelectedColumnName = "EditorID";

      PopulateLanguageComboBox();

      cboxSourceLanguage.Text = TESVSnip.Properties.Settings.Default.THSourceLanguage;
      cboxTargetLanguage.Text = TESVSnip.Properties.Settings.Default.THTargetLanguage;
      ClearTextBoxControl(this);
      dvPlugIn.Table = tblPlugInStringsProject;
      dvPlugIn.Sort = "FormIDHexa, StringType, RecordTypeTH";

      cbSearchInSkyrimString.Items.Add("String");
      cbSearchInSkyrimString.Items.Add("DLString");
      cbSearchInSkyrimString.Items.Add("ILString");
      cbSearchInSkyrimString.CheckBoxItems[0].Checked = true;
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
      Cursor.Current = Cursors.WaitCursor;

      string StringType;
      if (sender == "olvTHStrings")
      {
        StringType = "Strings";
        lastOLVListItem = olvTHStrings.SelectedItem;
      }
      else
        if (sender == "olvTHDLStrings")
        {
          StringType = "DLStrings";
          lastOLVListItem = olvTHDLStrings.SelectedItem;
        }
        else
          if (sender == "olvTHILStrings")
          {
            StringType = "ILStrings";
            lastOLVListItem = olvTHILStrings.SelectedItem;
          }
          else
          {
            StringType = "OtherStrings";
            lastOLVListItem = olvTHOtherStrings.SelectedItem;
          }

      if (lastOLVListItem == null) { Cursor.Current = Cursors.Default; return; }

      TESVSnip.Docking.ObjStrings itemObj = (TESVSnip.Docking.ObjStrings)lastOLVListItem.RowObject;

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
          txtTargetStringNew.Text = Convert.ToString(foundRows[0]["TargetItemDesc"]);
          txtTargetStringOld.Text = Convert.ToString(foundRows[0]["TargetItemDescOld"]);

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

          lastOLVListItemObjStrings = itemObj;

          FindOtherPossibleStringTranslation();

          populateListViewStringsInProgress = false;
        }

        Cursor.Current = Cursors.Default;
      }
      catch (Exception ex)
      {
        edtMemo.Text += ex.Message + Environment.NewLine;
        Cursor.Current = Cursors.Default;
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
      e.AutoPopDelay = 25000;
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
      if (!populateListViewStringsInProgress)
        SaveChange();
    }

    /// <summary>
    /// Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkboxNewTextTranslate_CheckedChanged(object sender, EventArgs e)
    {
      if (!populateListViewStringsInProgress)
        SaveChange();
    }

    /// <summary>
    /// SaveChange
    /// </summary>
    private void SaveChange()
    {
      string FormID = txtFormID.Text;
      string RecordType = txtSkyrimRecordType.Text;
      string RecordName = txtSkyrimRecordTypeTH.Text;

      if (String.IsNullOrEmpty(txtFormID.Text)) return;

      DataRowView[] foundRows;
      try
      {
        foundRows = dvPlugIn.FindRows(new object[] { FormID, RecordType, RecordName });

        if (foundRows.Length == 1)
        {
          foundRows[0].BeginEdit();
          foundRows[0].Row["SourceItemDesc"] = txtSourceStringNew.Text;
          foundRows[0].Row["SourceItemDescOld"] = txtSourceStringOld.Text;

          foundRows[0].Row["TargetItemDesc"] = txtTargetStringNew.Text;
          foundRows[0].Row["TargetItemDescOld"] = txtTargetStringOld.Text;

          foundRows[0].Row["WriteStringInPlugIn"] = chkboxNewTextTranslate.Checked;
          foundRows[0].EndEdit();

          lastOLVListItemObjStrings.SourceItemDesc = txtSourceStringNew.Text;
          lastOLVListItemObjStrings.TargetItemDesc = txtTargetStringNew.Text;
          lastOLVListItemObjStrings.WriteStringInPlugIn = chkboxNewTextTranslate.Checked;
          //TESVSnip.Docking.ObjStrings itemObj = (TESVSnip.Docking.ObjStrings)lastOLVListItem.RowObject;
          olvTHStrings.RefreshItem(lastOLVListItem);
        }
      }
      catch (Exception ex)
      {
        edtMemo.Text += ex.Message + Environment.NewLine;
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
      //  e.Handled = e.KeyChar == 13;//block return key because there is no default button
      if (e.KeyChar == 13)
      {
        SaveChange();
        e.Handled = e.KeyChar == 13;
      }
    }

    /// <summary>
    /// Search in list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtSearchInStringsList_TextChanged(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(txtSearchInStringsList.Text))
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
      if (!String.IsNullOrEmpty(txtSearchInDLStringsList.Text))
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
      if (!String.IsNullOrEmpty(txtSearchInILStringsList.Text))
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
      bool check = cbSearchInSkyrimString.CheckBoxItems[0].Checked | cbSearchInSkyrimString.CheckBoxItems[1].Checked | cbSearchInSkyrimString.CheckBoxItems[2].Checked;
      if (!check)
      {
        Cliver.Message.Show("Translator Helper", System.Drawing.SystemIcons.Information, "Check one strings in list.", 0, "OK");
        return;
      }

      if (String.IsNullOrEmpty(txtSearchInSkyrimString.Text))
      {
        Cliver.Message.Show("Translator Helper", System.Drawing.SystemIcons.Information, "No text.", 0, "OK");
        return;
      }

      if (listViewSkyrimDict != null) listViewSkyrimDict.Clear(); else listViewSkyrimDict = new System.Collections.Generic.List<ObjStringsDict>();
      listViewSkyrimDict.Clear();
      olvSkyrimDict.Items.Clear();
      tblSkyrimEsmDict.Rows.Clear();
      GC.Collect();

      if (!LoadSkyrimStringsDictionnary()) return;

      string src;
      string trg;
      string stringType;
      bool strAdded;
      bool typeOK;
      foreach (DataRow row in tblStrings.Rows)
      {
        //if (Convert.ToString(row["StringIDHexa"]) != 0.ToString("X8"))
        //{
        stringType = Convert.ToString(row["StringType"]);
        src = Convert.ToString(row["SourceTextValue"]);
        trg = Convert.ToString(row["TargetTextValue"]);

        typeOK = false;
        check = cbSearchInSkyrimString.CheckBoxItems[0].Checked;
        if (check & stringType.ToUpper() == "STRINGS") typeOK = true;

        check = cbSearchInSkyrimString.CheckBoxItems[1].Checked;
        if (check & stringType.ToUpper() == "DLSTRINGS") typeOK = true;

        check = cbSearchInSkyrimString.CheckBoxItems[2].Checked;
        if (check & stringType.ToUpper() == "ILSTRINGS") typeOK = true;

        if (typeOK)
        {
          strAdded = false;

          if (!String.IsNullOrEmpty(src))
            if (src.IndexOf(txtSearchInSkyrimString.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
              strAdded = true;

          if (!strAdded)
            if (!String.IsNullOrEmpty(trg))
              if (trg.IndexOf(txtSearchInSkyrimString.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                strAdded = true;

          if (strAdded)
          {
            listViewSkyrimDict.Add(new ObjStringsDict(
             Convert.ToString(row["StringIDHexa"]),
             Convert.ToString(row["SourceTextValue"]),
             Convert.ToString(row["TargetTextValue"])
             ));
          }
        }
        //}
      }

      this.olvSkyrimDict.SetObjects(listViewSkyrimDict);
      olvSkyrimDict.ShowGroups = false;
      olvSkyrimDict.BuildList();

      TextMatchFilter filter = TextMatchFilter.Contains(this.olvSkyrimDict, txtSearchInSkyrimString.Text);
      this.olvSkyrimDict.ModelFilter = filter;
      this.olvSkyrimDict.DefaultRenderer = new HighlightTextRenderer(filter);

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
      SaveChange();
    }

    private void olvTHSkyrimTargetStrings_DoubleClick(object sender, EventArgs e)
    {
      OLVListItem item = ((BrightIdeasSoftware.ObjectListView)sender).SelectedItem;
      TESVSnip.Docking.ObjOtherSkyrimStrings itemObj = (TESVSnip.Docking.ObjOtherSkyrimStrings)item.RowObject;
      txtTargetStringOld.Text = txtTargetStringNew.Text;
      txtTargetStringNew.Text = itemObj.SkyrimText;
      SaveChange();
    }

    /// <summary>
    /// olvSkyrimDict_DoubleClick
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void olvSkyrimDict_DoubleClick(object sender, EventArgs e)
    {

      if (lastOLVListItem == null) return;

      OLVListItem item = ((BrightIdeasSoftware.ObjectListView)sender).SelectedItem;
      TESVSnip.Docking.ObjStringsDict itemObj = (TESVSnip.Docking.ObjStringsDict)item.RowObject;
      int colIdx = (((BrightIdeasSoftware.ObjectListView)sender)).HotColumnIndex;
      string colName = (((BrightIdeasSoftware.ObjectListView)sender)).Columns[colIdx].Name;

      if (colName == "olvColSkyrimItemDescTargetLang")
      {
        txtTargetStringOld.Text = txtTargetStringNew.Text;
        txtTargetStringNew.Text = itemObj.TargetString;
        SaveChange();
      }
      else
      {
        txtSourceStringOld.Text = txtSourceStringNew.Text;
        txtSourceStringNew.Text = itemObj.SourceString;
        SaveChange();
      }

    }

    /// <summary>
    /// olvTHStrings Sort
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void olvTHStrings_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      string colName = ((BrightIdeasSoftware.OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns[e.Column])).AspectName;
      olvTHStringsSort(sender, colName);
    }

    private void olvTHStringsSort(object sender, string colName)
    {
      OLVColumn ovlCol = null;

      if (listViewStrings.Count <= 0) { tabPageStrings.Text = "Name - 0 row"; return; }

      if (!String.IsNullOrEmpty(olvTHStringsLastSelectedColumnName))
        if (olvTHStringsLastSelectedColumnName != colName)
        {
          ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + olvTHStringsLastSelectedColumnName + "STR"]);
          ovlCol.HeaderImageKey = "";
        }

      if (listViewStrings.Count > 0)
      {
        if (olvTHStringsLastSortOrder == SortOrder.None)
          olvTHStringsLastSortOrder = SortOrder.Ascending;
        else
          olvTHStringsLastSortOrder = olvTHStringsLastSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;



        CreateListViewColumnTH("Strings", colName, olvTHStringsLastSortOrder);

        olvTHStringsLastSelectedColumnName = colName;
        olvTHStrings.Items.Clear();
        this.olvTHStrings.SetObjects(listViewStrings);
        olvTHStrings.ShowGroups = true;
        olvTHStrings.BuildList();
        ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + colName + "STR"]);
        ovlCol.HeaderImageKey = olvTHStringsLastSortOrder == SortOrder.Descending ? "sort-descend" : "sort-ascend";
        tabPageStrings.Text = "Name - " + listViewStrings.Count.ToString() + " rows";
      }
      else
      {
        tabPageStrings.Text = "Name - 0 row";
      }
    }

    /// <summary>
    /// olvTHDLStrings Sort
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void olvTHDLStrings_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      string colName = ((BrightIdeasSoftware.OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns[e.Column])).AspectName;
      olvTHDLStringsSort(sender, colName);
    }

    private void olvTHDLStringsSort(object sender, string colName)
    {
      OLVColumn ovlCol = null;

      if (listViewStringsDL.Count <= 0) { tabPageDLStrings.Text = "Description - 0 row"; return; }

      if (!String.IsNullOrEmpty(olvTHStringsDLLastSelectedColumnName))
        if (olvTHStringsDLLastSelectedColumnName != colName)
        {
          ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + olvTHStringsDLLastSelectedColumnName + "DL"]);
          ovlCol.HeaderImageKey = "";
        }

      if (listViewStringsDL.Count > 0)
      {
        if (olvTHStringsDLLastSortOrder == SortOrder.None)
          olvTHStringsDLLastSortOrder = SortOrder.Ascending;
        else
          olvTHStringsDLLastSortOrder = olvTHStringsDLLastSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;


        CreateListViewColumnTH("Strings", colName, olvTHStringsDLLastSortOrder);

        olvTHStringsDLLastSelectedColumnName = colName;
        olvTHDLStrings.Items.Clear();
        this.olvTHDLStrings.SetObjects(listViewStringsDL);
        olvTHDLStrings.ShowGroups = true;
        olvTHDLStrings.BuildList();
        ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + colName + "DL"]);
        ovlCol.HeaderImageKey = olvTHStringsDLLastSortOrder == SortOrder.Descending ? "sort-descend" : "sort-ascend";
        tabPageDLStrings.Text = "Description - " + listViewStringsDL.Count.ToString() + " rows";
      }
      else
      {
        tabPageDLStrings.Text = "Description - 0 row";
      }
    }

    /// <summary>
    ///  olvTHILStrings Sort
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void olvTHILStrings_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      string colName = ((BrightIdeasSoftware.OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns[e.Column])).AspectName;
      olvTHILStringsSort(sender, colName);
    }

    private void olvTHILStringsSort(object sender, string colName)
    {
      OLVColumn ovlCol = null;

      if (listViewStringsIL.Count <= 0) { tabPageILStrings.Text = "Text - 0 row"; return; }

      if (!String.IsNullOrEmpty(olvTHStringsILLastSelectedColumnName))
        if (olvTHStringsILLastSelectedColumnName != colName)
        {
          ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + olvTHStringsILLastSelectedColumnName + "IL"]);
          ovlCol.HeaderImageKey = "";
        }

      if (listViewStringsIL.Count > 0)
      {
        if (olvTHStringsILLastSortOrder == SortOrder.None)
          olvTHStringsILLastSortOrder = SortOrder.Ascending;
        else
          olvTHStringsILLastSortOrder = olvTHStringsILLastSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;



        CreateListViewColumnTH("Strings", colName, olvTHStringsILLastSortOrder);

        olvTHStringsILLastSelectedColumnName = colName;
        olvTHDLStrings.Items.Clear();
        this.olvTHDLStrings.SetObjects(listViewStringsIL);
        olvTHDLStrings.ShowGroups = true;
        olvTHDLStrings.BuildList();
        ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + colName + "IL"]);
        ovlCol.HeaderImageKey = olvTHStringsILLastSortOrder == SortOrder.Descending ? "sort-descend" : "sort-ascend";
        tabPageILStrings.Text = "Text - " + listViewStringsIL.Count.ToString() + " rows";
      }
      else
      {
        tabPageILStrings.Text = "Text - 0 row";
      }
    }

    /// <summary>
    ///  olvTHOtherStrings Sort
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void olvTHOtherStrings_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      string colName = ((BrightIdeasSoftware.OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns[e.Column])).AspectName;
      olvTHOtherStringsSort(sender, colName);
    }

    private void olvTHOtherStringsSort(object sender, string colName)
    {
      OLVColumn ovlCol = null;

      if (listViewStringsOther.Count <= 0) { tabPageOther.Text = "Other - 0 row"; return; }

      if (!String.IsNullOrEmpty(olvTHStringsOTHERLastSelectedColumnName))
        if (olvTHStringsOTHERLastSelectedColumnName != colName)
        {
          ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + olvTHStringsOTHERLastSelectedColumnName + "OTHER"]);
          ovlCol.HeaderImageKey = "";
        }

      if (listViewStringsOther.Count > 0)
      {
        if (olvTHStringsOTHERLastSortOrder == SortOrder.None)
          olvTHStringsOTHERLastSortOrder = SortOrder.Ascending;
        else
          olvTHStringsOTHERLastSortOrder = olvTHStringsOTHERLastSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

        CreateListViewColumnTH("Strings", colName, olvTHStringsOTHERLastSortOrder);

        olvTHStringsOTHERLastSelectedColumnName = colName;
        olvTHDLStrings.Items.Clear();
        this.olvTHDLStrings.SetObjects(listViewStringsOther);
        olvTHDLStrings.ShowGroups = true;
        olvTHDLStrings.BuildList();
        ovlCol = (OLVColumn)((((BrightIdeasSoftware.ObjectListView)sender)).Columns["olvCol" + colName + "OTHER"]);
        ovlCol.HeaderImageKey = olvTHStringsOTHERLastSortOrder == SortOrder.Descending ? "sort-descend" : "sort-ascend";
        tabPageOther.Text = "Other - " + listViewStringsOther.Count.ToString() + " rows";
      }
      else
      {
        tabPageOther.Text = "Other - 0 row";
      }
    }

    /// <summary>
    /// btnTranslateSkyrimSrcWithWebEngine_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTranslateSkyrimSrcWithWebEngine_Click(object sender, EventArgs e)
    {
      TESVSnip.Translator.FormTranslation formTranslation = new TESVSnip.Translator.FormTranslation(TESVSnip.Encoding.CP1252.CodePage, "en", "fr", ref translateWithWeb);
      formTranslation.SetTextBox(txtStringSkyrimDescSource.SelectionStart, txtStringSkyrimDescSource.SelectionLength, txtStringSkyrimDescSource.Text);
      formTranslation.TranslateText();
      if (formTranslation.ShowDialog() == DialogResult.OK)
      {
        txtTargetStringNew.Text = formTranslation.GeTextTranslated();
        txtTargetStringNew_Validated(sender, e);
      }
      formTranslation.Close();
      formTranslation.Dispose();
      formTranslation = null;
    }

    /// <summary>
    /// btnTranslatePlugInSrcWithWebEngine_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTranslatePlugInSrcWithWebEngine_Click(object sender, EventArgs e)
    {
      TESVSnip.Translator.FormTranslation formTranslation = new TESVSnip.Translator.FormTranslation(TESVSnip.Encoding.CP1252.CodePage, "en", "fr", ref translateWithWeb);
      formTranslation.SetTextBox(txtSourceStringNew.SelectionStart, txtSourceStringNew.SelectionLength, txtSourceStringNew.Text);
      formTranslation.TranslateText();
      if (formTranslation.ShowDialog() == DialogResult.OK)
      {
        txtTargetStringNew.Text = formTranslation.GeTextTranslated();
        txtTargetStringNew_Validated(sender, e);
      }
      formTranslation.Close();
      formTranslation.Dispose();
      formTranslation = null;
    }


  }

}