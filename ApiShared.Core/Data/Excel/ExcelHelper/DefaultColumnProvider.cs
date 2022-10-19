using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExporter
{
    public class DefaultColumnProvider : IColumnProvider
    {
        public bool HasAttributeOnly { get; set; }
        public Type? DynamicType { get; set; }
        public NameType NamingType { get; set; }

        public DefaultColumnProvider(bool hasAttributeOnly = false, NameType namingType = NameType.Property, Type? defaultType = null)
        {
            HasAttributeOnly = hasAttributeOnly;
            DynamicType = defaultType;
            NamingType = namingType;
        }

        public List<ExcelColumnAttribute> GetColumns<T>()
        {
            if (NamingType == NameType.Field)
                return FieldData<T>();
            else
                return PropertyData<T>();
        }

        private List<ExcelColumnAttribute> PropertyData<T>()
        {
            var result = new List<ExcelColumnAttribute>();

            PropertyInfo[] props;
            if (DynamicType != null)
                props = DynamicType.GetProperties();
            else
                props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                if (HasAttributeOnly)
                {
                    var attr = prop.GetCustomAttributes(true).FirstOrDefault(c => c is ExcelColumnAttribute);
                    if (attr != null)
                    {
                        var excelAttr = (ExcelColumnAttribute)attr;
                        if (excelAttr.SourceName == null)
                            excelAttr.SourceName = prop.Name;

                        if (excelAttr.Name == null)
                            excelAttr.Name = excelAttr.SourceName;

                        result.Add(excelAttr);
                    }
                }
                else
                {
                    result.Add(new ExcelColumnAttribute() { Name = prop.Name, SourceName = prop.Name });
                }
            }
            return result;
        }
        private List<ExcelColumnAttribute> FieldData<T>()
        {
            var result = new List<ExcelColumnAttribute>();

            FieldInfo[] props;
            if (DynamicType != null)
                props = DynamicType.GetFields();
            else
                props = typeof(T).GetFields();

            foreach (var prop in props)
            {
                if (HasAttributeOnly)
                {
                    var attr = prop.GetCustomAttributes(true).FirstOrDefault(c => c is ExcelColumnAttribute);
                    if (attr != null)
                    {
                        var excelAttr = (ExcelColumnAttribute)attr;
                        if (excelAttr.SourceName == null)
                            excelAttr.SourceName = prop.Name;

                        result.Add(excelAttr);
                    }
                }
                else
                {
                    result.Add(new ExcelColumnAttribute() { Name = prop.Name, SourceName = prop.Name });
                }
            }
            return result;
        }
    }
}
