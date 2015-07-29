using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Menu_Lookup.Settings;
using Menu_Lookup.Utilities;

namespace Menu_Lookup.DTO
{
  public class MenuItem
  {
    public string Module { get; set; }
    public string ModuleSolution { get; set; }
    public int Number { get; set; }
    public string ViewKey
    {
      get { return string.Format("{0}-{1:000}", Module, Number); }
    }
    public string Assembly { get; set; }
    public string ControlName { get; set; }
    public string Model { get; set; }
    public bool IsZoomable { get; set; }
    public IList<string> Descriptions { get; set; }
    public string Description { get { return Descriptions.FirstOrDefault(); } }
    
    public string SolutionName
    {
      get
      {
        var path = SolutionPath;
        if (SolutionPath.IsNullOrTrimmedEmpty()) return string.Empty;
        if (!File.Exists(path)) return string.Empty;
        return Path.GetFileName(path);
      }
    }

    private string SubModule
    {
      get
      {
        var asm = Assembly.ToUpper();
        var phoenix = asm.Contains(".MVC.DLL");
        if (phoenix) return string.Empty;
        if (asm.Contains(".MISCELLANEOUS.DLL")) return "Misc";
        if (asm.Contains(".DATAENTRY.DLL")) return "Data";
        if (asm.Contains(".REPORT.DLL")) return "Rpt";
        if (asm.Contains(".MAINTENANCE.DLL")) return "Maint";
        if (asm.Contains(".INQUIRY.DLL")) return "Inq";
        return "UNKWN";
      }
    }

    public string SolutionPath
    {
      get
      {
        var phoenix = Assembly.ToUpper().Contains(".MVC.DLL");
        var startingPath = Path.Combine(Options.CodeFolder, @"MasterSln");
        var module = ModuleSolution.UppercaseFirst();

        if (phoenix)
        {
          if (module.Length == 3 && module != "Aaa" && module != "Aas") module = module.Substring(0, 2) + "0";
          var solutionFolder = Path.Combine(startingPath, "Caselle." + module);
          var solution = Path.Combine(solutionFolder, "Caselle." + module + ".sln");
          if (File.Exists(solution)) return solution;
          return string.Empty;
        }
        
        var moduleSolution = Path.Combine(startingPath, module + "Sln");
        var submodule = module + SubModule + "Sln";
        var subModuleSolution = Path.Combine(moduleSolution, submodule);
        var file = Path.Combine(subModuleSolution, submodule + ".sln");
        if (File.Exists(file)) return file;
        if (submodule == "Bp0RptSln" && module == "Bp0") file = Path.Combine(subModuleSolution, "Caselle.Bp0.Report.Main.sln");
        if (File.Exists(file)) return file;
        return string.Empty;
      }
    }

    public static MenuItem GenerateProtoItem(string key, string path, string name, string model, string zoomAttribute)
    {
      var item = new MenuItem();
      item.Descriptions = new List<string>();
      var keyParts = key.Split('-');
      if (keyParts.Count() == 2)
      {
        item.Module = keyParts[0];
        item.ModuleSolution = keyParts[0];
        item.Number = Convert.ToInt32(keyParts[1]);
      }
      item.Assembly = path;
      if (item.Module.ToLower() == "aaa" && !item.Assembly.IsNullOrTrimmedEmpty())
      {
        if (item.Assembly.Contains("MVC"))
        {
          var parts = item.Assembly.Split('.');
          if (parts.Count() == 4 && parts[0] == "Caselle" && parts[2] == "MVC" && parts[1].Length == 3)
          {
            item.ModuleSolution = parts[1].ToLower();
          }
        }
        else if (item.Assembly.Contains("UI"))
        {
          var parts = item.Assembly.Split('.');
          if (parts.Count() == 5 && parts[0] == "Caselle" && parts[2] == "UI" && parts[1].Length == 3)
          {
            item.ModuleSolution = parts[1].ToLower();
          }
        }
      }
      item.ControlName = name;
      item.Model = model;
      item.IsZoomable = zoomAttribute == "True";
      return item;
    }

    public static void CopyItemProperty(string property)
    {
      if (!property.Contains(".")) return;
      Clipboard.SetText(property.Substring(property.LastIndexOf(".") + 1));
    }
  }
}
