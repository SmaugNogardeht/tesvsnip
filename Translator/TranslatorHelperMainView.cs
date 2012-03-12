using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Windows.Forms;

using TESVSnip.Data;
using TESVSnip.Properties;

namespace TESVSnip
{
  internal partial class MainView
  {
    private string _pluginPath = String.Empty;

    /// <summary>
    /// Initialize ToolStrip - Add TranslatorHelper menu
    /// </summary>
    private void InitializeToolStripTranslatorHelper()
    {
      ToolStripMenuItem itemTranslator = new ToolStripMenuItem("&Translator");

      itemTranslator.DropDownItems.Add("Process selected PlugIn", null, ProcessSelectedPlugin_Click);
      itemTranslator.DropDownItems.Add("Open PlugIn and process it", null, OpenPluginAndProcess_Click);
      itemTranslator.DropDownItems.Add("Update translated strings from an external '.esp.' file (Replace all strings).", null, OpenPluginAndUpdateTargetStringsOnly_Click);

      itemTranslator.DropDownItems.Add(new ToolStripSeparator());

      itemTranslator.DropDownItems.Add("Open Project", null, OpenProject_Click);
      itemTranslator.DropDownItems.Add("Save Project", null, SaveProject_Click);
      itemTranslator.DropDownItems.Add("Close Project", null, CloseProject_Click);
      itemTranslator.DropDownItems.Add("Delete Project", null, DeleteProject_Click);

      itemTranslator.DropDownItems.Add(new ToolStripSeparator());

      itemTranslator.DropDownItems.Add("Export to CSV", null, ExportToCSV_Click);

      itemTranslator.DropDownItems.Add(new ToolStripSeparator());

      itemTranslator.DropDownItems.Add("Compare Source/Target strings of the project", null, CompareStringsRecordsProject_Click);
      //itemTranslator.DropDownItems.Add("Translate untranslated texts of the project from the source dictionary", null, SearchUntranslatedTextsProject_Click);
      itemTranslator.DropDownItems.Add("Remove obsolete records of the project (status = Del)", null, RemoveObsoleteRecordsProject_Click);

      itemTranslator.DropDownItems.Add(new ToolStripSeparator());

      itemTranslator.DropDownItems.Add("Write translation in selected mod (with embed strings).", null, WriteStringsInPlugIn_Click);

      itemTranslator.DropDownItems.Add(new ToolStripSeparator());

      itemTranslator.DropDownItems.Add("Load Skyrim Dictionnary", null, LoadSkyrimDictionnary_Click);
      itemTranslator.DropDownItems.Add("Build Skyrim Dictionnary", null, BuildSkyrimDictionnary_Click);

      menuStrip1.Items.Add(itemTranslator);

      translatorHelperContent.MainViewTH = this; //Set handle of MainView Form
    }

    /// <summary>
    /// TestIfPlugInIsSelected
    /// </summary>
    /// <returns></returns>
    private bool TestIfPlugInIsSelected()
    {
      var plugin = GetPluginFromNode(this.PluginTree.SelectedRecord);
      if (plugin == null)
      {
        MessageBox.Show(Resources.NoPluginSelected, Resources.ErrorText);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Dummy function
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NotImplemented_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Not Implemented");
    }

    /// <summary>
    /// Close All - Bypass closeAllToolStripMenuItem_Click for skip alert message
    /// </summary>
    private void CloseAll()
    {
      //closeAllToolStripMenuItem_Click(this, new EventArgs());
      PluginList.All.Records.Clear();
      PluginTree.UpdateRoots();
      SubrecordList.Record = null;
      Clipboard = null;
      CloseStringEditor();
      UpdateMainText("");
      RebuildSelection();
      PluginTree.UpdateRoots();
      GC.Collect();
    }

    /// <summary>
    /// ProcessSelectedPlugin_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ProcessSelectedPlugin_Click(object sender, EventArgs e)
    {
      if (PluginTree == null) { MessageBox.Show("Open mod first !", Resources.ErrorText); return; }
      if (PluginTree.TopRecord == null) { MessageBox.Show("Select mod first !", Resources.ErrorText); return; }

      PluginTree.SelectedRecord = PluginTree.TopRecord;
      string filePath = Path.Combine(Program.gameDataDir, PluginTree.SelectedRecord.DescriptiveName);
      if (File.Exists(filePath)) _pluginPath = filePath;

      ProcessPlugin(false, true, false); //bool onlyReadRecord, bool filtersRecords, bool onlyTargetStrings = false
    }

    /// <summary>
    /// OpenPluginAndProcess_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenPluginAndProcess_Click(object sender, EventArgs e)
    {
      if (OpenPlugin(String.Empty, true))
        ProcessPlugin(false, true, false); //bool onlyReadRecord, bool filtersRecords, bool onlyTargetStrings = false
    }

    /// <summary>
    /// OpenPluginAndUpdateTargetStringsOnly_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenPluginAndUpdateTargetStringsOnly_Click(object sender, EventArgs e)
    {
      if (OpenPlugin(String.Empty, true))
        ProcessPlugin(false, true, true);
    }

    /// <summary>
    /// OpenPlugin
    /// </summary>
    /// <param name="pluginPath"></param>
    /// <param name="displayConfirmMessage"></param>
    /// <returns></returns>
    public bool OpenPlugin(string pluginPath, bool displayConfirmMessage)
    {
      if (displayConfirmMessage)
      {
        if ((PluginTree.TopRecord != null) | (PluginTree.SelectedRecord != null))
        {
          int answer_index = Cliver.Message.Show("Translator Helper", System.Drawing.SystemIcons.Question, "This choice close all opening mod without save it. Do you want close all ?", 0, "Yes", "No");
          if (answer_index == 1) return true;
        }
      }

      if (String.IsNullOrWhiteSpace(pluginPath))
      {
        OpenModDialog.Multiselect = false;
        if (OpenModDialog.ShowDialog(this) == DialogResult.OK)
        {
          if (OpenModDialog.FileNames.Length != 1)
          {
            MessageBox.Show("Select only one file !", Resources.ErrorText);
            return false;
          }
          _pluginPath = OpenModDialog.FileNames[0];
        }
        else
          return false;
      }
      else
      {
        if (!System.IO.File.Exists(pluginPath))
        {
          MessageBox.Show("Can't find " + pluginPath, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;
        }
        _pluginPath = pluginPath;
      }

      Cursor.Current = Cursors.WaitCursor;
      Update();
      try
      {
        CloseAll();
        PluginTree.UpdateRoots();
        LoadPlugin(_pluginPath);
        FixMasters();
        PluginTree.UpdateRoots();
        PluginTree.SelectedRecord = PluginTree.TopRecord;
        return true;
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    /// <summary>
    /// Process plugin
    /// </summary>
    /// <param name="onlyReadRecord"></param>
    /// <param name="filtersRecords"></param>
    /// <param name="onlyTargetStrings"></param>
    private void ProcessPlugin(bool onlyReadRecord, bool filtersRecords, bool onlyTargetStrings = false)
    {
      if (PluginTree.TopRecord == null)
      {
        MessageBox.Show("No open mod !!!", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }

      if (PluginTree.SelectedRecords.ToList().Count != 1)
      {
        MessageBox.Show("Select only one mod !!!", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }

      PluginTree.SelectedRecord = PluginTree.TopRecord;

      int answer_index = Cliver.Message.Show("Translator Helper", System.Drawing.SystemIcons.Question,
        "Do you want process this mod <" + PluginTree.SelectedRecord.DescriptiveName + ">  ?", 1, "Start Process", "Abort Process");

      if (answer_index == 0) //Start Process
      {
        Cursor.Current = Cursors.WaitCursor;
        Update();
        try
        {
          translatorHelperContent.PluginTree = GetPluginFromNode(PluginTree.SelectedRecord);
          //false=Full plugin process with init/update project  true=filter process
          //true=not init / update. Used for generate dictionnary
          translatorHelperContent.ProcessPlugIn(onlyReadRecord, filtersRecords, onlyTargetStrings, _pluginPath);
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }
      }
    }

    /// <summary>
    /// Save project
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveProject_Click(object sender, EventArgs e)
    {
      translatorHelperContent.SaveProject();
    }

    /// <summary>
    /// Save project
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenProject_Click(object sender, EventArgs e)
    {
      translatorHelperContent.OpenProject();
    }

    /// <summary>
    /// Clear project
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseProject_Click(object sender, EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      Update();
      try
      {
        translatorHelperContent.ClearProject();
        CloseAll();
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    /// <summary>
    /// Compare records of the project.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CompareStringsRecordsProject_Click(object sender, EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      Update();
      try
      {
        translatorHelperContent.CompareInProjectSkyrimStringsWithPluginStringsSourceAndTarget();
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    /// <summary>
    /// Remove obsolete records of the project.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveObsoleteRecordsProject_Click(object sender, EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      Update();
      try
      {
        translatorHelperContent.RemoveObsoleteRecords();
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    /// <summary>
    /// Delete project
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteProject_Click(object sender, EventArgs e)
    {
      translatorHelperContent.DeleteProject();
    }

    /// <summary>
    /// Export to CSV
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExportToCSV_Click(object sender, EventArgs e)
    {
      if (translatorHelperContent.tblPlugInStringsProject.Rows.Count <= 0)
      {
        MessageBox.Show("Project is empty!!!", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }
      Stream myStream;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();

      saveFileDialog1.Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*";
      saveFileDialog1.FilterIndex = 1;
      saveFileDialog1.RestoreDirectory = true;
      saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;

      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        //if ((myStream = saveFileDialog1.OpenFile()) != null)
        //{
        StreamWriter wText = new StreamWriter(File.Create(saveFileDialog1.FileName), Encoding.CP1252);

          wText.Write("GroupName"); wText.Write("\t");
          wText.Write("StringStatus"); wText.Write("\t");
          wText.Write("CompareStatusSource"); wText.Write("\t");
          wText.Write("CompareStatusTarget"); wText.Write("\t");
          wText.Write("RecordType"); wText.Write("\t");
          wText.Write("RecordTypeTH"); wText.Write("\t");
          wText.Write("FormID"); wText.Write("\t");
          wText.Write("FormIDHexa"); wText.Write("\t");
          wText.Write("EditorID"); wText.Write("\t");
          wText.Write("SourceStringIDHexa"); wText.Write("\t");
          wText.Write("SourceItemDesc"); wText.Write("\t");
          wText.Write("TargerItemDesc"); wText.Write("\t");
          wText.Write("WriteStringInPlugIn");
          wText.Write(Environment.NewLine);

          foreach (DataRow row in translatorHelperContent.tblPlugInStringsProject.Rows)
          {
            //if (Convert.ToString(row["EditorID"]) == "MagmaEruptionShout01")
            //  wText.Write("\t");
            wText.Write(Convert.ToString(row["FormID"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["FormIDHexa"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["GroupName"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["StringStatus"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["CompareStatusSource"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["CompareStatusTarget"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["RecordType"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["RecordTypeTH"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["EditorID"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["SourceStringIDHexa"])); wText.Write("\t");
            wText.Write(Convert.ToString(row["SourceItemDesc"]).Replace(Environment.NewLine," ")); wText.Write("\t");
            wText.Write(Convert.ToString(row["TargerItemDesc"]).Replace(Environment.NewLine," ")); wText.Write("\t");
            wText.Write(Convert.ToBoolean(row["WriteStringInPlugIn"]));
            wText.Write(Environment.NewLine);
          }

          wText.Close();
          //myStream.Close();
        //}
      }

      //translatorHelperContent.ExportToCSV();
    }
    

    /// <summary>
    /// Write strings in plugin
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WriteStringsInPlugIn_Click(object sender, EventArgs e)
    {
      if (PluginTree == null) { MessageBox.Show("Open mod first !", Resources.ErrorText); return; }
      if (PluginTree.TopRecord == null) { MessageBox.Show("Select mod first !", Resources.ErrorText); return; }

      PluginTree.SelectedRecord = PluginTree.TopRecord;

      Properties.Settings.Default.LocalizationName = translatorHelperContent.GetTargetLanguage();
      translatorHelperContent.PluginTree = GetPluginFromNode(PluginTree.SelectedRecord);
      if (translatorHelperContent.WriteStringsInPlugIn())
        saveToolStripMenuItem_Click(sender, e);

      Cliver.Message.Show("Translator Helper", System.Drawing.SystemIcons.Information, "Update plugin finished.", 1, "OK");      
    }

    /// <summary>
    /// UpdateMainTextforTH
    /// </summary>
    /// <param name="rec"></param>
    public void UpdateMainTextForTH(BaseRecord rec)
    {
      UpdateMainText(rec);
    }

    /// <summary>
    /// UpdateUTF8State
    /// </summary>
    /// <param name="state"></param>
    public void UpdateUTF8State(bool setUTF8)
    {
      uTF8ModeToolStripMenuItem.Checked = setUTF8;
      Properties.Settings.Default.UseUTF8 = uTF8ModeToolStripMenuItem.Checked;
      uTF8ModeToolStripMenuItem_Click(this, new EventArgs());
    }

    
          /// <summary>
    /// LoadSkyrimDictionnary_Click
    /// </summary>
    private void LoadSkyrimDictionnary_Click(object sender, EventArgs e)
    {
      TESVSnip.Functions.StartCounter();

      Cursor.Current = Cursors.WaitCursor;
      Update();

      try
      {
        translatorHelperContent.LoadSkyrimEsmDictionnary();
      }
      finally
      {
        Cursor.Current = Cursors.Default;
        translatorHelperContent.AddToMemo("Elapsed time:" + TESVSnip.Functions.StopCounter() + Environment.NewLine);
      }
    }

    /// <summary>
    /// BuildSkyrimDicInTargetLanguage_Click
    /// </summary>
    private void BuildSkyrimDictionnary_Click(object sender, EventArgs e)
    {
      TESVSnip.Functions.StartCounter();

      Cursor.Current = Cursors.WaitCursor;
      Update();

      try
      {
        CloseProject_Click(sender, new EventArgs());

        Plugin p = new Plugin();

        string filePath = Path.Combine(Program.gameDataDir, @"Strings\Skyrim_" + translatorHelperContent.GetSourceLanguage() + ".strings");
        if (!File.Exists(filePath))
        {
          MessageBox.Show("Can't find " + filePath, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          TESVSnip.Functions.StopCounter();
          return;
        }

        filePath = Path.Combine(Program.gameDataDir, @"Strings\Skyrim_" + translatorHelperContent.GetTargetLanguage() + ".strings");
        if (!File.Exists(filePath))
        {
          MessageBox.Show("Can't find " + filePath, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          TESVSnip.Functions.StopCounter();
          return;
        }

        Properties.Settings.Default.LocalizationName = translatorHelperContent.GetSourceLanguage();
        p.ReloadStrings("Skyrim", Path.Combine(Program.gameDataDir, "Strings"));
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.Strings, "Strings", "CREATE");
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.DLStrings, "DLStrings", String.Empty);
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.ILStrings, "ILStrings", String.Empty);
        p.ReloadStrings("Update", Path.Combine(Program.gameDataDir, "Strings"));
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.Strings, "Strings", String.Empty);
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.DLStrings, "DLStrings", String.Empty);
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.ILStrings, "ILStrings", "WRITE");

        Properties.Settings.Default.LocalizationName = translatorHelperContent.GetTargetLanguage();
        p.ReloadStrings("Skyrim", Path.Combine(Program.gameDataDir, "Strings"));
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.Strings, "Strings", "CREATE");
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.DLStrings, "DLStrings", String.Empty);
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.ILStrings, "ILStrings", String.Empty);
        p.ReloadStrings("Update", Path.Combine(Program.gameDataDir, "Strings"));
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.Strings, "Strings", String.Empty);
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.DLStrings, "DLStrings", String.Empty);
        translatorHelperContent.GenerateSkyrimStringsDictionary(Properties.Settings.Default.LocalizationName, p.ILStrings, "ILStrings", "WRITE");

        PluginTree.UpdateRoots();
        OpenPlugin(Path.Combine(Program.gameDataDir, "Skyrim.esm"), false);
        PluginTree.SelectedRecord = PluginTree.TopRecord;
        translatorHelperContent.PluginTree = GetPluginFromNode(PluginTree.SelectedRecord);
        translatorHelperContent.ProcessPlugIn(true, false, false, String.Empty);
        translatorHelperContent.GenerateSkyrimStringsDictionaryWithSkyrimReference();
      }
      finally
      {
        Cursor.Current = Cursors.Default;
        CloseAll();
        translatorHelperContent.AddToMemo("Elapsed time:" + TESVSnip.Functions.StopCounter() + Environment.NewLine);
      }

      MessageBox.Show(
        "The " + translatorHelperContent.GetSourceLanguage() + " to " + translatorHelperContent.GetTargetLanguage() + " dictionary has been generated." +
        Environment.NewLine +
        "If STEAM updates STRINGS files, remember to regenerate dictionaries.",
        "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

  }
}