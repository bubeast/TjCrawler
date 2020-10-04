using System;
using System.Collections.Generic;
using System.Text;

namespace TjCrawler.Domain.Attributes
{
    /// <summary>
    /// Selector type of given attribute
    /// </summary>
    public enum SelectorType
    {
        XPath,
        CssSelector,
        FixedValue,
        RawText
    }

    public enum ScrappingMode
    {
        Table,
        Forms
    }
}
