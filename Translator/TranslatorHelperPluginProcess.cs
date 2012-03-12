﻿using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;

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
    private int totalRecordsWritingToplugIn;

    /// <summary>
    /// CompareInProjectSkyrimStringsWithPluginStringsSourceAndTarget
    /// </summary>
    public void CompareInProjectSkyrimStringsWithPluginStringsSourceAndTarget()
    {
      DataRow row;
      string sourceDiff = String.Empty;
      string targetDiff = String.Empty;

      for (int i = 0; i < tblPlugInStringsProject.Rows.Count; i++)
      {
        sourceDiff = String.Empty;
        targetDiff = String.Empty;

        row = tblPlugInStringsProject.Rows[i];
        row.BeginEdit();

        //if (Convert.ToString(row["SourceItemDesc"]) == "Amulet of Kynareth")

        if (Convert.ToString(row["SkyrimItemDescSourceLang"]) != Convert.ToString(row["SourceItemDesc"])) sourceDiff = "S";
        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["SourceItemDescOld"])))
          if (Convert.ToString(row["SourceItemDesc"]) != Convert.ToString(row["SourceItemDescOld"]))
            if (String.IsNullOrWhiteSpace(sourceDiff))
              sourceDiff = "M";
            else
              sourceDiff += "/M";

        if (Convert.ToString(row["SkyrimItemDescTargetLang"]) != Convert.ToString(row["TargerItemDesc"])) targetDiff = "S";
        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["TargerItemDescOld"])))
          if (Convert.ToString(row["TargerItemDesc"]) != Convert.ToString(row["TargerItemDescOld"]))
            if (String.IsNullOrWhiteSpace(targetDiff))
              targetDiff = "M";
            else
              targetDiff += "/M";

        row["CompareStatusSource"] = sourceDiff;
        row["CompareStatusTarget"] = targetDiff;
        row.EndEdit();
      }

      PopulateListViewStrings();
    }

    /// <summary>
    /// UpdateProjectStrings
    /// </summary>
    private void UpdateProjectStringsFromSkyrim(bool onlyTargetStrings)
    {
      if (!LoadSkyrimEsmDictionnary()) return;
      if (tblPlugInStringsLoad.Rows.Count == 0)
      {
        MessageBox.Show("Mod is empty!!!", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }

      DataRow rowProject;
      DataRowView[] foundRows;
      DataRowView[] foundRowsDict;
      string itemDesc;
      string stringID;
      int countRowsDict;
      int countRows;

      try
      {
        DataView dvSkyrimEsmDictByFormIdStringType = new DataView();
        dvSkyrimEsmDictByFormIdStringType.Table = tblSkyrimEsmDict;
        dvSkyrimEsmDictByFormIdStringType.Sort = "FormIDHexa, StringType";

        DataView dvSkyrimEsmDictByFormIDHexa = new DataView();
        dvSkyrimEsmDictByFormIDHexa.Table = tblSkyrimEsmDict;
        dvSkyrimEsmDictByFormIDHexa.Sort = "FormIDHexa";

        DataView dvSkyrimEsmDictByRecordTypeStringIDHexa = new DataView();
        dvSkyrimEsmDictByRecordTypeStringIDHexa.Table = tblSkyrimEsmDict;
        dvSkyrimEsmDictByRecordTypeStringIDHexa.Sort = "RecordType, SkyrimStringIDHexa";

        DataView dvSkyrimEsmDictByStringIDHexa = new DataView();
        dvSkyrimEsmDictByStringIDHexa.Table = tblSkyrimEsmDict;
        dvSkyrimEsmDictByStringIDHexa.Sort = "SkyrimStringIDHexa";

        DataView dvPlugInByEditorID = new DataView();
        dvPlugInByEditorID.Table = tblPlugInStringsProject;
        dvPlugInByEditorID.Sort = "EditorID, RecordTypeTH";  // "EditorID, StringType, RecordTypeTH";

        DataView dvPlugInByFormIDHexa = new DataView();
        dvPlugInByFormIDHexa.Table = tblPlugInStringsProject;
        dvPlugInByFormIDHexa.Sort = "FormIDHexa, RecordTypeTH"; ; // "FormIDHexa, StringType, RecordTypeTH";

        foreach (DataRow row in tblPlugInStringsProject.Rows)
        {
          row.BeginEdit();
          if (onlyTargetStrings)
            row["StringStatus"] = "=";
          else
            row["StringStatus"] = "Del";
          row.EndEdit();
        }

        //bool translateRecord;

        foreach (DataRow rowPlugInLoad in tblPlugInStringsLoad.Rows)
        {
          //if (Convert.ToString(rowPlugInLoad["EditorID"]) == "TGAmuletofArticulation07")
          //  translateRecord = true;

          //foundRows = dvPlugInFormIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]), Convert.ToString(rowPlugInLoad["StringType"]), Convert.ToString(rowPlugInLoad["RecordTypeTH"]) });
          foundRows = dvPlugInByFormIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]), Convert.ToString(rowPlugInLoad["RecordTypeTH"]) });
          countRows = foundRows.Length;
          //if (foundRows.Length == 0)
          //  foundRows = dvPlugInByEditorID.FindRows(new object[] { Convert.ToString(rowPlugInLoad["EditorID"]), Convert.ToString(rowPlugInLoad["RecordTypeTH"]) });
            //foundRows = dvPlugInEditorID.FindRows(new object[] { Convert.ToString(rowPlugInLoad["EditorID"]), Convert.ToString(rowPlugInLoad["StringType"]), Convert.ToString(rowPlugInLoad["RecordTypeTH"]) });

          #region Create new entry
          if ((!onlyTargetStrings) & (foundRows.Length == 0))
          {
            //foundRowsDict = dvSkyrimEsmDictFormIdStringType.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]), Convert.ToString(rowPlugInLoad["StringType"]) });
            //if (foundRowsDict.Length > 0)
            itemDesc = Convert.ToString(rowPlugInLoad["SourceItemDesc"]);
            stringID = Convert.ToString(rowPlugInLoad["SourceStringIDHexa"]);

            //countRowsDict = 1; //test if subrecord if used like a string or like parameter: condition is stringID is not null and stringID not found in skyrim and itemDesc is null 
            //if (!String.IsNullOrWhiteSpace(stringID) & String.IsNullOrWhiteSpace(itemDesc))
            //{
            //  foundRowsDict = dvSkyrimEsmDictByFormIdStringType.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]), Convert.ToString(rowPlugInLoad["StringType"]) });
            //  countRowsDict = foundRowsDict.Length;
            //}

            if (countRows == 0)
            {
              /// New rows
              rowProject = tblPlugInStringsProject.NewRow();
              rowProject["GroupName"] = rowPlugInLoad["GroupName"];
              rowProject["RecordType"] = rowPlugInLoad["RecordType"];
              rowProject["RecordTypeTH"] = rowPlugInLoad["RecordTypeTH"];
              rowProject["StringType"] = rowPlugInLoad["StringType"];
              rowProject["FormID"] = rowPlugInLoad["FormID"];
              rowProject["FormIDHexa"] = rowPlugInLoad["FormIDHexa"];
              rowProject["SourceEditorID"] = rowPlugInLoad["EditorID"];
              rowProject["TargetEditorID"] = rowPlugInLoad["EditorID"];
              rowProject["SourceStringID"] = rowPlugInLoad["SourceStringID"];
              rowProject["SourceStringIDHexa"] = rowPlugInLoad["SourceStringIDHexa"];
              rowProject["SourceItemDesc"] = rowPlugInLoad["SourceItemDesc"];
              rowProject["StringStatus"] = "New";
              rowProject["WriteStringInPlugIn"] = true;
              if (Convert.ToString(rowPlugInLoad["SourceItemDesc"]) == "...") rowProject["WriteStringInPlugIn"] = false;
              if (Convert.ToString(rowPlugInLoad["SourceItemDesc"]).Trim() == "" && Convert.ToString(rowPlugInLoad["SourceStringIDHexa"]) == "00000000") rowProject["WriteStringInPlugIn"] = false;
              if (Convert.ToString(rowPlugInLoad["EditorID"]) == "UBGDetectQuest")
                rowProject["WriteStringInPlugIn"] = false;

              tblPlugInStringsProject.Rows.Add(rowProject);

              foundRows = dvPlugInByFormIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]), Convert.ToString(rowPlugInLoad["RecordTypeTH"]) });
              countRows = foundRows.Length;
            }
            //if (foundRows.Length == 0)
            //  foundRows = dvPlugInEditorID.FindRows(new object[] { Convert.ToString(rowPlugInLoad["EditorID"]), Convert.ToString(rowPlugInLoad["RecordTypeTH"]) });
          }
          #endregion Create new entry

          #region Update entry
          if (countRows > 0) //always true - if false debug because big problem
          {
            foreach (DataRowView foundRow in foundRows)
            {
              //Look in the dictionary
              foundRowsDict = dvSkyrimEsmDictByFormIdStringType.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]), Convert.ToString(rowPlugInLoad["StringType"]) });
              if (foundRowsDict.Length == 0)
                foundRowsDict = dvSkyrimEsmDictByFormIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["FormIDHexa"]) });
              if (foundRowsDict.Length == 0)
                foundRowsDict = dvSkyrimEsmDictByRecordTypeStringIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["RecordType"]), Convert.ToString(rowPlugInLoad["SourceStringIDHexa"]) });
              if (foundRowsDict.Length == 0)
                foundRowsDict = dvSkyrimEsmDictByStringIDHexa.FindRows(new object[] { Convert.ToString(rowPlugInLoad["SourceStringIDHexa"]) });
              
              if (foundRowsDict.Length > 0)
              {
                //Find in dictionnary
                foundRow.BeginEdit();

                if (!onlyTargetStrings)
                {
                  foundRow["GroupName"] = foundRowsDict[0]["GroupName"];
                  foundRow["EditorID"] = rowPlugInLoad["EditorID"]; // foundRowsDict[0]["EditorID"];
                  foundRow["SkyrimStringID"] = foundRowsDict[0]["SkyrimStringID"];
                  foundRow["SkyrimStringIDHexa"] = foundRowsDict[0]["SkyrimStringIDHexa"];
                  foundRow["SkyrimItemDescSourceLang"] = foundRowsDict[0]["SkyrimItemDescSourceLang"];
                  foundRow["SkyrimItemDescTargetLang"] = foundRowsDict[0]["SkyrimItemDescTargetLang"];
                }

                if (Convert.ToString(foundRow["StringStatus"]) == "New") //not concerned by onlyTargetStrings because i already test it before
                {
                  foundRow["TargetStringID"] = rowPlugInLoad["SourceStringID"];
                  foundRow["TargetStringIDHexa"] = rowPlugInLoad["SourceStringIDHexa"];
                  foundRow["TargerItemDesc"] = foundRowsDict[0]["SkyrimItemDescTargetLang"];
                  foundRow["TargerItemDescOld"] = String.Empty;
                  foundRow["TargetStringID"] = rowPlugInLoad["SourceStringID"];
                }
                else
                {
                  foundRow["StringStatus"] = "=";

                  if (onlyTargetStrings)
                  {
                    //compare only translated strings
                    if (Convert.ToString(foundRow["SourceItemDesc"]) != Convert.ToString(rowPlugInLoad["SourceItemDesc"]))
                    {
                      foundRow["TargerItemDescOld"] = foundRow["TargerItemDesc"];
                      foundRow["TargerItemDesc"] = rowPlugInLoad["SourceItemDesc"];
                      foundRow["StringStatus"] = "Upd";
                    }
                  }
                  else
                  {
                    if (Convert.ToString(foundRow["SourceItemDesc"]) != Convert.ToString(rowPlugInLoad["SourceItemDesc"]))
                    {
                      foundRow["SourceStringID"] = rowPlugInLoad["SourceStringID"];
                      foundRow["SourceStringIDHexa"] = rowPlugInLoad["SourceStringIDHexa"];
                      foundRow["SourceItemDescOld"] = foundRow["SourceItemDesc"];
                      foundRow["SourceItemDesc"] = rowPlugInLoad["SourceItemDesc"];
                      foundRow["TargerItemDescOld"] = foundRow["TargerItemDesc"];
                      foundRow["StringStatus"] = "Upd";
                    }
                  }
                }

                foundRow.EndEdit();
              }//Find in dictionnary
              else
              {
                //not found in dictionnary
                //perhaps special record create only from the plugin

                foundRow.BeginEdit();

                foundRow["EditorID"] = Convert.ToString(rowPlugInLoad["EditorID"]);
                foundRow["SkyrimStringID"] = 0;
                foundRow["SkyrimStringIDHexa"] = 0.ToString("x8").ToUpperInvariant();
                foundRow["SkyrimItemDescSourceLang"] = String.Empty;
                foundRow["SkyrimItemDescTargetLang"] = String.Empty;
                //foundRow["StringType"] = "OtherStrings";

                if (Convert.ToString(foundRow["StringStatus"]) == "New")
                {
                  foundRow["TargetStringID"] = rowPlugInLoad["SourceStringID"];
                  foundRow["TargetStringIDHexa"] = rowPlugInLoad["SourceStringIDHexa"];
                  foundRow["TargerItemDesc"] = rowPlugInLoad["SourceItemDesc"];
                  foundRow["TargerItemDescOld"] = String.Empty;
                  foundRow["TargetStringID"] = rowPlugInLoad["SourceStringID"];
                }
                else
                {
                  foundRow["StringStatus"] = "=";
                  if (Convert.ToString(foundRow["SourceItemDesc"]) != Convert.ToString(rowPlugInLoad["SourceItemDesc"]))
                  {
                    foundRow["SourceStringID"] = rowPlugInLoad["SourceStringID"];
                    foundRow["SourceStringIDHexa"] = rowPlugInLoad["SourceStringIDHexa"];
                    foundRow["SourceItemDescOld"] = foundRow["SourceItemDesc"];
                    foundRow["SourceItemDesc"] = rowPlugInLoad["SourceItemDesc"];
                    foundRow["TargerItemDescOld"] = foundRow["TargerItemDesc"];
                    foundRow["StringStatus"] = "Upd";
                  }
                }

                foundRow.EndEdit();

                //foundRow["StringStatus"] = "?";
              }


            }
          }
          #endregion
        }

        dvSkyrimEsmDictByFormIdStringType.Dispose();
        dvSkyrimEsmDictByFormIdStringType = null;
        dvPlugInByEditorID.Dispose();
        dvPlugInByEditorID = null;
        dvPlugInByFormIDHexa.Dispose();
        dvPlugInByFormIDHexa = null;
        dvSkyrimEsmDictByFormIDHexa.Dispose();
        dvSkyrimEsmDictByFormIDHexa = null;
        dvSkyrimEsmDictByRecordTypeStringIDHexa.Dispose();
        dvSkyrimEsmDictByRecordTypeStringIDHexa = null;
        dvSkyrimEsmDictByStringIDHexa.Dispose();
        dvSkyrimEsmDictByStringIDHexa = null;
        GC.Collect();

        dvPlugIn.Table = tblPlugInStringsProject;
        dvPlugIn.Sort = "FormIDHexa, StringType, RecordTypeTH";
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
    }

    /// <summary>
    /// ProcessPlugIn
    /// </summary>
    public void ProcessPlugIn(bool onlyReadRecord, bool filtersRecords, bool onlyTargetStrings, string pluginPath)
    {
      bool newPlugIn = true;
      int answer_index;

      Cursor.Current = Cursors.WaitCursor;
      edtMemo.Text = String.Empty;
      Update();

      try
      {
        if (!String.IsNullOrWhiteSpace(pluginPath)) PluginLocation = pluginPath;
        if (!onlyReadRecord)
        {
          if (!String.IsNullOrEmpty(edtPlugInName.Text))
          {
            if (edtPlugInName.Text != PluginTree.GetFileName())
            {
              answer_index = Cliver.Message.Show("Translator Helper", System.Drawing.SystemIcons.Question,
                "Your are selected a different PlugIn. Do you confirm your choice ?", 1, "I confirme", "I abort");
              if (answer_index == 1) return;
            }

            answer_index = Cliver.Message.Show("Translator Helper", System.Drawing.SystemIcons.Question,
              "Would you like to update the current project with this plugin ?", 0, "Yes, I update", "No, I create");
            if (answer_index == 1)
              newPlugIn = true;
            else
              newPlugIn = false;
          }

          if (newPlugIn) ClearProject();
          ReadAndUpdatePlugInHeader();
        }

        ReadPlugInGroup(onlyReadRecord, filtersRecords, onlyTargetStrings);

        if (!onlyReadRecord)
        {
          UpdateProjectStringsFromSkyrim(onlyTargetStrings);
          CompareInProjectSkyrimStringsWithPluginStringsSourceAndTarget();
          PopulateListViewStrings();
          edtPluginInfo.Text = PluginTree.GetDesc() + Environment.NewLine + "Total rows: " + tblPlugInStringsProject.Rows.Count.ToString();
        }
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    /// <summary>
    /// ReadPlugHeader
    /// Read plugin header
    /// </summary>
    public void ReadAndUpdatePlugInHeader()
    {
      try
      {
        if (PluginTree == null)
        {
          edtMemo.Text += Environment.NewLine + "Load/Select PlugIn first !!";
          MessageBox.Show("Load/Select PlugIn first !!", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        TESVSnip.Record rec = (TESVSnip.Record)PluginTree.Records[0];

        foreach (TESVSnip.SubRecord sr in rec.SubRecords)
          if (sr.DescriptiveName == "CNAM")
          {
            edtPlugInAuthor.Text = sr.GetStrData();
            break;
          }

        tblPlugInHeader.Rows.Clear();
        DataRow row = tblPlugInHeader.NewRow();
        row["PlugInName"] = PluginTree.DescriptiveName;
        row["Author"] = edtPlugInAuthor.Text;
        row["PluginLocation"] = PluginLocation;
        row["PluginInfo"] = PluginTree.GetDesc();
        row["ProjectStructureVersion"] = 1;
        tblPlugInHeader.Rows.Add(row);

        edtPlugInName.Text = PluginTree.DescriptiveName;
        edtPluginInfo.Text = PluginTree.GetDesc();
        //  PluginTree.DescriptiveName;
        //string pluginDesc = PluginTree.GetDesc();
        ////string[] arrDesc = pluginDesc.Split(Environment.NewLine, StringSplitOptions.None);
        //string[] lines = Regex.Split(pluginDesc, "\r\n");
        //edtPluginInfo.Text = lines[0] + Environment.NewLine + lines[1] + Environment.NewLine + lines[2];
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugHeader ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
    }

    /// <summary>
    /// ReadPlugInRecords
    /// </summary>
    private void ReadPlugInGroup(bool onlyReadRecord, bool filtersRecords, bool onlyTargetStrings)
    {
      TESVSnip.GroupRecord groupRec;

      try
      {
        tblPlugInStringsLoad.Rows.Clear();

        for (int groupCounter = 1; groupCounter < PluginTree.Records.Count; groupCounter++)
        {
          groupRec = (TESVSnip.GroupRecord)PluginTree.Records[groupCounter];
          ReadWritePlugInRecordsSubRecords(ref  groupRec, onlyReadRecord, filtersRecords, onlyTargetStrings, false);
        }
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
    }

    /// <summary>
    /// ReadPlugInRecordsSubRecords
    /// </summary>
    /// <param name="groupRec"></param>
    /// <param name="onlyReadRecord"></param>
    /// <param name="filtersRecords"></param>
    /// <param name="onlyTargetStrings"></param>
    private void ReadWritePlugInRecordsSubRecords(ref TESVSnip.GroupRecord groupRec, bool onlyReadRecord, bool filtersRecords, bool onlyTargetStrings,bool updateStringsPlugIn)
    {
      TESVSnip.Record record;
      DataRow row;
      bool translateRecord = false;
      bool continueProcessString = false;

      string groupName;
      string editorID = String.Empty;
      string itemDesc = String.Empty;
      string stringID = String.Empty;
      string recType = String.Empty;
      string recTypeTH = String.Empty;
      string stringType = String.Empty;
      string formIDHexa = String.Empty;
      TESVSnip.ElementValueType valueType;
      TESVSnip.GroupRecord groupRecRecursive;

      int countUpdatedRecords = 0;
      DataView dvPlugInFormIDHexa = null;
      DataRowView[] foundRows;

      //Counter for duplicate subrecord in same item
      int subRecord_ITXT_Counter = 0;
      int subRecord_NNAM_Counter = 0;
      int subRecord_CNAM_Counter = 0;
      int subRecord_DNAM_Counter = 0;
      int subRecord_TNAM_Counter = 0;
      int subRecord_PFO2_Counter = 0;
      int subRecord_ALFD_Counter = 0;
      int subRecord_FNAM_ounter = 0;
      int subRecord_WNAM_Counter = 0;
      int subRecord_BPTN_Counter = 0;
      int subRecord_EPF2_Counter = 0;
      int subRecord_RPLI_Counter = 0;
      int subRecord_PNAM_Counter = 0;

      int pos = groupRec.DescriptiveName.IndexOf("(");
      if (pos > 1)
        groupName = groupRec.DescriptiveName.Substring(pos).Trim().Replace("(", "").Replace(")", "");
      else
        groupName = groupRec.DescriptiveName;
      //edtMemo.Text += Environment.NewLine + groupName;

      if (updateStringsPlugIn)
      {
        dvPlugInFormIDHexa = new DataView();
        dvPlugInFormIDHexa.Table = tblPlugInStringsProject;
        dvPlugInFormIDHexa.Sort = "GroupName, FormIDHexa, RecordTypeTH";
      }

      try
      {
        for (int recordCounter = 0; recordCounter < groupRec.Records.Count; recordCounter++)
        {
          if (groupRec.Records[recordCounter].GetType().FullName == "TESVSnip.GroupRecord")
          {
            groupRecRecursive = (TESVSnip.GroupRecord)groupRec.Records[recordCounter];
            ReadWritePlugInRecordsSubRecords(ref groupRecRecursive, onlyReadRecord, filtersRecords, onlyTargetStrings, updateStringsPlugIn);
          }
          if (groupRec.Records[recordCounter].GetType().FullName == "TESVSnip.Record") //test if TESVSnip record
          {
            record = (TESVSnip.Record)groupRec.Records[recordCounter];
            this.MainViewTH.UpdateMainTextForTH(record);
            bool matchRecordOK = record.MatchRecordStructureToRecord();

            itemDesc = String.Empty;
            stringID = String.Empty;
            subRecord_ITXT_Counter = 0;
            subRecord_NNAM_Counter = 0;
            subRecord_CNAM_Counter = 0;
            subRecord_DNAM_Counter = 0;
            subRecord_TNAM_Counter = 0;
            subRecord_PFO2_Counter = 0;
            subRecord_ALFD_Counter = 0;
            subRecord_FNAM_ounter = 0;
            subRecord_WNAM_Counter = 0;
            subRecord_BPTN_Counter = 0;
            subRecord_EPF2_Counter = 0;
            subRecord_RPLI_Counter = 0;
            subRecord_PNAM_Counter = 0;

            editorID = String.Empty;
            itemDesc = String.Empty;
            stringID = String.Empty;
            recType = String.Empty;
            stringType = String.Empty;

            if (record.FormID.ToString().ToLower() == "16855720")
              stringType = String.Empty;

            foreach (var subRec in record.SubRecords)
            {
              if (subRec.Size > 1)
              {
                #region EDID
                if (subRec.Name == "EDID")
                {
                  editorID = subRec.GetStrData();
                  //edtMemo.Text += Environment.NewLine + " *** EDID:" + editorID + " Match:" + matchRecordOK.ToString();
                }
                #endregion EDID

                #region OTHER
                if (subRec.Name != "EDID")
                {
                  itemDesc = String.Empty;
                  stringID = String.Empty;
                  recType = String.Empty;
                  stringType = String.Empty;
                  continueProcessString = false;
                  translateRecord = true;

                  //if (subRec.Name == "NAM1" & subRec.Size >= 4)//RDMP  RNAM  MOD2
                  //  translateRecord = true;

                  if (matchRecordOK)
                  {
                    valueType = subRec.Structure.elements[0].type;
                    //if (valueType == TESVSnip.ElementValueType.String) continueProcessString = true;
                    if (valueType == TESVSnip.ElementValueType.LString) continueProcessString = true;
                    if (valueType == TESVSnip.ElementValueType.Blob) continueProcessString = true;
                    if (valueType == TESVSnip.ElementValueType.FormID) continueProcessString = false;
                  }
                  //else
                  //{
                  //  if (subRec.Name == "FULL") { continueProcessString = true; }
                  //  if (subRec.Name == "DESC") { continueProcessString = true; }
                  //  if (subRec.Name == "ITXT") { continueProcessString = true; }
                  //  if (subRec.Name == "NNAM") { continueProcessString = true; }
                  //  if (subRec.Name == "CNAM") { continueProcessString = true; }
                  //  if (subRec.Name == "DNAM") { continueProcessString = true; }
                  //  if (subRec.Name == "TNAM") { continueProcessString = true; }
                  //  if (subRec.Name == "SNAM") { continueProcessString = true; }
                  //  if (subRec.Name == "DATA") { continueProcessString = true; }
                  //}

                  if (subRec.Size < 4) continueProcessString = false;

                  if (continueProcessString)
                  {
                    if (subRec.Name == "ALFD") { subRecord_ALFD_Counter++; }
                    if (subRec.Name == "BPTN") { subRecord_BPTN_Counter++; }
                    if (subRec.Name == "CNAM") { subRecord_CNAM_Counter++; }
                    if (subRec.Name == "EPF2") { subRecord_EPF2_Counter++; }
                    if (subRec.Name == "FNAM") { subRecord_FNAM_ounter++; }
                    if (subRec.Name == "ITXT") { subRecord_ITXT_Counter++; }
                    if (subRec.Name == "NNAM") { subRecord_NNAM_Counter++; }
                    if (subRec.Name == "PFO2") { subRecord_PFO2_Counter++; }
                    if (subRec.Name == "DNAM") { subRecord_DNAM_Counter++; }
                    if (subRec.Name == "TNAM") { subRecord_TNAM_Counter++; }
                    if (subRec.Name == "RPLI") { subRecord_RPLI_Counter++; }
                    if (subRec.Name == "WNAM") { subRecord_WNAM_Counter++; }
                    if (subRec.Name == "PNAM") { subRecord_PNAM_Counter++; }
                    
                    ArraySegment<byte> dataSegment = new ArraySegment<byte>(subRec.GetData(), 0x00, (int)subRec.Size);
                    bool isString = TypeConverter.IsLikelyString(dataSegment);
                    if (isString)
                      SetTextAsString(dataSegment, ref itemDesc, ref stringID);
                    else
                      if (subRec.Size > 4)
                      {
                        if (subRec.Name == "FULL" | subRec.Name == "DESC")
                          SetTextAsString(dataSegment, ref itemDesc, ref stringID);
                        //else
                        //  SetTextByID(dataSegment, ref itemDesc, ref stringID);
                      }
                      else
                      {
                        string itemDescTemp = String.Empty;
                        SetTextAsString(dataSegment, ref itemDescTemp, ref stringID);
                        SetTextByID(dataSegment, ref itemDesc, ref stringID);

                        //if (stringID == "00000000")
                        //{
                        //  SetTextAsString(dataSegment, ref itemDesc, ref stringID);
                        if (itemDescTemp.Length <= 0)
                          itemDesc = String.Empty;
                        else
                          if (itemDescTemp.Length >= 1)
                            if (itemDescTemp[0] == '\0')
                            { itemDesc = String.Empty; stringID = "00000000"; } //don't translate
                        //}
                      }

                    translateRecord = true;
                    if (filtersRecords) //filtersRecords=true for not populate listview for translation, filtersRecords=false it's only for generate dictionnary with full strings
                    {
                      if (subRec.Name == "FULL" | subRec.Name == "DESC")

                        if (editorID == itemDesc) //do not translate if EDID:"ArmorAtronachFrostShield" = FULL:"ArmorAtronachFrostShield"
                          translateRecord = false;
                    }

                    if (stringID == "00000001") translateRecord = false;

                    if (stringID == "00000000" & itemDesc == String.Empty) translateRecord = false;

                    if (itemDesc.Length >= 3)
                      if (itemDesc.Substring(0, 3) == "...")
                        translateRecord = false;

                    if (translateRecord)
                      if ((!String.IsNullOrWhiteSpace(itemDesc)) | (!String.IsNullOrWhiteSpace(stringID)))
                      {
                        //edtMemo.Text += Environment.NewLine + "     * " + subRec.Name + ":" + " ContinueProcessstring:" + continueProcessString.ToString();

                        recType = subRec.Name;
                        recTypeTH = subRec.Name;
                        if (subRec.Name == "ITXT") recTypeTH = recType + ":" + subRecord_ITXT_Counter.ToString();
                        if (subRec.Name == "NNAM") recTypeTH = recType + ":" + subRecord_NNAM_Counter.ToString();
                        if (subRec.Name == "CNAM") recTypeTH = recType + ":" + subRecord_CNAM_Counter.ToString();
                        if (subRec.Name == "DNAM") recTypeTH = recType + ":" + subRecord_DNAM_Counter.ToString();
                        if (subRec.Name == "TNAM") recTypeTH = recType + ":" + subRecord_TNAM_Counter.ToString();
                        if (subRec.Name == "PFO2") recTypeTH = recType + ":" + subRecord_PFO2_Counter.ToString();
                        if (subRec.Name == "ALFD") recTypeTH = recType + ":" + subRecord_ALFD_Counter.ToString();
                        if (subRec.Name == "FNAM") recTypeTH = recType + ":" + subRecord_FNAM_ounter.ToString();
                        if (subRec.Name == "WNAM") recTypeTH = recType + ":" + subRecord_WNAM_Counter.ToString();
                        if (subRec.Name == "PNAM") recTypeTH = recType + ":" + subRecord_PNAM_Counter.ToString();

                        if (recTypeTH=="PNAM:43")
                          recTypeTH = recType + ":" + subRecord_PNAM_Counter.ToString();

                        if (!updateStringsPlugIn)
                        {
                          row = tblPlugInStringsLoad.NewRow();
                          row["GroupName"] = groupName;
                          row["RecordType"] = recType;
                          row["RecordTypeTH"] = recTypeTH;
                          row["StringType"] = GetDefaultStringType(recType); //default- good string type is given by UpdateProjectStrings(...)
                          row["FormID"] = record.FormID;
                          row["FormIDHexa"] = record.FormID.ToString("x8").ToUpperInvariant();
                          row["EditorID"] = editorID;
                          row["SourceStringID"] = Convert.ToUInt32(stringID, 16);
                          row["SourceStringIDHexa"] = stringID.ToUpperInvariant();
                          //row["SourceItemDesc"] = itemDesc.Replace("[", "").Replace("]", "");
                          row["SourceItemDesc"] = itemDesc;
                          row["StringStatus"] = "?";
                          tblPlugInStringsLoad.Rows.Add(row);
                        }

                        if (updateStringsPlugIn)
                        {
                          //DataView dvPlugInFormIDHexa = new DataView();
                          //dvPlugInFormIDHexa.Table = tblPlugInStringsProject;
                          //dvPlugInFormIDHexa.Sort = "GroupName, FormIDHexa, RecordTypeTH";  

                          formIDHexa = record.FormID.ToString("x8").ToUpperInvariant();
                          foundRows = dvPlugInFormIDHexa.FindRows(new object[] { groupName, formIDHexa, recTypeTH });

                          if (foundRows.Length == 1)
                          {
                            //this.MainViewTH.UpdateMainTextByTH(subRec);
                            if (Convert.ToBoolean(foundRows[0]["WriteStringInPlugIn"]))
                            {
                              byte[] itemName = TypeConverter.str2h(Convert.ToString(foundRows[0]["TargerItemDesc"]));
                              subRec.SetData(itemName);
                              countUpdatedRecords++;
                              //this.MainViewTH.UpdateMainTextByTH(subRec);
                              edtMemo.Text += Environment.NewLine + groupName + ": " + formIDHexa + " - " + recTypeTH;
                            }
                          }
                        }

                      }

                  }


                }
                #endregion OTHER
              }

            }
          }
        }

        totalRecordsWritingToplugIn += countUpdatedRecords;
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecordsSubRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
    }

    /// <summary>
    /// WriteStringsInPlugIn
    /// </summary>
    public bool WriteStringsInPlugIn()
    {

      TESVSnip.GroupRecord groupRec;
      bool onlyReadRecord = true;
      bool filtersRecords = true;
      bool onlyTargetStrings = false;
      bool updateStringsPlugIn = true;

      try
      {
        tblPlugInStringsLoad.Rows.Clear();
        totalRecordsWritingToplugIn = 0;
        for (int groupCounter = 1; groupCounter < PluginTree.Records.Count; groupCounter++)
        {
          groupRec = (TESVSnip.GroupRecord)PluginTree.Records[groupCounter];
          ReadWritePlugInRecordsSubRecords(ref  groupRec, onlyReadRecord, filtersRecords, onlyTargetStrings, updateStringsPlugIn);
        }

        edtMemo.Text += Environment.NewLine + "TOTAL Record Updated: " + totalRecordsWritingToplugIn.ToString();
        tblPlugInStringsLoad.Rows.Clear();
        return true;
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
        return false;
      }
 
    }

  }
}