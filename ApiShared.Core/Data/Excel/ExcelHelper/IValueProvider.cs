using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExporter
{
    public interface IValueProvider
    {
        public object? GetValue(string name, object classObject);
        public Dictionary<string, object?> GetValues(string[] names, object classObject);
    }

    public class DefaultValueProvider : IValueProvider
    {
        NameType NamingType { get; set; }

        public DefaultValueProvider(NameType namingType = NameType.Property)
        {
            NamingType = namingType;
        }

        public object? GetValue(string name, object classObject)
        {
            if(NamingType == NameType.Property)
                return classObject.GetType().GetProperty(name)?.GetValue(classObject, null);
            else
                return classObject.GetType().GetField(name)?.GetValue(classObject);
        }

        public Dictionary<string, object?> GetValues(string[] names, object classObject)
        {
            var result = new Dictionary<string, object?>();
            foreach (var name in names)
            {
                result.Add(name, GetValue(name, classObject));
            }
            return result;
        }
    }
}
