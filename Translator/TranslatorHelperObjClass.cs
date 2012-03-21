using System;
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
using System.Diagnostics;

namespace TESVSnip.Docking
{
  public partial class TranslatorHelper : BaseDockContent
  {
    private System.Collections.Generic.List<ObjStrings> listViewStrings = null;
    private System.Collections.Generic.List<ObjStrings> listViewStringsDL = null;
    private System.Collections.Generic.List<ObjStrings> listViewStringsIL = null;
    private System.Collections.Generic.List<ObjStrings> listViewStringsOther = null;

    private System.Collections.Generic.List<ObjStringsDict> listViewSkyrimDict = null;

    private System.Collections.Generic.List<ObjOtherSkyrimStrings> listViewOtherSkyrimStringsSource = null;
    private System.Collections.Generic.List<ObjOtherSkyrimStrings> listViewOtherSkyrimStringsTarget = null;
  }

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
