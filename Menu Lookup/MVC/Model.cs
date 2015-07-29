using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Menu_Lookup.DTO;
using Menu_Lookup.Settings;

namespace Menu_Lookup.MVC
{
  public class Model
  {
    private IList<MenuItem> _menuItems;
    private readonly string _menuPath = Path.Combine(Options.CodeFolder, @"Caselle\Menu\");
    private const string Menu = "Caselle.Csl.MVC.Menu.xml";
    private const string MenuModules = "Caselle.Csl.MVC.MenuModules.xml";

    public IList<MenuItem> MenuItems
    {
      get { return _menuItems ?? (_menuItems = GetMenuItems()); }
    }

    private IList<MenuItem> GetMenuItems()
    {
      var items = GetProtoItems();
      FillItems(items);
      return items.Where(x => x.Descriptions.Count > 0).ToList();
    }

    private void FillItems(IEnumerable<MenuItem> items)
    {
      var dictionary = items.ToDictionary(i => i.ViewKey, i => i);

      XmlReader xr = new XmlTextReader(Path.Combine(_menuPath, Menu));
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
      XmlReader xr = new XmlTextReader(Path.Combine(_menuPath, MenuModules));
      while (xr.Read())
      {
        if (!xr.IsStartElement()) continue;
        if (xr.Name != "Application") continue;

        var key = xr.GetAttribute("Key");
        var path = xr.GetAttribute("Path");
        var name = xr.GetAttribute("ControlName");
        var model = xr.GetAttribute("Model");
        var zoomAttribute = xr.GetAttribute("IsZoomable");

        items.Add(MenuItem.GenerateProtoItem(key, path, name, model, zoomAttribute));
      }

      return items;
    }
  }
}
