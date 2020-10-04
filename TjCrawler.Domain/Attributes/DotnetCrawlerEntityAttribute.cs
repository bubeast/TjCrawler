using System;
using System.Collections.Generic;
using System.Text;

namespace TjCrawler.Domain.Attributes
{
    public class DotnetCrawlerEntityAttribute : Attribute
    {
        public string XPath { get; set; }
        public string XPathAlt { get; set; }
    }    
}
