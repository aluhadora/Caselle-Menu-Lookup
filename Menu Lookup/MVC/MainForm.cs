using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CaselleProfiles.DTO;
using Menu_Lookup.Settings;
using Menu_Lookup.Utilities;
using MenuItem = Menu_Lookup.DTO.MenuItem;

namespace Menu_Lookup.MVC
{
  public partial class MainForm : Form, IView
  {
    private readonly Model _model;
    private IEnumerable<string> _descriptions;

    public MainForm(Model model)
    {
      _model = model;
      InitializeComponent();

      profileSelector1.ProfileChanged += ProfileChanged;
      _model.View = this;
    }

    private void ProfileChanged(Profile profile)
    {
      _model.Reload();
      comboBox1_SelectedIndexChanged(this, new EventArgs());
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      var descriptions = _model.MenuItems.Select(x => x.Module.Trim('0').UppercaseFirst() + " " + x.Description).OrderBy(x => x).Distinct().ToList();
      descriptions.AddRange(_model.MenuItems.SelectMany(x => x.Descriptions).OrderBy(x => x).Distinct().ToList());
      _descriptions = descriptions;
      comboBox1.Items.AddRange(_descriptions.Cast<object>().ToArray());

      comboBox1.Focus();

      KeyPreview = true;
      RecursiveSubscribeToKeyHandling(this);
    }

    private void RecursiveSubscribeToKeyHandling(Control control)
    {
      control.PreviewKeyDown += KeyPressed;
      control.ControlAdded += SubscribeControl;
      foreach (Control c in control.Controls)
      {
        RecursiveSubscribeToKeyHandling(c);
      }
    }

    private void KeyPressed(object sender, PreviewKeyDownEventArgs e)
    {
      var keys = new List<KeyInformation>();
      foreach (KeyInformation key in panel1.Controls)
      {
        keys.Add(key);
      }

      if (e.KeyCode == Keys.Down)
      {
        if (ActiveControl == comboBox1) return;
        if (ActiveControl == profileSelector1) return;
        if (keys.Count(x => x.Active) != 1) return;
        SelectNextControl(keys.First(x => x.Active), true, true, false, true);
        SelectKey(ActiveControl as KeyInformation);
      }
      else if (e.KeyCode == Keys.Up)
      {
        if (ActiveControl == comboBox1) return;
        if (ActiveControl == profileSelector1) return;
        if (keys.Count(x => x.Active) != 1) return;
        SelectNextControl(keys.First(x => x.Active), false, true, false, true);
        SelectKey(ActiveControl as KeyInformation);
      }
      else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
      {
        if (ActiveControl == comboBox1)
        {
          keyInformation1.IgnoreNextClick = true;
          SelectKey(keyInformation1);
          keyInformation1.Invalidate();
        }
        else
        {
          if (keys.Count(x => x.Active) != 1) return;
          keys.First().ClickLink();
        }
      }
      else if (e.KeyCode == Keys.Escape)
      {
        if (ActiveControl != comboBox1)
        {
          comboBox1.Focus();
          ActiveControl = comboBox1;
          return;
        }
        if (comboBox1.Text.IsNullOrTrimmedEmpty())
        {
          Close();
          return;
        }
        comboBox1.Text = string.Empty;
      }
    }

    private void SubscribeControl(object sender, ControlEventArgs e)
    {
      ((Control)sender).PreviewKeyDown += KeyPressed;
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (comboBox1.SelectedItem == null) return;
      var description = comboBox1.SelectedItem.ToString();
      var matchingKeys = _model.MenuItems.Where(x => x.Descriptions.Contains(description)).OrderBy(x => x.Module).ToList();
      if (!matchingKeys.Any() && description.Length > 4)
      {
        var moduleRemoved = description;
        moduleRemoved = moduleRemoved.Substring(moduleRemoved.IndexOf(" ") + 1);
        matchingKeys =
          _model.MenuItems.Where(
            x => x.Descriptions.Contains(moduleRemoved) && x.Module.ToUpper().StartsWith(description.ToUpper().Substring(0, 2)))
            .OrderBy(x => x.Module)
            .ToList();
      }

      keyInformation1.LoadItem(matchingKeys.FirstOrDefault());
      for (int i = 1; i < matchingKeys.Count(); i++)
      {
        var name = string.Format("key{0}", i);
        if (panel1.Controls.ContainsKey(name))
        {
          var info = panel1.Controls[name] as KeyInformation;
          if (info == null) continue;
          info.LoadItem(matchingKeys[i]);
        }
        else
        {
          var info = new KeyInformation
          {
            Name = name,
            Left = keyInformation1.Left,
            Top = panel1.Controls.Cast<Control>().Max(x => x.Bottom) + 6,
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
          };
          info.LoadItem(matchingKeys[i]);
          info.SolutionClicked += LoadSolution;
          panel1.Controls.Add(info);
        }
      }
      for (int i = matchingKeys.Count(); i < 9999; i++)
      {
        var name = string.Format("key{0}", i);
        if (panel1.Controls.ContainsKey(name))
        {
          panel1.Controls.RemoveByKey(name);
        }
      }

      SelectKey(keyInformation1);
    }

    private void SelectKey(KeyInformation info)
    {
      if (info == null) return;

      foreach (KeyInformation key in panel1.Controls)
      {
        key.Active = key == info;
        if (key.Active)
        {
          Focus();
          ActiveControl = key;
        }
      }
    }

    private void LoadSolution(MenuItem item, bool closeAfter, bool edit)
    {
      if (item == null || item.SolutionPath == string.Empty) return;
      const string vsPath = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe";
      const string notepadPath = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
      Process.Start(edit ? notepadPath : vsPath, item.SolutionPath);
      if (closeAfter)
      {
        MenuItem.CopyItemProperty(item.ControlName);
        Close();
      }
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
      comboBox1.Focus();
    }

    #region Implementation of IView

    public Profile Profile
    {
      get { return profileSelector1.CurrentProfile; }
      set { profileSelector1.ProfileName = value != null ? value.Name : string.Empty; }
    }

    #endregion
  }
}
