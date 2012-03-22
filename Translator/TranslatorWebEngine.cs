using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace TESVSnip.Translator
{
  public static class StringExtensions
  {
    public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
    {
      int startIndex = 0;
      while (true)
      {
        startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
        if (startIndex == -1)
          break;

        originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

        startIndex += newValue.Length;
      }

      return originalString;
    }
  }

  public class TranslatorWebEngine
  {
    private System.Text.Encoding iso = System.Text.Encoding.GetEncoding(1252);
    private System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

    private Int32 _codePage = 1252;
    public Int32 CodePage { get { return _codePage; } set { _codePage = value; iso = System.Text.Encoding.GetEncoding(_codePage); } }

    private string _sourceLang = "en";
    public string SourceLang { get { return _sourceLang; } set { _sourceLang=value; } }

    private string _targetLang = "fr";
    public string TargetLang { get { return _targetLang; } set { _targetLang=value; } }

    /// <summary>
    /// Translates a text using screenscaping on Google Translate
    /// http://hsp.dk/2011/01/unofficial-google-translate-in-c-and-vb-net/
    /// Unofficial Google Translate in C# and VB.NET
    /// Posted: 12-01-2011 | Author: hspsoftware | Category: .NET, Hacks, Tutorials
    /// Henrik Pedersen's blog : http://hsp.dk
    /// </summary>
    /// <param name="input">The string to translate</param>
    /// <returns>Translated text</returns>
    public string TranslateText(string input)
    {
      string langFrom = _sourceLang; //The language to translate from. Fx "en" for English or "da" for Danish
      string langTo = _targetLang; //The language to translate to in the same format as langFrom

      //Defines a new WebClient
      WebClient Client = new WebClient();
      //Sets the client encoding to UTF8
      Client.Headers.Add("Charset", "text/html; charset=UTF-8");
      //Creates the string. And yes I prefer this over string.format ! ;)
      string downloadUrl = "http://www.google.com/translate_t?hl=da&ie=UTF8&text=" + input + "&langpair=" + langFrom + "|" + langTo;
      //Downloads the string from the URL above
      string data = Client.DownloadString(downloadUrl);
      //Searches for the beginning of the resultbox and cuts everything away before that
      data = data.Substring(data.IndexOf("<span id=result_box") + 19);
      //Finds the ending of the resultbox by searching for two spans right after each other
      data = data.Remove(data.IndexOf("</span></span>") + 7);
      //Defines a new regex used for counting all spans inside the resultbox
      Regex spans = new Regex("<span");
      //Finds the count and puts it inside the variable spanOccurences
      int spanOccurences = spans.Matches(data).Count;
      //Defines an empty string for use in the for loop
      string translatedText = "";
      //Extract each tiny bit of text from each span in the resultbox
      for (int i = 0; i < spanOccurences; i++)
      {
        //Defines currentBlock and sets it to everything which comes after the first "<span"
        string currentBlock = data.Substring(data.IndexOf("<span") + 5);
        //Finds the ending of the current span and removes everything after that
        currentBlock = currentBlock.Remove(currentBlock.IndexOf("</span>"));
        //Goes back to the beginning and cleans everything from inside the first span
        currentBlock = currentBlock.Substring(currentBlock.IndexOf(">") + 1);
        //Removes the current processed span from the beginning of the data for next extraction
        data = data.Substring(data.IndexOf("</span>") + 7);
        //Adds the extracted text to the translatedText variable
        translatedText += currentBlock;
      }

      byte[] utfBytes = utf8.GetBytes(translatedText);
      byte[] isoBytes = System.Text.Encoding.Convert(utf8, iso, utfBytes);
      string textIso = iso.GetString(isoBytes);
      translatedText = HttpUtility.HtmlDecode(textIso).Replace("<br>", Environment.NewLine, StringComparison.InvariantCultureIgnoreCase);

      //Returns the translated text
      return translatedText;
    }

  }
}
