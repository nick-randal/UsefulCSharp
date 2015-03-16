Author xml faster by leaving out all the redundancy and noisy syntax.  
Quick XML uses *leading whitespace* to define how the document hierarchy is built.  The number of whitespace (tabs or spaces) is up to you, as long as the child items have more than the parent.
Attributes, inner text and CData are defined after the element they belong to at the same indentation level.

- Attributes - key[space]value
- Inner text - enclosed in "text"
- CData - enclosed in [data]
- Comments - marked with !comment

The following code can be plugged in to Linqpad.
Add the nuget package **Randal.QuickXml**.
```csharp
var qxml = @"
Directory
	!Super cool
	People
		Person
		Name Bob
		Age 32
		Person
		Name Sue
		Age 29
			Ambition
			""To foster world peace""
			Data
			[123456789abcdef]
	Test
	xmlns:g http://test.com
		g:Apples
		g:Bananas Are not appples
".Trim();

var parser = new QuickXmlParser();
parser.ParseToXElement(qxml).Dump();
```
and this will be generated
```xml
<Directory>
  <!--Super cool-->
  <People>
    <Person Name="Bob" Age="32" />
    <Person Name="Sue" Age="29">
      <Ambition>To foster world peace</Ambition>
      <Data><![CDATA[123456789abcdef]]></Data>
    </Person>
  </People>
  <Test xmlns:g="http://test.com">
    <g:Apples g:Bananas="Are not appples" />
  </Test>
</Directory>
```


Example 1 - Generate Quick XML from XML
```csharp
var gen = new QuickXmlGenerator();

using(var writer = new StringWriter())
{
	var xml = XDocument.Parse(text);
	gen.GenerateQuickXml(writer, xml);
	var qxml = writer.ToString();
}

// OR

using(var writer = new StringWriter())
{
	var xml = XElement.Parse(text);
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

// OR

using(var reader = new StringReader(qxml))
{
	var xml = parser.ParseToXElement(reader);
}
```
