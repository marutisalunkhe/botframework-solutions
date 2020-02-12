// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Resources;
using System.Xml.Linq;

namespace PointOfInterestSkill.Utilities
{
    public class ResxToLg
    {
        // TODO consider multi language, path etc.
        public static void Convert(string path, string fileIn, string fileEntry)
        {
            // TODO remove New
            var fileOut = fileIn + "New";
            var doc = XDocument.Load(Path.Join(path, fileIn + ".resx"));
            var nodes = doc.Root.Elements("data");
            using (var activityFile = new StreamWriter(Path.Join(path, fileOut + ".lg")))
            using (var textFile = new StreamWriter(Path.Join(path, fileOut + "Texts.lg")))
            {
                textFile.WriteLine($"[import]({fileOut}.lg)");
                foreach (var node in nodes)
                {
                    var name = node.Attribute("name").Value;
                    var value = node.Element("value").Value;
                    activityFile.WriteLine(@$"# {name}(Data, Cards, Layout)
[Activity
Text = @{{{name}.Text(Data)}}
Speak = @{{{name}.Text(Data)}}
Attachments = @{{if(Cards==null,null,foreach(Cards, Card, CreateCard(Card)))}}
AttachmentLayout = @{{if(Layout==null,'list',Layout)}}
]
");
                    textFile.WriteLine($@"# {name}.Text(Data)
- {value}
");
                }
            }

            // TODO remove New
            using (var ttFile = new StreamWriter(Path.Join(path, fileOut + ".tt")))
            {
                ttFile.Write(@"<#@ template debug=""false"" hostspecific=""true"" language=""C#"" #>
<#@ output extension="".cs"" #>
<#@ include file=""..\Shared\LgIdCollection.t4""#>");
            }

            using (var entryFile = new StreamWriter(Path.Join(path, fileEntry + ".lg"), true))
            {
                entryFile.WriteLine($"[import]({fileOut}Texts.lg)");
            }
        }
    }
}
