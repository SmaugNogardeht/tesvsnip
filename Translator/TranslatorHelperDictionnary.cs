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
    /// GenerateSkyrimStringsDictionary
    /// </summary>
    /// <param name="language"></param>
    /// <param name="dict"></param>
    /// <param name="type"></param>
    public void GenerateSkyrimStringsDictionary(string language, LocalizedStringDict dict, string type, string action)
    {
      DataRow row;

      if (action == "CREATE") tblSkyrimStrings.Rows.Clear();

      foreach (System.Collections.Generic.KeyValuePair<uint, string> kvp in dict)
      {
        row = tblSkyrimStrings.NewRow();
        row["StringID"] = kvp.Key;
        row["StringIDHexa"] = kvp.Key.ToString("X8").ToUpperInvariant(); ;
        row["TextValue"] = kvp.Value;
        row["StringType"] = type; //strings / dlstring /  ilstrings
        tblSkyrimStrings.Rows.Add(row);
      }

      if (action == "WRITE")
      {
        string appPath = Path.GetDirectoryName(Application.ExecutablePath);
        appPath = Path.Combine(appPath, @"Dict\");
        System.IO.Directory.CreateDirectory(appPath);

        string appPathPlugin = Path.Combine(appPath, "Skyrim_Strings_" + language + ".dicstr");
        if (File.Exists(appPathPlugin)) File.Delete(appPathPlugin);
        tblSkyrimStrings.WriteXml(appPathPlugin, XmlWriteMode.WriteSchema);
        tblSkyrimStrings.Rows.Clear();
      }
    }

    /// <summary>
    /// GenerateSkyrimStringsDictionaryWithSkyrimReference
    /// </summary>
    /// <param name="languageSource"></param>
    /// <param name="type"></param>
    public void GenerateSkyrimStringsDictionaryWithSkyrimReference()
    {
      string languageSource = cboxSourceLanguage.Text;
      string languageTarget = cboxTargetLanguage.Text;
      string SourceStringIDHexa;
      string StringType = String.Empty;

      string appPath = Path.GetDirectoryName(Application.ExecutablePath);
      appPath = Path.Combine(appPath, @"Dict\");

      if (!LoadSkyrimStringsDictionnary()) return;

      string sourceLang = String.Empty;
      string targetLang = String.Empty;

      DataRowView[] foundRows;
      DataRow rowDict;

      DataView dvSource = new DataView();
      dvSource.Table = tblSkyrimSourceStrings;
      dvSource.Sort = "StringIDHexa, StringType"; //StringType = strings / dlstring /  ilstrings
      //dvSource.Sort = "StringIDHexa";

      DataView dvTarget = new DataView();
      dvTarget.Table = tblSkyrimTargetStrings;
      dvTarget.Sort = "StringIDHexa, StringType"; //StringType = strings / dlstring /  ilstrings
      //dvTarget.Sort = "StringIDHexa";

      try
      {
        tblSkyrimEsmDict.Rows.Clear();

        foreach (DataRow row in tblPlugInStringsLoad.Rows)
        {
          for (int stringTypeCpt = 1; stringTypeCpt <= 3; stringTypeCpt++)
          {
            SourceStringIDHexa = Convert.ToString(row["SourceStringIDHexa"]);

            sourceLang = String.Empty;
            targetLang = String.Empty;

            StringType = "Strings";
            if (stringTypeCpt == 2) StringType = "DLStrings";
            if (stringTypeCpt == 3) StringType = "ILStrings";

            foundRows = dvSource.FindRows(new object[] { SourceStringIDHexa, StringType });
            //foundRows = dvSource.FindRows(new object[] { SourceStringIDHexa });
            if (foundRows.Length == 1) sourceLang = foundRows[0]["TextValue"].ToString();

            foundRows = dvTarget.FindRows(new object[] { SourceStringIDHexa, StringType });
            if (foundRows.Length == 1) targetLang = foundRows[0]["TextValue"].ToString();

            //foreach (DataRowView rowV in foundRows)
            if (foundRows.Length == 1 | foundRows.Length == 1)
            {
              //if (foundRows[0]["TextValue"].ToString().Length > 255)
              //  sourceLang = foundRows[0]["TextValue"].ToString().Substring(0, 255);
              //else
              //sourceLang = foundRows[0]["TextValue"].ToString();

              StringType = Convert.ToString(foundRows[0]["StringType"]);

              rowDict = tblSkyrimEsmDict.NewRow();
              rowDict["GroupName"] = row["GroupName"];
              rowDict["RecordType"] = row["RecordType"];
              rowDict["StringType"] = StringType; // rowV["StringType"]; // row["StringType"];
              rowDict["FormID"] = row["FormID"];
              rowDict["FormIDHexa"] = Convert.ToString(row["FormIDHexa"]).ToUpperInvariant();
              rowDict["EditorID"] = row["EditorID"];
              rowDict["SkyrimStringID"] = row["SourceStringID"];
              rowDict["SkyrimStringIDHexa"] = Convert.ToString(row["SourceStringIDHexa"]).ToUpperInvariant();
              rowDict["SkyrimItemDescSourceLang"] = sourceLang;
              rowDict["SkyrimItemDescTargetLang"] = targetLang;

              tblSkyrimEsmDict.Rows.Add(rowDict);
            }
          }

        }
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecordsSubRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
      finally
      {
        dvSource.Dispose();
        dvSource = null;
        dvTarget.Dispose();
        dvTarget = null;

        string appPathPlugin = Path.Combine(appPath, "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic");
        tblSkyrimEsmDict.WriteXml(appPathPlugin, XmlWriteMode.WriteSchema);
        tblSkyrimEsmDict.Rows.Clear();
        GC.Collect();
      }
    }

    /// <summary>
    /// LoadSkyrimStringsDictionnary
    /// </summary>
    private bool LoadSkyrimStringsDictionnary()
    {
      string languageSource = cboxSourceLanguage.Text;
      string languageTarget = cboxTargetLanguage.Text;

      string appPath = Path.GetDirectoryName(Application.ExecutablePath);
      appPath = Path.Combine(appPath, @"Dict\");

      string appPathPlugin = Path.Combine(appPath, "Skyrim_Strings_" + languageSource + ".dicstr");

      if (!File.Exists(appPathPlugin))
      {
        MessageBox.Show("File doesn't exist." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }

      try
      {
        #region Read Source Strings Dictionnary

        tblSkyrimSourceStrings.Rows.Clear();
        tblSkyrimSourceStrings.ReadXml(appPathPlugin);

        if (tblSkyrimSourceStrings.Rows.Count == 0)
        {
          edtMemo.Text += Environment.NewLine + "****** Strings file is empty." + Environment.NewLine + appPathPlugin;
          MessageBox.Show("Strings file is empty." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;
        }
        //else
        //  edtMemo.Text += Environment.NewLine + "Skyrim_Strings_" + languageSource + ".dicstr : " + tblSkyrimSourceStrings.Rows.Count.ToString() + " lines";

        #endregion Read Source Strings Dictionnary

        #region Read Target Strings Dictionnary

        appPathPlugin = Path.Combine(appPath, "Skyrim_Strings_" + languageTarget + ".dicstr");

        if (!File.Exists(appPathPlugin))
        {
          MessageBox.Show("File doesn't exist." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;
        }

        tblSkyrimTargetStrings.Rows.Clear();
        tblSkyrimTargetStrings.ReadXml(appPathPlugin);

        if (tblSkyrimTargetStrings.Rows.Count == 0)
        {
          edtMemo.Text += Environment.NewLine + "****** Strings file is empty." + Environment.NewLine + appPathPlugin;
          MessageBox.Show("Strings file is empty." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;
        }
        else
          edtMemo.Text += Environment.NewLine + "Skyrim_Strings_" + languageTarget + ".dicstr : " + tblSkyrimTargetStrings.Rows.Count.ToString() + " lines";

        #endregion Read Target Strings Dictionnary

        return true;
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecordsSubRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
        return false;
      }
    }

    /// <summary>
    /// LoadSkyrimEsmDictionnary
    /// </summary>
    public bool LoadSkyrimEsmDictionnary()
    {
      string languageSource = cboxSourceLanguage.Text;
      string languageTarget = cboxTargetLanguage.Text;

      if (LastLoadedSkyrimEsmDictionnary == "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic") return true;

      string appPath = Path.GetDirectoryName(Application.ExecutablePath);
      appPath = Path.Combine(appPath, @"Dict\");
      string appPathPlugin = Path.Combine(appPath, "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic");

      if (!File.Exists(appPathPlugin))
      {
        MessageBox.Show("Dictionnary file doesn't exist." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
      else
        edtMemo.Text += Environment.NewLine + "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic : " + tblSkyrimTargetStrings.Rows.Count.ToString() + " lines";

      try
      {
        if (listViewSkyrimDict != null) listViewSkyrimDict.Clear(); else listViewSkyrimDict = new System.Collections.Generic.List<ObjStringsDict>();
        olvSkyrimDict.Items.Clear();
        tblSkyrimEsmDict.Rows.Clear();
        GC.Collect();
        tblSkyrimEsmDict.ReadXml(appPathPlugin);

        //if (!LoadSkyrimEsmDictionnary()) return;

        foreach (DataRow row in tblSkyrimEsmDict.Rows)
        {
          if (Convert.ToString(row["SkyrimStringIDHexa"]) != 0.ToString("X8"))
            if (!String.IsNullOrWhiteSpace(Convert.ToString(row["SkyrimItemDescSourceLang"])))
              if (!String.IsNullOrWhiteSpace(Convert.ToString(row["SkyrimItemDescTargetLang"])))
              {
                listViewSkyrimDict.Add(new ObjStringsDict(
                 Convert.ToString(row["SkyrimStringIDHexa"]),
                 Convert.ToString(row["SkyrimItemDescSourceLang"]),
                 Convert.ToString(row["SkyrimItemDescTargetLang"])
                 ));
              }
        }

        this.olvSkyrimDict.SetObjects(listViewSkyrimDict);
        olvSkyrimDict.ShowGroups = false;
        olvSkyrimDict.BuildList();
        LastLoadedSkyrimEsmDictionnary = "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic";

        return true;
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecordsSubRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
        return false;
      }
    }

  }
}
