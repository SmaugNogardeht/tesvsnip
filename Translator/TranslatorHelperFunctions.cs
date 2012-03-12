﻿using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections.Generic;

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
    /// <summary>
    /// ClearTextBoxControl
    /// </summary>
    private void ClearTextBoxControl(Control control)
    {
      foreach (Control c1 in control.Controls)
        if (c1.GetType().Name == "TextBox")
          ((TextBox)c1).Text = String.Empty;
        else
          ClearTextBoxControl(c1);
    }

    /// <summary>
    /// AddToMemo
    /// </summary>
    /// <param name="textValue"></param>
    public void AddToMemo(string textValue)
    {
      edtMemo.Text += textValue;
    }

    /// <summary>
    /// Create columns for ListView
    /// </summary>
    private void CreateListViewColumn()
    {
      this.olvTHStrings.AllColumns.Clear();
      this.olvTHDLStrings.AllColumns.Clear();
      this.olvTHILStrings.AllColumns.Clear();
      this.olvSkyrimDict.AllColumns.Clear();
      this.olvTHSkyrimSourceStrings.AllColumns.Clear();

      string typeCol = String.Empty;

      System.Collections.Generic.List<OLVColumn> listCol = new System.Collections.Generic.List<OLVColumn>();
      BrightIdeasSoftware.OLVColumn olvCol;
      BrightIdeasSoftware.OLVColumn primarySortColumn;
      BrightIdeasSoftware.OLVColumn secondarySortColumn;

      #region olvTHSkyrimSourceStrings / olvTHSkyrimTargetStrings

      for (int i = 0; i < 2; i++)
      {
        listCol.Clear();

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "StringID";
        olvCol.Text = "ID";
        olvCol.Width = 60;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvColSkyrimSourceStringsIDHexa";
        olvCol.HeaderFormatStyle = headerFormatStyleData2;
        olvCol.TextAlign = HorizontalAlignment.Center;
        olvCol.Sortable = true;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "SkyrimText";
        olvCol.Text = "Text";
        olvCol.Width = 450;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvColSkyrimSourceStringsText";
        olvCol.HeaderFormatStyle = headerFormatStyleData2;
        olvCol.TextAlign = HorizontalAlignment.Left;
        olvCol.Sortable = true;
        listCol.Add(olvCol);
        primarySortColumn = olvCol;

        if (i == 0)
        {
          this.olvTHSkyrimSourceStrings.AllColumns.AddRange(listCol);
          this.olvTHSkyrimSourceStrings.RebuildColumns();
        }

        if (i == 1)
        {
          this.olvTHSkyrimTargetStrings.AllColumns.AddRange(listCol);
          this.olvTHSkyrimTargetStrings.RebuildColumns();
        }
      }
      #endregion

      #region olvSkyrimDict

      listCol.Clear();

      olvCol = new BrightIdeasSoftware.OLVColumn();
      olvCol.AspectName = "StringID";
      olvCol.Text = "ID";
      olvCol.Width = 60;
      olvCol.HeaderTextAlign = HorizontalAlignment.Center;
      olvCol.Groupable = false;
      olvCol.Name = "olvColSkyrimStringIDHexa";
      olvCol.HeaderFormatStyle = headerFormatStyleData;
      olvCol.TextAlign = HorizontalAlignment.Center;
      olvCol.Sortable = true;
      listCol.Add(olvCol);

      olvCol = new BrightIdeasSoftware.OLVColumn();
      olvCol.AspectName = "SourceString";
      olvCol.Text = "Source";
      olvCol.Width = 240;
      olvCol.HeaderTextAlign = HorizontalAlignment.Center;
      olvCol.Groupable = false;
      olvCol.Name = "olvColSkyrimItemDescSourceLang";
      olvCol.HeaderFormatStyle = headerFormatStyleData;
      olvCol.TextAlign = HorizontalAlignment.Left;
      olvCol.Sortable = true;
      listCol.Add(olvCol);
      primarySortColumn = olvCol;

      olvCol = new BrightIdeasSoftware.OLVColumn();
      olvCol.AspectName = "TargetString";
      olvCol.Text = "Target";
      olvCol.Width = 240;
      olvCol.HeaderTextAlign = HorizontalAlignment.Center;
      olvCol.Groupable = false;
      olvCol.Name = "olvColSkyrimItemDescTargetLang";
      olvCol.HeaderFormatStyle = headerFormatStyleData;
      olvCol.TextAlign = HorizontalAlignment.Left;
      olvCol.Sortable = true;
      listCol.Add(olvCol);
      
      this.olvSkyrimDict.AllColumns.AddRange(listCol);
      this.olvSkyrimDict.RebuildColumns();
      this.olvSkyrimDict.PrimarySortColumn = primarySortColumn;

      #endregion

      #region Other list
      
      for (int i = 0; i <= 3; i++)
      {
        listCol.Clear();
        if (i == 1) typeCol = "DL";

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "GroupName";
        olvCol.Text = "Group";
        olvCol.Width = 100;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = true;
        olvCol.Name = "olvColGroup" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Left;
        olvCol.Sortable = true;
        olvCol.Groupable = true;
        listCol.Add(olvCol);
        primarySortColumn = olvCol;

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "StringStatus";
        olvCol.Text = "State";
        olvCol.Width = 50;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = true;
        olvCol.Name = "olvColStringStatus" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Center;
        olvCol.Sortable = true;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "CompareStatusSource";
        olvCol.Text = "Src <>";
        olvCol.Width = 50;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = true;
        olvCol.Name = "olvColCompareStatus" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Center;
        olvCol.Sortable = true;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "CompareStatusTarget";
        olvCol.Text = "Tgt <>";
        olvCol.Width = 50;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = true;
        olvCol.Name = "olvColCompareStatusTarget" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Center;
        olvCol.Sortable = true;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "RecordType";
        olvCol.Text = "Type";
        olvCol.Width = 60;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = true;
        olvCol.Name = "olvColRecordType" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Left;
        olvCol.Sortable = true;
        secondarySortColumn = olvCol;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "EditorID";
        olvCol.Text = "Editor ID";
        olvCol.Width = 200;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvColEditorID" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Left;
        olvCol.Sortable = true;
        listCol.Add(olvCol);
        //secondarySortColumn = olvCol;

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "SourceItemDesc";
        olvCol.Text = "Source Text";
        olvCol.Width = 300;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvColSourceItemDesc" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Left;
        olvCol.Sortable = true;
        secondarySortColumn = olvCol;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "TargetItemDesc";
        olvCol.Text = "Target Text";
        olvCol.Width = 300;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvColTargerItemDesc" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Left;
        olvCol.Sortable = true;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "SourceStringIDHexa";
        olvCol.Text = "String ID";
        olvCol.Width = 70;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvColSourceStringIDHexa" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Center;
        olvCol.Sortable = true;
        olvCol.IsVisible = false;
        listCol.Add(olvCol);

        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "WriteStringInPlugIn";
        olvCol.Text = "Write";
        olvCol.Width = 300;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvWriteStringInPlugIn" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Center;
        olvCol.Sortable = true;
        olvCol.IsVisible = false;
        listCol.Add(olvCol);


        olvCol = new BrightIdeasSoftware.OLVColumn();
        olvCol.AspectName = "FormID";
        olvCol.Text = "Form ID";
        olvCol.Width = 70;
        olvCol.HeaderTextAlign = HorizontalAlignment.Center;
        olvCol.Groupable = false;
        olvCol.Name = "olvColFormID" + typeCol;
        olvCol.HeaderFormatStyle = headerFormatStyleData;
        olvCol.TextAlign = HorizontalAlignment.Center;
        olvCol.Sortable = true;
        olvCol.IsVisible = false;
        listCol.Add(olvCol);

        if (i == 0)
        {
          this.olvTHStrings.AllColumns.AddRange(listCol);
          this.olvTHStrings.RebuildColumns();
          //this.olvTHStrings.AlwaysGroupByColumn = primarySortColumn;
          this.olvTHStrings.PrimarySortColumn = primarySortColumn;
          this.olvTHStrings.SecondarySortColumn = secondarySortColumn;
        }

        if (i == 1)
        {
          this.olvTHDLStrings.AllColumns.AddRange(listCol);
          this.olvTHDLStrings.RebuildColumns();
          this.olvTHDLStrings.PrimarySortColumn = primarySortColumn;
          this.olvTHDLStrings.SecondarySortColumn = secondarySortColumn;
        }

        if (i == 2)
        {
          this.olvTHILStrings.AllColumns.AddRange(listCol);
          this.olvTHILStrings.RebuildColumns();
          this.olvTHILStrings.PrimarySortColumn = primarySortColumn;
          this.olvTHILStrings.SecondarySortColumn = secondarySortColumn;
        }

        if (i == 3)
        {
          this.olvTHOtherStrings.AllColumns.AddRange(listCol);
          this.olvTHOtherStrings.RebuildColumns();
          this.olvTHOtherStrings.PrimarySortColumn = primarySortColumn;
          this.olvTHOtherStrings.SecondarySortColumn = secondarySortColumn;
        }
      }

      #endregion
    }

    /// <summary>
    /// PopulateLanguageComboBox
    /// </summary>
    private void PopulateLanguageComboBox()
    {
      cboxSourceLanguage.Items.Clear();
      cboxTargetLanguage.Items.Clear();

      cboxSourceLanguage.Items.Add("English"); cboxTargetLanguage.Items.Add("English");
      cboxSourceLanguage.Items.Add("Czech"); cboxTargetLanguage.Items.Add("Czech");
      cboxSourceLanguage.Items.Add("French"); cboxTargetLanguage.Items.Add("French");
      cboxSourceLanguage.Items.Add("German"); cboxTargetLanguage.Items.Add("German");
      cboxSourceLanguage.Items.Add("Italian"); cboxTargetLanguage.Items.Add("Italian");
      cboxSourceLanguage.Items.Add("Spanish"); cboxTargetLanguage.Items.Add("Spanish");
      cboxSourceLanguage.Items.Add("Russian"); cboxTargetLanguage.Items.Add("Russian");
      cboxSourceLanguage.Items.Add("Polish"); cboxTargetLanguage.Items.Add("Polish");
      cboxSourceLanguage.Items.Add("Japanese"); cboxTargetLanguage.Items.Add("Japanese");

      cboxSourceLanguage.SelectedIndex = 0;
      cboxTargetLanguage.SelectedIndex = 2;
    }

    /// <summary>
    /// PopulateListViewStrings
    /// </summary>
    private void PopulateListViewStrings()
    {
      populateListViewStringsInProgress = true;

      if (listViewStrings != null) { listViewStrings.Clear(); listViewStrings = null; }
      if (listViewStringsDL != null) { listViewStringsDL.Clear(); listViewStringsDL = null; }
      if (listViewStringsIL != null) { listViewStringsIL.Clear(); listViewStringsIL = null; }
      if (listViewStringsOther != null) { listViewStringsOther.Clear(); listViewStringsOther = null; }

      //if (listViewSkyrimDict != null) { listViewSkyrimDict.Clear(); listViewSkyrimDict = null; }

      if (listViewOtherSkyrimStringsSource != null) { listViewOtherSkyrimStringsSource.Clear(); listViewOtherSkyrimStringsSource = null; }
      if (listViewOtherSkyrimStringsTarget != null) { listViewOtherSkyrimStringsTarget.Clear(); listViewOtherSkyrimStringsTarget = null; }

      GC.Collect();

      listViewStrings = new System.Collections.Generic.List<ObjStrings>();
      listViewStringsDL = new System.Collections.Generic.List<ObjStrings>();
      listViewStringsIL = new System.Collections.Generic.List<ObjStrings>();
      listViewStringsOther = new System.Collections.Generic.List<ObjStrings>();

      //listViewSkyrimDict = new System.Collections.Generic.List<ObjStringsDict>();

      listViewOtherSkyrimStringsSource = new System.Collections.Generic.List<ObjOtherSkyrimStrings>();
      listViewOtherSkyrimStringsTarget = new System.Collections.Generic.List<ObjOtherSkyrimStrings>();

      foreach (DataRow row in tblPlugInStringsProject.Rows)
      {
        if (Convert.ToString(row["StringType"]) == "Strings")
        {
          listViewStrings.Add(new ObjStrings(
           Convert.ToString(row["GroupName"]),
           Convert.ToString(row["StringStatus"]),
           Convert.ToString(row["CompareStatusSource"]),
           Convert.ToString(row["CompareStatusTarget"]),
           Convert.ToString(row["RecordTypeTH"]),
           Convert.ToString(row["FormIDHexa"]),
           Convert.ToString(row["EditorID"]),
           Convert.ToString(row["SourceStringIDHexa"]),
           Convert.ToString(row["SourceItemDesc"]),
           Convert.ToString(row["TargerItemDesc"]),
           Convert.ToBoolean(row["WriteStringInPlugIn"])
           ));
        }

        if (Convert.ToString(row["StringType"]) == "DLStrings")
        {
          listViewStringsDL.Add(new ObjStrings(
           Convert.ToString(row["GroupName"]),
           Convert.ToString(row["StringStatus"]),
           Convert.ToString(row["CompareStatusSource"]),
           Convert.ToString(row["CompareStatusTarget"]),
           Convert.ToString(row["RecordTypeTH"]),
           Convert.ToString(row["FormIDHexa"]),
           Convert.ToString(row["EditorID"]),
           Convert.ToString(row["SourceStringIDHexa"]),
           Convert.ToString(row["SourceItemDesc"]),
           Convert.ToString(row["TargerItemDesc"]),
           Convert.ToBoolean(row["WriteStringInPlugIn"])
           ));
        }

        if (Convert.ToString(row["StringType"]) == "ILStrings")
        {
          listViewStringsIL.Add(new ObjStrings(
           Convert.ToString(row["GroupName"]),
           Convert.ToString(row["StringStatus"]),
           Convert.ToString(row["CompareStatusSource"]),
           Convert.ToString(row["CompareStatusTarget"]),
           Convert.ToString(row["RecordTypeTH"]),
           Convert.ToString(row["FormIDHexa"]),
           Convert.ToString(row["EditorID"]),
           Convert.ToString(row["SourceStringIDHexa"]),
           Convert.ToString(row["SourceItemDesc"]),
           Convert.ToString(row["TargerItemDesc"]),
           Convert.ToBoolean(row["WriteStringInPlugIn"])
           ));
        }

        if (Convert.ToString(row["StringType"]) == "OtherStrings")
        {
          listViewStringsOther.Add(new ObjStrings(
           Convert.ToString(row["GroupName"]),
           Convert.ToString(row["StringStatus"]),
           Convert.ToString(row["CompareStatusSource"]),
           Convert.ToString(row["CompareStatusTarget"]),
           Convert.ToString(row["RecordTypeTH"]),
           Convert.ToString(row["FormIDHexa"]),
           Convert.ToString(row["EditorID"]),
           Convert.ToString(row["SourceStringIDHexa"]),
           Convert.ToString(row["SourceItemDesc"]),
           Convert.ToString(row["TargerItemDesc"]),
           Convert.ToBoolean(row["WriteStringInPlugIn"])
           ));
        }
      }

      if (listViewStrings.Count > 0)
      {
        olvTHStrings.Items.Clear();
        this.olvTHStrings.SetObjects(listViewStrings);
        olvTHStrings.ShowGroups = true;
        olvTHStrings.BuildList();
        tabPageStrings.Text = "Name - " + listViewStrings.Count.ToString() + " rows";
      }
      else
        tabPageStrings.Text = "Name - 0 row";

      if (listViewStringsDL.Count > 0)
      {
        olvTHDLStrings.Items.Clear();
        this.olvTHDLStrings.SetObjects(listViewStringsDL);
        olvTHDLStrings.ShowGroups = true;
        olvTHDLStrings.BuildList();
        tabPageDLStrings.Text = "Description - " + listViewStringsDL.Count.ToString() + " rows";
      }
      else
        tabPageDLStrings.Text = "Description - 0 row";
      
      if (listViewStringsIL.Count > 0)
      {
        olvTHILStrings.Items.Clear();
        this.olvTHILStrings.SetObjects(listViewStringsIL);
        olvTHILStrings.ShowGroups = true;
        olvTHILStrings.BuildList();
        tabPageILStrings.Text = "Text - " + listViewStringsIL.Count.ToString() + " rows";
      }
      else
        tabPageILStrings.Text = "Text - 0 row";

      if (listViewStringsOther.Count > 0)
      {
        olvTHOtherStrings.Items.Clear();
        this.olvTHOtherStrings.SetObjects(listViewStringsOther);
        olvTHOtherStrings.ShowGroups = true;
        olvTHOtherStrings.BuildList();
        tabPageOther.Text = "Other - " + listViewStringsOther.Count.ToString() + " rows";
      }
      else
        tabPageOther.Text = "Other - 0 row";

      LoadSkyrimEsmDictionnary();

      populateListViewStringsInProgress = false;
    }

    /// <summary>
    /// GetDefaultStringType
    /// Get default string type. By default the most common type in strings Skyrim
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    private string GetDefaultStringType(string recordName)
    {
      string returnValue = "OtherStrings";
      switch (recordName)
      {
        case "ALFD":
          returnValue = "ILStrings";
          break;
        case "BPTN":
          returnValue = "Strings";
          break;
        case "CNAM":
          returnValue = "DLStrings";
          break;
        case "DATA":
          returnValue = "Strings";
          break;
        case "DESC":
          returnValue = "DLStrings";
          break;
        case "DNAM":
          returnValue = "Strings";
          break;
        case "EPF2":
          returnValue = "Strings";
          break;
        case "EPFD":
          returnValue = "Strings";
          break;
        case "FNAM":
          returnValue = "Strings";
          break;
        case "FULL":
          returnValue = "Strings";
          break;
        case "ITXT":
          returnValue = "Strings";
          break;
        case "MNAM":
          returnValue = "Strings";
          break;
        case "PFO2":
          returnValue = "Strings";
          break;
        case "RDMP":
          returnValue = "Strings";
          break;
        case "RNAM":
          returnValue = "Strings";
          break;
        case "RPLI":
          returnValue = "ILStrings";
          break;
        case "SHRT":
          returnValue = "Strings";
          break;
        case "TNAM":
          returnValue = "Strings";
          break;
        case "WNAM":
          returnValue = "ILStrings";
          break;
        default:
          returnValue = "OtherStrings";
          break;
      }
      return returnValue;
    }

    /// <summary>
    /// FindOtherPossibleStringTranslation
    /// In some cases, thera are identical name in sub-records without string id and there are more possibility for on combination of FormID/SubRecord Name and
    /// i can identify the good string
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    //listViewOtherSkyrimStringsTarget

    //  if (listViewStringsOther.Count > 0)
    //  {
    //    olvTHOtherStrings.Items.Clear();
    //    this.olvTHOtherStrings.SetObjects(listViewStringsOther);
    //    olvTHOtherStrings.ShowGroups = true;
    //    olvTHOtherStrings.BuildList();
    //    tabPageOther.Text = "Other - " + listViewStringsOther.Count.ToString() + " rows";
    //  }
    //  else
    //    tabPageOther.Text = "Other - 0 row";
    private void FindOtherPossibleStringTranslation()
    {
      listViewOtherSkyrimStringsSource.Clear();
      listViewOtherSkyrimStringsTarget.Clear();
      olvTHSkyrimSourceStrings.Items.Clear();
      olvTHSkyrimTargetStrings.Items.Clear();

      if (listViewSkyrimDict.Count <= 0) return;

      DataRowView[] foundRowsDict = null;
      DataView dvSkyrimEsmDict = new DataView();
      Dictionary<string, string> dictionary = new Dictionary<string, string>();

      dvSkyrimEsmDict.Table = tblSkyrimEsmDict;

      for (int countSearch = 1; countSearch <= 2; countSearch++)
      {
       
        //Look in the dictionary
        if (countSearch == 1)
        {
          dvSkyrimEsmDict.Sort = "FormIDHexa";
          foundRowsDict = dvSkyrimEsmDict.FindRows(new object[] { txtFormID.Text });
        }

        if (countSearch == 2)
        {
          dvSkyrimEsmDict.Sort = "SkyrimStringIDHexa";
          foundRowsDict = dvSkyrimEsmDict.FindRows(new object[] { txtSourceStringsID.Text });
        }

        //if (foundRowsDict.Length == 0)
        //  foundRowsDict = dvSkyrimEsmDict.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]) });
        //if (foundRowsDict.Length == 0)
        //  foundRowsDict = dvSkyrimEsmDictByRecordTypeStringIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["RecordType"]), Convert.ToString(rowPlugInLoad["SourceStringIDHexa"]) });
        //if (foundRowsDict.Length == 0)
          //foundRowsDict = dvSkyrimEsmDictByStringIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["SourceStringIDHexa"]) });


        if (foundRowsDict.Length > 0)
        {
          foreach (DataRowView row in foundRowsDict)
          {
            if (!dictionary.ContainsKey(txtFormID.Text))
            {
              dictionary.Add(txtFormID.Text, txtSourceStringsID.Text);

              listViewOtherSkyrimStringsSource.Add(new ObjOtherSkyrimStrings(
                 Convert.ToString(row["SkyrimStringIDHexa"]),
                 Convert.ToString(row["SkyrimItemDescSourceLang"])));

              listViewOtherSkyrimStringsTarget.Add(new ObjOtherSkyrimStrings(
                 Convert.ToString(row["SkyrimStringIDHexa"]),
                 Convert.ToString(row["SkyrimItemDescTargetLang"])));
            }
          }

        }
      }

      dictionary.Clear();
      dictionary = null;

      if (listViewOtherSkyrimStringsSource.Count > 0)
      {
        this.olvTHSkyrimSourceStrings.SetObjects(listViewOtherSkyrimStringsSource);
        olvTHSkyrimSourceStrings.ShowGroups = true;
        olvTHSkyrimSourceStrings.BuildList();
      }

      if (listViewOtherSkyrimStringsTarget.Count > 0)
      {
        this.olvTHSkyrimTargetStrings.SetObjects(listViewOtherSkyrimStringsTarget);
        olvTHSkyrimTargetStrings.ShowGroups = true;
        olvTHSkyrimTargetStrings.BuildList();
      }

      dvSkyrimEsmDict.Dispose();
      dvSkyrimEsmDict = null;

    }

  }
}