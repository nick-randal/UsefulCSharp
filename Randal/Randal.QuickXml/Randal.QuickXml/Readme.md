Author xml faster by leaving out all the redundancy and noisy syntax.  
Quick XML uses leading whitespace to define the document hierarchy is built.
Attributes, inner text and CData are defined after the element they belong to.

Attributes - key<space>value<eol>
Inner text - enclosed in "text"
CData - enclosed in [data]
Comments - marked with !comment

Example 1 - Generate Quick XML from XML
```csharp
var xml = XDocument.Parse(text);
var gen = new QuickXmlGenerator();

using(var writer = new StringWriter())
{
	gen.GenerateQuickXml(writer, xml);
	var qxml = writer.ToString();
}
```

Example 2 - Parse Quick XML to XElement or XDocument
```csharp
var parser = new QuickXmlParser();

using(var reader = new StringReader(qxml))
{
	var xml = parser.ParseToXDocument(reader);
}

using(var reader = new StringReader(qxml))
{
	var xml = parser.ParseToXElement(reader);
}
```
