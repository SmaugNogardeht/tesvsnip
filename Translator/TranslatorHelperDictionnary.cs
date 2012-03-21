using System;
using System.Collections.Generic;
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
    private DataTable tblStrings = new DataSetTH.T_StringsDataTable();
    private DataView tblStringsDV = null;

    public DataTable tblSkyrimEsmDict = new DataSetTH.T_StringsDictDataTable();
    private DataView tblSkyrimEsmDictDV_FormIDHexa = null;
    private DataView tblSkyrimEsmDictDV_SkyrimStringIDHexa = null;

    private string LastLoadedSkyrimStringsDictionnary = String.Empty;
    private string LastLoadedSkyrimEsmDictionnary = String.Empty;

    /// <summary>
    /// GetStringsDictionnaryPath
    /// </summary>
    /// <returns></returns>
    private string GetStringsDictionnaryPath()
    {
      string languageSource = cboxSourceLanguage.Text;
      string languageTarget = cboxTargetLanguage.Text;
      string appPath = Path.GetDirectoryName(Application.ExecutablePath);
      appPath = Path.Combine(appPath, @"Dict\");
      System.IO.Directory.CreateDirectory(appPath);
      return Path.Combine(appPath, @"Skyrim_" + languageSource + "_to_" + languageTarget + ".dicstr");
    }

    /// <summary>
    /// GenerateSkyrimStringsDictionary
    /// </summary>
    /// <param name="language"></param>
    /// <param name="dict"></param>
    /// <param name="stringType"></param>
    /// <param name="action"></param>
    public void GenerateSkyrimStringsDictionary(string language, LocalizedStringDict dict, string stringType, string action)
    {
      DataRow row = tblStrings.NewRow();
      string key;
      string primaryKey;
      DataRowView[] drv;

      if (action == "CREATE") { tblStrings.Rows.Clear(); if (tblStringsDV != null) tblStringsDV.Table = null; }

      if (tblStringsDV == null) tblStringsDV = new DataView();

      if (tblStringsDV.Table == null)
      {
        tblStringsDV.Table = tblStrings;
        tblStringsDV.Sort = "PK"; //PK= String ID + StringType (strings / dlstring /  ilstrings)
      }

      foreach (System.Collections.Generic.KeyValuePair<uint, string> kvp in dict)
      {
        try
        {
          key = kvp.Key.ToString("X8").ToUpperInvariant();
          primaryKey = key + "|" + stringType.ToUpperInvariant();

          /// Create new row
          if (language == cboxSourceLanguage.Text)
          {
            //if (key == "00000001")
            //  key = "00000001";

            drv = tblStringsDV.FindRows(new object[] { primaryKey });
            if (drv.Length < 1)
            {
              row = tblStrings.NewRow();
              row["PK"] = primaryKey;
              row["StringID"] = kvp.Key;
              row["StringIDHexa"] = key;
              //if (kvp.Value.Length > 200)
              //  row["SourceTextValue"] = kvp.Value.Substring(0, 200);
              //else
              row["SourceTextValue"] = kvp.Value;
              row["TargetTextValue"] = "?";
              row["StringType"] = stringType;
              tblStrings.Rows.Add(row);
            }
            else
            {
              drv[0]["TargetTextValue"] = kvp.Value;
            }
          }

          /// Add/Create new row
          if (language == cboxTargetLanguage.Text)
          {
            //if (key == "00000001")
            //  key = "00000001";

            drv = tblStringsDV.FindRows(new object[] { primaryKey });
            if (drv.Length < 1)
            {
              row = tblStrings.NewRow();
              row["PK"] = primaryKey;
              row["StringID"] = kvp.Key;
              row["StringIDHexa"] = kvp.Key.ToString("X8").ToUpperInvariant();
              row["SourceTextValue"] = "?";
              //if (kvp.Value.Length > 200)
              //  row["TargetTextValue"] = kvp.Value.Substring(0, 200);
              //else
              row["TargetTextValue"] = kvp.Value;
              row["StringType"] = stringType;
              tblStrings.Rows.Add(row);
            }
            else
            {
              drv[0]["TargetTextValue"] = kvp.Value;
            }
          }
        }
        catch (Exception crap)
        {
          edtMemo.Text += crap.Message + Environment.NewLine;
          return;
        }
      }

      if (action == "WRITE")
      {
        string appPathPlugin = GetStringsDictionnaryPath();
        if (File.Exists(appPathPlugin)) File.Delete(appPathPlugin);
        tblStrings.WriteXml(appPathPlugin, XmlWriteMode.WriteSchema);
        LastLoadedSkyrimStringsDictionnary = GetStringsDictionnaryPath();
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
      string sourceStringIDHexa;
      string stringType = String.Empty;
      string primaryKey;

      string appPath = Path.GetDirectoryName(Application.ExecutablePath);
      appPath = Path.Combine(appPath, @"Dict\");

      if (!LoadSkyrimStringsDictionnary()) return;

      string sourceLang = "????????";
      string targetLang = "????????";

      bool foundString;

      DataRowView[] foundRows;
      DataRow rowDict;

      if (tblStringsDV == null) tblStringsDV = new DataView();

      if (tblStringsDV.Table == null)
      {
        tblStringsDV.Table = tblStrings;
        tblStringsDV.Sort = "PK"; //PK= String ID + StringType (strings / dlstring /  ilstrings)
      } 

      try
      {
        tblSkyrimEsmDict.Rows.Clear();

        foreach (DataRow row in tblPlugInStringsLoad.Rows)
        {
          foundString = false;
          for (int stringTypeCpt = 1; stringTypeCpt <= 3; stringTypeCpt++)
          {
            sourceStringIDHexa = Convert.ToString(row["SourceStringIDHexa"]);

            sourceLang = String.Empty;
            targetLang = String.Empty;

            stringType = "Strings";
            if (stringTypeCpt == 2) stringType = "DLStrings";
            if (stringTypeCpt == 3) stringType = "ILStrings";

            primaryKey = sourceStringIDHexa + "|" + stringType;

            foundRows = tblStringsDV.FindRows(new object[] { primaryKey });
            if (foundRows.Length == 1)
            {
              sourceLang = foundRows[0]["SourceTextValue"].ToString();
              targetLang = foundRows[0]["TargetTextValue"].ToString();
              stringType = Convert.ToString(foundRows[0]["StringType"]);

              rowDict = tblSkyrimEsmDict.NewRow();
              rowDict["GroupName"] = row["GroupName"];
              rowDict["RecordType"] = row["RecordType"];
              rowDict["StringType"] = stringType; 
              rowDict["FormID"] = row["FormID"];
              rowDict["FormIDHexa"] = Convert.ToString(row["FormIDHexa"]).ToUpperInvariant();
              rowDict["EditorID"] = row["EditorID"];
              rowDict["SkyrimStringID"] = row["SourceStringID"];
              rowDict["SkyrimStringIDHexa"] = sourceStringIDHexa.ToUpperInvariant();
              rowDict["SkyrimItemDescSourceLang"] = sourceLang;
              rowDict["SkyrimItemDescTargetLang"] = targetLang;

              tblSkyrimEsmDict.Rows.Add(rowDict);
              foundString = true;
              break;
            }
          }

          if (foundString == false)
          {
            rowDict = tblSkyrimEsmDict.NewRow();
            rowDict["GroupName"] = row["GroupName"];
            rowDict["RecordType"] = row["RecordType"];
            rowDict["StringType"] = stringType; // rowV["StringType"]; // row["StringType"];
            rowDict["FormID"] = row["FormID"];
            rowDict["FormIDHexa"] = Convert.ToString(row["FormIDHexa"]).ToUpperInvariant();
            rowDict["EditorID"] = row["EditorID"];
            rowDict["SkyrimStringID"] = 0;
            rowDict["SkyrimStringIDHexa"] = 0.ToString("X8").ToUpperInvariant();
            rowDict["SkyrimItemDescSourceLang"] = "?";
            rowDict["SkyrimItemDescTargetLang"] = "?";
            tblSkyrimEsmDict.Rows.Add(rowDict);
          }

        }
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in ReadPlugInRecordsSubRecords ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
      }
      finally
      {
        string appPathPlugin = Path.Combine(appPath, "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic");
        tblSkyrimEsmDict.WriteXml(appPathPlugin, XmlWriteMode.WriteSchema);
        //tblSkyrimEsmDict.Rows.Clear();
        tblPlugInStringsLoad.Rows.Clear();
        GC.Collect();
      }
    }

    /// <summary>
    /// LoadSkyrimStringsDictionnary
    /// </summary>
    private bool LoadSkyrimStringsDictionnary()
    {
      string appPathPlugin = GetStringsDictionnaryPath();

      if (!File.Exists(appPathPlugin))
      {
        MessageBox.Show("Dictionnary doesn't exist." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }

      try
      {
        tblStrings.Rows.Clear();
        tblStrings.ReadXml(appPathPlugin);

        if (tblStrings.Rows.Count == 0)
        {
          edtMemo.Text += Environment.NewLine + "****** Strings file is empty." + Environment.NewLine + appPathPlugin;
          MessageBox.Show("Dictionnary file is empty." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;
        }
        else
        {
          if (tblStringsDV == null) tblStringsDV = new DataView();

          if (tblStringsDV.Table == null)
          {
            tblStringsDV.Table = tblStrings;
            tblStringsDV.Sort = "PK"; //PK= String ID + StringType (strings / dlstring /  ilstrings)
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in LoadSkyrimStringsDictionnary ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
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

      if (tblSkyrimEsmDict == null) return false;

      if (tblSkyrimEsmDict.Rows.Count > 0)
        if (LastLoadedSkyrimEsmDictionnary == "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic") return true;

      string appPath = Path.GetDirectoryName(Application.ExecutablePath);
      appPath = Path.Combine(appPath, @"Dict\");
      string appPathPlugin = Path.Combine(appPath, "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic");

      if (!File.Exists(appPathPlugin))
      {
        MessageBox.Show("Dictionnary file doesn't exist." + Environment.NewLine + appPathPlugin, "Translator Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }

      try
      {
        tblSkyrimEsmDict.ReadXml(appPathPlugin);
        edtMemo.Text += Environment.NewLine + "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic : " + tblSkyrimEsmDict.Rows.Count.ToString() + " lines";

        LastLoadedSkyrimEsmDictionnary = "Skyrim_" + languageSource + "_to_" + languageTarget + ".dic";

        return true;
      }
      catch (Exception ex)
      {
        edtMemo.Text += Environment.NewLine + "****** ERROR in LoadSkyrimEsmDictionnary ******" + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
        return false;
      }
    }

  }
}