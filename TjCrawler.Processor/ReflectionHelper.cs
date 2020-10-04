using TjCrawler.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DotnetCrawler.Processor
{
    public class ReflectionHelper
    {
        internal static string GetEntityExpression<TEntity>()
        {
            var entityAttribute = (typeof(TEntity)).GetCustomAttribute<DotnetCrawlerEntityAttribute>();
            if (entityAttribute == null || string.IsNullOrWhiteSpace(entityAttribute.XPath))
                throw new Exception("This entity should be xpath");

            return entityAttribute.XPath;
        }

        internal static string GetEntityAltExpression<TEntity>()
        {
            var entityAttribute = (typeof(TEntity)).GetCustomAttribute<DotnetCrawlerEntityAttribute>();
            if (entityAttribute == null || string.IsNullOrWhiteSpace(entityAttribute.XPathAlt))
                throw new Exception("This entity should be xpath");

            return entityAttribute.XPathAlt;
        }

        public static Dictionary<string, Tuple<SelectorType, string>> GetPropertyAttributes<TEntity>()
        {
            var attributeDictionary = new Dictionary<string, Tuple<SelectorType, string>>();

            PropertyInfo[] props = typeof(TEntity).GetProperties();
            var propList = props.Where(p => p.CustomAttributes.Count() > 0);

            foreach (PropertyInfo prop in propList)
            {
                var attr = prop.GetCustomAttribute<DotnetCrawlerFieldAttribute>();
                if (attr != null)
                {
                    attributeDictionary.Add(prop.Name, Tuple.Create(attr.SelectorType, attr.Expression));
                }
            }
            return attributeDictionary;
        }

        internal static object CreateNewEntity<TEntity>()
        {
            object instance = Activator.CreateInstance(typeof(TEntity));
            return instance;
        }

        internal static void TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
            {
                if (prop.PropertyType == typeof(string))
                {
                    string coreValue = value?.ToString();
                    var displayNameProperty = prop.GetCustomAttribute<DisplayNameAttribute>(false);

                    if (displayNameProperty != null && value != null
                        && value.ToString().ToLowerInvariant().Contains(displayNameProperty.DisplayName.ToLowerInvariant())
                        && value.ToString().Contains(':'))
                    {
                        coreValue = value.ToString().Split(':')[1];
                    }

                    if (!String.IsNullOrWhiteSpace(coreValue))
                    {
                        prop.SetValue(obj, coreValue.Trim(), null);
                    }
                } else if (prop.PropertyType == typeof(bool) && value != null){
                    var evaluationExpression = prop.GetCustomAttribute<DisplayNameAttribute>(false);

                    if (evaluationExpression != null
                        && value.ToString().ToLowerInvariant().Equals(evaluationExpression.DisplayName.ToLowerInvariant()))
                    {
                        prop.SetValue(obj, true, null);
                    }
                } else {
                    prop.SetValue(obj, value, null);
                }
            }
        }
    }
}
