using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Linq;

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;


namespace Utilities
{
    public static class utils
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string tableName)
        {
            DataTable tbl = ToDataTable(collection);
            tbl.TableName = tableName;
            return tbl;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            DataTable dt = new DataTable();
            Type t = typeof(T);
            PropertyInfo[] pia = t.GetProperties();
            object temp;
            DataRow dr;

            for (int i = 0; i < pia.Length; i++)
            {
                dt.Columns.Add(pia[i].Name, Nullable.GetUnderlyingType(pia[i].PropertyType) ?? pia[i].PropertyType);
                dt.Columns[i].AllowDBNull = true;
            }

            //Populate the table
            foreach (T item in collection)
            {
                dr = dt.NewRow();
                dr.BeginEdit();

                for (int i = 0; i < pia.Length; i++)
                {
                    temp = pia[i].GetValue(item, null);
                    if (temp == null || (temp.GetType().Name == "Char" && ((char)temp).Equals('\0')))
                    {
                        dr[pia[i].Name] = (object)DBNull.Value;
                    }
                    else
                    {
                        dr[pia[i].Name] = temp;
                    }
                }

                dr.EndEdit();
                dt.Rows.Add(dr);
            }
            return dt;
        }

		public class NombreLabelsModel
		{
        public string NombreCampo { get; set; }
        public string DiplayNombre { get; set; }
		}

        public static List<NombreLabelsModel> GetNamesFromDataAnotationModel<T>()
        {
            List<NombreLabelsModel> lista = new List<NombreLabelsModel>();
            Type t = typeof(T);
            PropertyInfo[] pia = t.GetProperties();
            string propertyName = pia[1].GetType().Name;
            for (int i = 0; i < pia.Length; i++)
            {
                CustomAttributeData displayAttribute = pia[i].CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "DisplayAttribute");
                if (displayAttribute != null)
                {
                    NombreLabelsModel obj = new NombreLabelsModel
                    {
                        NombreCampo = pia[i].Name.ToString(),
                        DiplayNombre = displayAttribute.NamedArguments.FirstOrDefault().TypedValue.Value.ToString()

                    };
                    lista.Add(obj);
                }
                
            }
            return lista;
        }

        public static string GetSingleNameFromDataAnotationModel<T>(string NombrePropiedad)
        {
            Type t = typeof(T);
            PropertyInfo[] pia = t.GetProperties();
            PropertyInfo p = pia.Where(o => o.Name == NombrePropiedad).FirstOrDefault();
            
                CustomAttributeData displayAttribute = p.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "DisplayAttribute");
                if (displayAttribute != null)
                {
                    return displayAttribute.NamedArguments.FirstOrDefault().TypedValue.Value.ToString();

                }
                else
                {
                    return "";
                }

        }

        public static string SerializeObject(object obj)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlDoc.Load(ms);
                return xmlDoc.InnerXml;
            }
        }

        

    }
}
