using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaselleProfiles.Processes;
using Menu_Lookup.MVC;

namespace Menu_Lookup
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var profiles = ProfilesProcess.Load();
      if (!profiles.Any())
      {
        MessageBox.Show("Run Caselle Profiles");
        return;
      }

      var profile = profiles.FirstOrDefault(x => args.Contains(x.Name)) ?? profiles.First();

      var model = new Model {DefaultProfile = profile};
      Application.Run(new MainForm(model));
    }
  }
}
