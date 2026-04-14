using System.Xml.Linq;

var doc = XDocument.Load("products.xml");

var products = doc.Root!
    .Elements("Product")
    .Select(p => new
    {
        Id = (int)p.Element("Id")!,
        Name = (string)p.Element("Name")!,
        Category = (string)p.Element("Category")!,
        Price = (decimal)p.Element("Price")!,
        InStock = (bool)p.Element("InStock")!
    });