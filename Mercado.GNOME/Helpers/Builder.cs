using System.IO;
using System.Reflection;
using System.Xml;

namespace Mercado.GNOME.Helpers;

public class Builder {
    public static Gtk.Builder FromFile(string name) {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        using StreamReader reader = new(stream!);
        string uiContents = reader.ReadToEnd();
        //  XmlDocument xml = new();
        //  xml.LoadXml(uiContents);
        // XmlNodeList elements = xml.GetElementsByTagName("*");
        // foreach (XmlElement element in elements) {
        //     if (element.HasAttribute("translatable")) {
        //         element.RemoveAttribute("translatable");
        //         if (element.HasAttribute("context")) {
        //             string context = element.GetAttribute("context");
        //             element.InnerText = element.InnerText;
        //         }
        //         else {
        //             element.InnerText = element.InnerText;
        //         }
        //     }
        // }
        // return Gtk.Builder.NewFromString(xml.OuterXml, -1);
        return Gtk.Builder.NewFromString(uiContents, -1);
    }
}