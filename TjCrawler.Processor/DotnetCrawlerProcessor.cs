using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TjCrawler.Domain.Attributes;

namespace DotnetCrawler.Processor
{
    public class DotnetCrawlerProcessor<TEntity> : IDotnetCrawlerProcessor<TEntity> where TEntity : class
    {
        public async Task<IEnumerable<TEntity>> Process(HtmlDocument document)
        {
            var nameValueDictionary = GetColumnNameValuePairsFromHtml(document);

            var processorEntity = ReflectionHelper.CreateNewEntity<TEntity>();
            foreach (var pair in nameValueDictionary)
            {
                ReflectionHelper.TrySetProperty(processorEntity, pair.Key, pair.Value);
            }

            return new List<TEntity>
            {
                processorEntity as TEntity
            };
        }

        public async Task<IEnumerable<TEntity>> ProcessForm(HtmlDocument document)
        {
            var nameValueDictionary = GetLabelValuePairsFromHtml(document);

            var processorEntity = ReflectionHelper.CreateNewEntity<TEntity>();
            foreach (var pair in nameValueDictionary)
            {
                ReflectionHelper.TrySetProperty(processorEntity, pair.Key, pair.Value);
            }

            return new List<TEntity>
            {
                processorEntity as TEntity
            };
        }

        private static Dictionary<string, object> GetColumnNameValuePairsFromHtml(HtmlDocument document)
        {
            var columnNameValueDictionary = new Dictionary<string, object>();

            var entityExpression = ReflectionHelper.GetEntityExpression<TEntity>();
            var propertyExpressions = ReflectionHelper.GetPropertyAttributes<TEntity>();

            var entityNode = document.DocumentNode.SelectSingleNode(entityExpression);

            if(entityNode == null)
            {
                var altExpression = ReflectionHelper.GetEntityAltExpression<TEntity>();
                entityNode = document.DocumentNode.SelectSingleNode(altExpression);
            }

            foreach (var expression in propertyExpressions)
            {
                var columnName = expression.Key;
                object columnValue = null;
                var fieldExpression = expression.Value.Item2;

                switch (expression.Value.Item1)
                {
                    case SelectorType.XPath:
                        var node = entityNode.SelectSingleNode(fieldExpression);
                        if (node != null)
                            columnValue = node.InnerText;
                        break;
                    case SelectorType.CssSelector:
                        var nodeCss = entityNode.QuerySelector(fieldExpression);
                        if (nodeCss != null)
                            columnValue = nodeCss.InnerText;
                        break;
                    case SelectorType.RawText:
                        var nodeRawText = entityNode.QuerySelector(fieldExpression);
                        if (nodeRawText != null)
                            columnValue = nodeRawText.InnerText;
                        break;
                    case SelectorType.FixedValue:
                        if (Int32.TryParse(fieldExpression, out var result))
                        {
                            columnValue = result;
                        }
                        break;
                    default:
                        break;
                }
                columnNameValueDictionary.Add(columnName, columnValue);
            }

            return columnNameValueDictionary;
        }

        private static Dictionary<string, Dictionary<string, List<object>>> GetLabelValuePairsFromHtml(HtmlDocument document)
        {
            var labelValueDictionary = new Dictionary<string, Dictionary<string, List<object>>>();

            var entityExpression = ReflectionHelper.GetEntityExpression<TEntity>();
            var propertyExpressions = ReflectionHelper.GetPropertyAttributes<TEntity>();

            var entityNode = document.DocumentNode.SelectSingleNode(entityExpression);

            if (entityNode == null)
            {
                var altExpression = ReflectionHelper.GetEntityAltExpression<TEntity>();
                entityNode = document.DocumentNode.SelectSingleNode(altExpression);
            }

            foreach (var expression in propertyExpressions)
            {
                var columnName = expression.Key;
                Dictionary<string, List<object>> dictionaryValue = null;
                var fieldExpression = expression.Value.Item2;

                switch (expression.Value.Item1)
                {
                    case SelectorType.XPath:
                        var nodeXPath = entityNode.SelectNodes(fieldExpression);
                        if (nodeXPath != null && nodeXPath.Count > 0)
                        {
                            foreach (var labelNode in nodeXPath)
                            {
                                var valueNode = entityNode.SelectSingleNode(labelNode.XPath).NextSibling;

                                if (valueNode != null)
                                {
                                    //implementation
                                }
                            }
                        }
                        break;
                    case SelectorType.CssSelector:
                        var nodeCss = entityNode.QuerySelectorAll(fieldExpression);
                        if (nodeCss != null && nodeCss.Count > 0)
                        {
                            dictionaryValue = new Dictionary<string, List<object>>();

                            foreach (var labelNodeCss in nodeCss)
                            {
                                var valueNodeCss = entityNode.SelectSingleNode(labelNodeCss.XPath).NextSibling;

                                if(valueNodeCss != null)
                                {
                                    List<object> list;

                                    if (!dictionaryValue.TryGetValue(labelNodeCss.InnerText, out list))
                                    {
                                        list = new List<object>();
                                        dictionaryValue.Add(labelNodeCss.InnerText, list);
                                    }

                                    list.Add(valueNodeCss.InnerText);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                labelValueDictionary.Add(columnName, dictionaryValue);
            }

            return labelValueDictionary;
        }

        public Task<IEnumerable<TEntity>> ProcessarDados(HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> ProcessarPartes(HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> ProcessarMovimentacoes(HtmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
