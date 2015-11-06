using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using CaselleProfiles.DTO;
using MenuItem = Menu_Lookup.DTO.MenuItem;

namespace Menu_Lookup.MVC
{
  public class Model
  {
    private IList<MenuItem> _menuItems;
    private IView _view;
    private const string Menu = "Caselle.Csl.MVC.Menu.xml";
    private const string MenuModules = "Caselle.Csl.MVC.MenuModules.xml";

    private string MenuPath
    {
      get { return Path.Combine(View.Profile.Directory, @"Caselle\Menu\"); }
    }

    public IList<MenuItem> MenuItems
    {
      get { return _menuItems ?? (_menuItems = GetMenuItems()); }
    }

    public IView View
    {
      get { return _view; }
      set
      {
        _view = value;
        _view.Profile = DefaultProfile;
      }
    }

    public Profile DefaultProfile { get; set; }

    private IList<MenuItem> GetMenuItems()
    {
      var items = GetProtoItems();
      FillItems(items);
      return items.Where(x => x.Descriptions.Count > 0).ToList();
    }

    private void FillItems(IEnumerable<MenuItem> items)
    {
      var dictionary = items.ToDictionary(i => i.ViewKey, i => i);

      XmlReader xr = new XmlTextReader(Path.Combine(MenuPath, Menu));
      while (xr.Read())
      {
        if (!xr.IsStartElement()) continue;
        if (xr.Name != "Application") continue;

        var key = xr.GetAttribute("Key");
        var description = xr.GetAttribute("Description");

        if (key == null) continue;
        dictionary[key].Descriptions.Add(description);
      }
    }

    private IList<MenuItem> GetProtoItems()
    {
      var items = new List<MenuItem>();
      XmlReader xr = new XmlTextReader(Path.Combine(MenuPath, MenuModules));
      while (xr.Read())
      {
        if (!xr.IsStartElement()) continue;
        if (xr.Name != "Application") continue;

        var key = xr.GetAttribute("Key");
        var path = xr.GetAttribute("Path");
        var name = xr.GetAttribute("ControlName");
        var model = xr.GetAttribute("Model");
        var zoomAttribute = xr.GetAttribute("IsZoomable");

        items.Add(MenuItem.GenerateProtoItem(_view.Profile, key, path, name, model, zoomAttribute));
      }

      return items;
    }

    public void Reload()
    {
      _menuItems = null;
      if (!MenuItems.Any()) MessageBox.Show("Bad things");
    }
  }
}
