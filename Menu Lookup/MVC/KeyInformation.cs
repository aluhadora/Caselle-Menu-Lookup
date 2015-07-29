using System;
using System.Drawing;
using System.Windows.Forms;
using Menu_Lookup.Utilities;
using MenuItem = Menu_Lookup.DTO.MenuItem;

namespace Menu_Lookup.MVC
{
  public partial class KeyInformation : Button
  {
    private MenuItem _item;
    private bool _active;
    public event Action<MenuItem, bool, bool> SolutionClicked;

    public KeyInformation()
    {
      InitializeComponent();
      FlatStyle = FlatStyle.Flat;
      Click += ClickLink;
      //FlatAppearance.BorderColor = Color.Blue;
      //FlatAppearance.BorderSize = 3;
    }

    private void ClickLink(object sender, EventArgs e)
    {
      ClickLink();
    }

    public bool Active
    {
      get { return _active; }
      set
      {
        _active = value;
        Invalidate();
      }
    }

    public bool IgnoreNextClick { get; set; }

    public void LoadItem(MenuItem item)
    {
      _item = item;
      if (item == null)
      {
        Clear();
        return;
      }

      keyLabel.Text = item.ViewKey;
      assemblyLabel.Text = item.Assembly;
      viewLabel.Text = item.ControlName;
      modelLabel.Text = item.Model;
      zoomLabel.Text = item.IsZoomable ? "Yes" : "No";
      solutionLinkLabel.Text = item.SolutionName;
      var solutionExists = !item.SolutionName.IsNullOrTrimmedEmpty();
      solutionPictureBox.Visible = solutionExists;
      if (solutionExists)
      {
        solutionPictureBox.BackgroundImage = IconUtility.IconForPath(item.SolutionPath);
      }
    }

    public void Clear()
    {
      keyLabel.Text = string.Empty;
      assemblyLabel.Text = string.Empty;
      viewLabel.Text = string.Empty;
      modelLabel.Text = string.Empty;
      zoomLabel.Text = string.Empty;
    }

    private void solutionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      if (SolutionClicked == null) return;
      if (e.Button == MouseButtons.Middle) SolutionClicked(_item, false, false);
      if (e.Button == MouseButtons.Left) SolutionClicked(_item, true, false);
      if (e.Button == MouseButtons.Right) SolutionClicked(_item, false, true);
    }

    private void solutionPictureBox_Click(object sender, EventArgs e)
    {
      if (IgnoreNextClick)
      {
        IgnoreNextClick = false;
        return;
      }
      if (SolutionClicked != null) SolutionClicked(_item, true, false);
    }

    private void viewLabel_Click(object sender, EventArgs e)
    {
      MenuItem.CopyItemProperty(_item.ControlName);
    }

    private void modelLabel_Click(object sender, EventArgs e)
    {
      MenuItem.CopyItemProperty(_item.Model);
    }

    private void KeyInformation_Paint(object sender, PaintEventArgs e)
    {
      var bottomWidth = 3;
      if (Focused)
      {
        ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                  Color.Blue, bottomWidth, ButtonBorderStyle.Inset,
                  Color.Blue, bottomWidth, ButtonBorderStyle.Inset,
                  Color.Blue, bottomWidth, ButtonBorderStyle.Inset,
                  Color.Blue, bottomWidth, ButtonBorderStyle.Inset);
      }

    }

    public void ClickLink()
    {
      if (IgnoreNextClick)
      {
        IgnoreNextClick = false;
        return;
      }
      if (SolutionClicked != null) SolutionClicked(_item, true, false);
    }
  }
}
