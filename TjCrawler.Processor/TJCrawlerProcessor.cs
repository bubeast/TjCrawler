using TjCrawler.Domain.Attributes;
using TjCrawler.Domain.Models.Processo;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotnetCrawler.Processor
{
    public class TJCrawlerProcessor<TEntity> : IDotnetCrawlerProcessor<TEntity> where TEntity : class
    {
        public async Task<IEnumerable<TEntity>> ProcessarDados(HtmlDocument document)
        {
            var nameValueDictionary = GetDadosProcessoFromHtml(document);

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

        private static Dictionary<string, object> GetDadosProcessoFromHtml(HtmlDocument document)
        {
            var columnNameValueDictionary = new Dictionary<string, object>();

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
                object columnValue = null;
                var fieldExpression = expression.Value.Item2;

                switch (expression.Value.Item1)
                {
                    case SelectorType.XPath:
                        var parentTableNode = entityNode.SelectSingleNode(fieldExpression);
                        if (parentTableNode?.InnerText != null)
                            //columnValue = parentTableNode.InnerText.Trim();
                            columnValue = Regex.Replace(parentTableNode.InnerText.Trim(), "[ ]{2,}", " ");
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

        public async Task<IEnumerable<TEntity>> ProcessarPartes(HtmlDocument document)
        {
            var nameValueDictionary = GetDadosPartesFromHtml(document);
            var entityList = new List<TEntity>();

            foreach (var dictionary in nameValueDictionary)
            {
                var processorEntity = ReflectionHelper.CreateNewEntity<TEntity>();

                foreach (var pair in dictionary)
                {
                    ReflectionHelper.TrySetProperty(processorEntity, pair.Key, pair.Value);
                }

                entityList.Add(processorEntity as TEntity);
            }

            return entityList;
        }

        private static ICollection<Dictionary<string, object>> GetDadosPartesFromHtml(HtmlDocument document)
        {
            Dictionary<string, object> columnNameValueDictionary = null;
            var listNameValueDictionary = new List<Dictionary<string, object>>();

            var entityExpression = ReflectionHelper.GetEntityExpression<TEntity>();
            var propertyExpressions = ReflectionHelper.GetPropertyAttributes<TEntity>();

            var entityNode = document.DocumentNode.SelectSingleNode(entityExpression);

            if (entityNode == null)
            {
                var altExpression = ReflectionHelper.GetEntityAltExpression<TEntity>();
                entityNode = document.DocumentNode.SelectSingleNode(altExpression);
            }

            List<ParteProcesso> partesProcesso = null;
            List<Advogado> advogados = null;
            Tuple<SelectorType, string> tipoParteProps;
            Tuple<SelectorType, string> nomeParteProps;
            Tuple<SelectorType, string> advogadosProps;
            var oxe = propertyExpressions.Keys;
            var existeExpression = propertyExpressions.TryGetValue("TipoParte", out tipoParteProps);

            if (existeExpression)
            {
                switch (tipoParteProps.Item1)
                {
                    case SelectorType.XPath:
                        break;
                    case SelectorType.CssSelector:
                        var nodesPartesProcesso = entityNode.QuerySelectorAll(tipoParteProps.Item2);

                        if (nodesPartesProcesso != null && nodesPartesProcesso.Count > 0)
                        {
                            partesProcesso = new List<ParteProcesso>();
                            int indexParte = 1;

                            foreach (HtmlNode tipoParte in nodesPartesProcesso)
                            {
                                columnNameValueDictionary = new Dictionary<string, object>();
                                ParteProcesso parteProcesso = new ParteProcesso();

                                if(tipoParte?.InnerText != null)
                                {
                                    columnNameValueDictionary.Add("TipoParte", tipoParte.InnerText.Trim().Split(':')[0]);
                                    parteProcesso.TipoParte = tipoParte.InnerText.Trim().Split(':')[0];
                                }

                                var nomeParteNode2 = entityNode.QuerySelector(String.Format("tr:nth-child({0}) > td:nth-child(2)", indexParte));

                                if (nomeParteNode2?.InnerText != null)
                                {
                                    var nomeParteString = nomeParteNode2.InnerText.Trim().Split(new string[] { "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                    parteProcesso.NomeParte = nomeParteString[0].Trim();
                                    columnNameValueDictionary.Add("NomeParte", nomeParteString[0].Trim());
                                }

                                var advogadosNodes = entityNode.QuerySelectorAll(String.Format("tr:nth-child({0}) > td:nth-child(2) > span", indexParte));

                                if (advogadosNodes != null && advogadosNodes.Count > 0)
                                {
                                    advogados = new List<Advogado>();

                                    foreach (var advogadoNode in advogadosNodes)
                                    {
                                        Advogado advogado = new Advogado
                                        {
                                            Nome = entityNode.SelectSingleNode(advogadoNode.XPath).NextSibling.InnerText.ToString().Trim()
                                        };

                                        advogados.Add(advogado);
                                    }

                                    parteProcesso.Advogados = advogados;
                                    columnNameValueDictionary.Add("Advogados", advogados);
                                }

                                indexParte++;
                                partesProcesso.Add(parteProcesso);
                                listNameValueDictionary.Add(columnNameValueDictionary);
                            }

                        }
                        break;
                    default:
                        break;
                }
            }

            return listNameValueDictionary;
        }

        public async Task<IEnumerable<TEntity>> ProcessarMovimentacoes(HtmlDocument document)
        {
            var nameValueDictionary = GetMovimentacoesFromHtml(document);
            var entityList = new List<TEntity>();

            foreach (var dictionary in nameValueDictionary)
            {
                var processorEntity = ReflectionHelper.CreateNewEntity<TEntity>();

                foreach (var pair in dictionary)
                {
                    ReflectionHelper.TrySetProperty(processorEntity, pair.Key, pair.Value);
                }

                entityList.Add(processorEntity as TEntity);
            }

            return entityList;
        }

        private static ICollection<Dictionary<string, object>> GetMovimentacoesFromHtml(HtmlDocument document)
        {
            Dictionary<string, object> columnNameValueDictionary = null;
            var listNameValueDictionary = new List<Dictionary<string, object>>();

            var entityExpression = ReflectionHelper.GetEntityExpression<TEntity>();
            var propertyExpressions = ReflectionHelper.GetPropertyAttributes<TEntity>();

            var entityNode = document.DocumentNode.SelectSingleNode(entityExpression);

            if (entityNode == null)
            {
                var altExpression = ReflectionHelper.GetEntityAltExpression<TEntity>();
                entityNode = document.DocumentNode.SelectSingleNode(altExpression);
            }

            List<Movimentacao> movimentacoes = null;
            List<Advogado> advogados = null;
            Tuple<SelectorType, string> dataMovProps;
            var oxe = propertyExpressions.Keys;
            var existeExpression = propertyExpressions.TryGetValue("Data", out dataMovProps);

            if (existeExpression)
            {
                switch (dataMovProps.Item1)
                {
                    case SelectorType.XPath:
                        break;
                    case SelectorType.CssSelector:
                        //var nodesDataMovimentacao = entityNode.QuerySelectorAll(dataMovProps.Item2);
                        var nodesDataMovimentacao = entityNode.QuerySelectorAll("tr");

                        if (nodesDataMovimentacao != null && nodesDataMovimentacao.Count > 0)
                        {
                            movimentacoes = new List<Movimentacao>();
                            int nodeIndexMovimentacao = 1;

                            foreach (HtmlNode tipoParte in nodesDataMovimentacao)
                            {
                                columnNameValueDictionary = new Dictionary<string, object>();
                                Movimentacao movimentacao = new Movimentacao();

                                var dataMovimentoNode = entityNode.QuerySelector(String.Format("tr:nth-child({0}) > td:nth-child(1)", nodeIndexMovimentacao));
                                var tituloMovimentoNode = entityNode.QuerySelector(String.Format("tr:nth-child({0}) > td:nth-child(3)", nodeIndexMovimentacao));
                                var textoMovimentoNode = entityNode.QuerySelector(String.Format("tr:nth-child({0}) > td:nth-child(3) > span", nodeIndexMovimentacao));
                                var linkMovimentoNode = entityNode.QuerySelector(String.Format("tr:nth-child({0}) > td:nth-child(3) > a", nodeIndexMovimentacao));

                                if (!String.IsNullOrWhiteSpace(dataMovimentoNode?.InnerText.Trim()))
                                {
                                    columnNameValueDictionary.Add("Data", dataMovimentoNode.InnerText.Trim().Split(':')[0]);
                                    movimentacao.Data = dataMovimentoNode.InnerText.Trim().Split(':')[0];
                                }

                                if (!String.IsNullOrWhiteSpace(tituloMovimentoNode?.InnerText.Trim()))
                                {
                                    var nomeParteString = tituloMovimentoNode.InnerText.Trim().Split(new string[] { "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    //var nomeParteString = tituloMovimentoNode.InnerText;
                                    movimentacao.TituloMovimento = nomeParteString.Trim();

                                    columnNameValueDictionary.Add("TituloMovimento", movimentacao.TituloMovimento);
                                }

                                if (!String.IsNullOrWhiteSpace(textoMovimentoNode?.InnerText.Trim()))
                                {
                                    //var nomeParteString = textoMovimentoNode.InnerText.Trim().Split(new string[] { "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                    var nomeParteString = textoMovimentoNode.InnerText;
                                    movimentacao.TextoMovimento = nomeParteString.Trim();

                                    columnNameValueDictionary.Add("TextoMovimento", movimentacao.TextoMovimento);
                                }

                                if (linkMovimentoNode != null)
                                {
                                    var attachmentLink = linkMovimentoNode.Attributes.Where(x => x.Name == "href").FirstOrDefault();

                                    //string inner = linkMovimentoNode.InnerHtml.Trim();
                                    //var tituloMovimento = inner.Substring(inner.IndexOf('>'));
                                    //movimentacao.TituloMovimento = tituloMovimento.Split("</a>", StringSplitOptions.RemoveEmptyEntries)[0];

                                    movimentacao.LinkMovimento = attachmentLink.Value.Trim();
                                    columnNameValueDictionary.Add("LinkMovimento", movimentacao.LinkMovimento);

                                    if (columnNameValueDictionary.Remove("TituloMovimento"))
                                    {
                                        var tituloMovimento = linkMovimentoNode.InnerHtml.Trim();
                                        movimentacao.TituloMovimento = tituloMovimento;

                                        columnNameValueDictionary.Add("TituloMovimento", movimentacao.TituloMovimento);
                                    }
                                }

                                nodeIndexMovimentacao++;
                                movimentacoes.Add(movimentacao);
                                listNameValueDictionary.Add(columnNameValueDictionary);
                            }

                        }
                        break;
                    default:
                        break;
                }
            }

            return listNameValueDictionary;
        }

        private static Dictionary<string, Dictionary<string, List<object>>> GetDadosMovimentacoesFromHtml(HtmlDocument document)
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

                                if (valueNodeCss != null)
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

        public Task<IEnumerable<TEntity>> Process(HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> ProcessForm(HtmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
