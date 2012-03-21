using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;

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
    /// Save Project
    /// </summary>
    public void SaveProject()
    {
      try
      {
        edtMemo.Text = String.Empty;

        if (String.IsNullOrEmpty(edtPlugInName.Text))
        {
          MessageBox.Show("Open project or plugin before saved.", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          return;
        }

        if (tblPlugInHeader == null | tblPlugInStringsProject == null)
        {
          MessageBox.Show("Empty project !", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          return;
        }

        if (tblPlugInHeader.Rows.Count == 1)
          tblPlugInHeader.Rows[0].BeginEdit();
        else
        {
          tblPlugInHeader.Clear();
          DataRow row = tblPlugInHeader.NewRow();
          tblPlugInHeader.Rows.Add(row);
          tblPlugInHeader.Rows[0].BeginEdit();
        }

        tblPlugInHeader.Rows[0]["PlugInName"] = edtPlugInName.Text;
        tblPlugInHeader.Rows[0]["Author"] = edtPlugInAuthor.Text;
        tblPlugInHeader.Rows[0]["TranslatedBy"] = edtTranslatedBy.Text;
        tblPlugInHeader.Rows[0]["SourceLanguage"] = cboxSourceLanguage.Text;
        tblPlugInHeader.Rows[0]["TargetLanguage"] = cboxTargetLanguage.Text;
        if (!String.IsNullOrEmpty(PluginLocation)) tblPlugInHeader.Rows[0]["PluginLocation"] = PluginLocation;
        tblPlugInHeader.Rows[0]["ProjectStructureVersion"] = 1;
        tblPlugInHeader.Rows[0]["PluginInfo"] = edtPluginInfo.Text;

        tblPlugInHeader.Rows[0].EndEdit();

        string appPath = Path.GetDirectoryName(Application.ExecutablePath);
        appPath = Path.Combine(appPath, @"Projects\");
        System.IO.Directory.CreateDirectory(appPath);

        string appPathPlugin = Path.Combine(appPath, edtPlugInName.Text + ".thprj");
        if (File.Exists(appPathPlugin)) File.Delete(appPathPlugin);
        tblPlugInHeader.WriteXml(appPathPlugin, XmlWriteMode.WriteSchema);

        string appPathPlugInStrings = Path.Combine(appPath, edtPlugInName.Text + ".thstrings");
        if (File.Exists(appPathPlugInStrings)) File.Delete(appPathPlugInStrings);
        tblPlugInStringsProject.WriteXml(appPathPlugInStrings, XmlWriteMode.WriteSchema);

        MessageBox.Show("Project saved.", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in SaveProject ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
    }

    /// <summary>
    /// Open Project
    /// </summary>
    public void OpenProject()
    {
      DataRow row;
      try
      {
        OpenProjectModDialog.Multiselect = false;
        string appPath = Path.GetDirectoryName(Application.ExecutablePath);
        appPath = Path.Combine(appPath, @"Projects\");
        System.IO.Directory.CreateDirectory(appPath);
        OpenProjectModDialog.InitialDirectory = appPath;

        OpenProjectModDialog.Filter = "Project (*.thprj)|*.thprj;";
        if (OpenProjectModDialog.ShowDialog(this) != DialogResult.OK) return;

        edtMemo.Text = String.Empty;

        tblPlugInHeader.Rows.Clear();
        tblPlugInStringsProject.Rows.Clear();

        //Open project
        string appPathPlugin = OpenProjectModDialog.FileName;
        if (File.Exists(appPathPlugin))
        {
          Cursor.Current = Cursors.WaitCursor;
          this.MainViewTH.Update();

          try
          {
            DataTable tbl = new DataTable();
            tbl.ReadXml(appPathPlugin);
            row = tblPlugInHeader.NewRow();
            foreach (DataColumn col in tblPlugInHeader.Columns)
            {
              foreach (DataColumn colTmp in tbl.Columns)
                if (col.ColumnName == colTmp.ColumnName)
                  row[col.ColumnName] = tbl.Rows[0][col.ColumnName];
            }
            tbl.Rows.Clear();
            tbl.Dispose();
            tblPlugInHeader.Rows.Add(row);

            edtPlugInName.Text = Convert.ToString(tblPlugInHeader.Rows[0]["PlugInName"]);
            edtPlugInAuthor.Text = Convert.ToString(tblPlugInHeader.Rows[0]["Author"]);
            edtTranslatedBy.Text = Convert.ToString(tblPlugInHeader.Rows[0]["TranslatedBy"]);
            cboxSourceLanguage.Text = Convert.ToString(tblPlugInHeader.Rows[0]["SourceLanguage"]);
            cboxTargetLanguage.Text = Convert.ToString(tblPlugInHeader.Rows[0]["TargetLanguage"]);
            PluginLocation = Convert.ToString(tblPlugInHeader.Rows[0]["PluginLocation"]);
            edtPluginInfo.Text = Convert.ToString(tblPlugInHeader.Rows[0]["PluginInfo"]);

          }
          catch (Exception ex)
          {
            edtMemo.Text = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source;
          }

          //Open strings of project
          string appPathPlugInStrings = Path.Combine(appPath, edtPlugInName.Text + ".thstrings");
          if (File.Exists(appPathPlugInStrings))
          {

            DataTable tbl = new DataTable();
            tbl.ReadXml(appPathPlugInStrings);

            foreach (DataRow rowXml in tbl.Rows)
            {
              row = tblPlugInStringsProject.NewRow();
              foreach (DataColumn col in tblPlugInStringsProject.Columns)
              {
                foreach (DataColumn colTmp in tbl.Columns)
                  if (col.ColumnName == colTmp.ColumnName)
                  {
                    row[col.ColumnName] = rowXml[col.ColumnName];
                  }
                  else
                    if (colTmp.ColumnName == "TargerItemDesc" & col.ColumnName == "TargetItemDesc")
                      row["TargetItemDesc"] = rowXml["TargerItemDesc"];
                    else
                      if (colTmp.ColumnName == "TargerItemDescOld" & col.ColumnName == "TargetItemDescOld")
                        row["TargetItemDescOld"] = rowXml["TargerItemDescOld"];
              }

              if (Convert.ToInt16(tblPlugInHeader.Rows[0]["ProjectStructureVersion"]) < 1) row["WriteStringInPlugIn"] = true;

              tblPlugInStringsProject.Rows.Add(row);
            }
            tbl.Rows.Clear();
            tbl.Dispose();
            tbl = null;

            //tblPlugInStringsProject.ReadXml(appPathPlugInStrings);

            if (Convert.ToInt16(tblPlugInHeader.Rows[0]["ProjectStructureVersion"]) < 1)
            {
              UpdateOldProjectStructure();
              CompareInProjectSkyrimStringsWithPluginStringsSourceAndTarget();
            }
            PopulateListViewStrings();
          }


          //Try open plugin associate to project
          if (String.IsNullOrEmpty(PluginLocation))
          {
            if (!String.IsNullOrEmpty(edtPlugInName.Text))
            {
              string filePath = Path.Combine(Program.gameDataDir, edtPlugInName.Text);
              if (File.Exists(filePath))
              {
                PluginLocation = filePath;
                this.MainViewTH.OpenPlugin(PluginLocation, false);
              }
              else
              {
                if (Path.GetExtension(PluginLocation) == "")
                {
                  edtPlugInName.Text += ".esp";
                  filePath = Path.Combine(Program.gameDataDir, edtPlugInName.Text);
                  if (File.Exists(filePath))
                  {
                    PluginLocation = filePath;
                    this.MainViewTH.OpenPlugin(PluginLocation, false);
                  }
                }
              }
            }
          }
          else
          {
            if (Path.GetExtension(PluginLocation) == "")
            {
              edtPlugInName.Text += ".esp";
              string filePath = Path.Combine(Program.gameDataDir, edtPlugInName.Text);
              if (File.Exists(filePath)) PluginLocation = filePath;
            }

            if (File.Exists(PluginLocation))
              this.MainViewTH.OpenPlugin(PluginLocation, false);
          }
        }
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in OpenProject ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    /// <summary>
    /// Delete Project
    /// </summary>
    public void DeleteProject()
    {
      try
      {
        OpenProjectModDialog.Multiselect = false;
        string appPath = Path.GetDirectoryName(Application.ExecutablePath);
        appPath = Path.Combine(appPath, @"Projects\");
        System.IO.Directory.CreateDirectory(appPath);
        OpenProjectModDialog.InitialDirectory = appPath;

        OpenProjectModDialog.Filter = "Project (*.thprj)|*.thprj;";
        if (OpenProjectModDialog.ShowDialog(this) != DialogResult.OK) return;

        string appPathPlugin = OpenProjectModDialog.FileName;
        if (File.Exists(appPathPlugin))
        {
          File.Delete(appPathPlugin);
          string appPathPlugInStrings = Path.Combine(appPath, edtPlugInName.Text + ".thstrings");
          if (File.Exists(appPathPlugInStrings)) File.Delete(appPathPlugInStrings);
          ClearProject();
          MessageBox.Show("Project deleted.", "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in DeleteProject ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
    }

    /// <summary>
    /// Clear Project
    /// </summary>
    public void ClearProject()
    {
      ClearTextBoxControl(this);

      if (listViewStrings != null) listViewStrings.Clear();
      olvTHStrings.Items.Clear();

      if (listViewStringsDL != null) listViewStringsDL.Clear();
      olvTHDLStrings.Items.Clear();

      if (listViewStringsIL != null) listViewStringsIL.Clear();
      olvTHILStrings.Items.Clear();

      if (listViewStringsOther != null) listViewStringsOther.Clear();
      olvTHOtherStrings.Items.Clear();

      if (listViewOtherSkyrimStringsSource != null) listViewOtherSkyrimStringsSource.Clear();
      olvTHSkyrimSourceStrings.Items.Clear();

      tblStrings.Rows.Clear();
      tblPlugInStringsLoad.Rows.Clear();
      tblPlugInStringsProject.Rows.Clear();
      tblSkyrimStrings.Rows.Clear();
    }

    /// <summary>
    /// RemoveObsoleteRecords
    /// </summary>
    public void RemoveObsoleteRecords()
    {
      DataRow row;
      for (int i = tblPlugInStringsProject.Rows.Count - 1; i >= 0; i--)
      {
        row = tblPlugInStringsProject.Rows[i];
        if (Convert.ToString(row["StringStatus"]) == "Del")
          row.Delete();
      }
      PopulateListViewStrings();
    }

    /// <summary>
    /// UpdateOldProjectStructure
    /// </summary>
    public void UpdateOldProjectStructure()
    {
      DataRow row;
      int pos;
      string group;

      if (tblPlugInHeader.Rows.Count != 1) return;
      int ProjectStructureVersion = Convert.ToInt16(tblPlugInHeader.Rows[0]["ProjectStructureVersion"]);
      if (ProjectStructureVersion >= 1) return;
      tblPlugInHeader.Rows[0].BeginEdit();
      tblPlugInHeader.Rows[0]["ProjectStructureVersion"] = 1;
      tblPlugInHeader.Rows[0].EndEdit();

      for (int i = tblPlugInStringsProject.Rows.Count - 1; i >= 0; i--)
      {
        row = tblPlugInStringsProject.Rows[i];
        row.BeginEdit();
        group = Convert.ToString(row["GroupName"]);
        pos = group.IndexOf("(");
        if (pos >=0)
          group = group.Substring(pos).Trim().Replace("(", "").Replace(")", "");
        row["GroupName"] = group;
        row["RecordTypeTH"] = row["RecordType"];
        row.EndEdit();
      }
      PopulateListViewStrings();
    }
  }
}