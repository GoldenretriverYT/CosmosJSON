using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosJSON
{
    public class Serializer
    {
        public static string Serialize(Dictionary<string, object> obj)
        {
            string s = "{";

            foreach (var key in obj.Keys) {
                s += "\"" + key + "\": " + SerializeValue(obj[key]) + ", ";
            }

            if (s.Length > 1) s = s.Substring(0, s.Length - 2);

            return s + "}";
        }

        public static string SerializeValue(object value)
        {
            if (value is string) return "\"" + value + "\"";
            if (value is bool b) return b.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            if (value is int i) return i.ToString(CultureInfo.InvariantCulture);
            if (value is float f) return f.ToString(CultureInfo.InvariantCulture);
            if (value is double d) return d.ToString(CultureInfo.InvariantCulture);
            if (value is Dictionary<string, object>) return Serialize((Dictionary<string, object>)value);
            if (value is object[]) return SerializeArray((object[])value);

            throw new Exception("Unsupported type: " + value.GetType().Name);
        }

        public static string SerializeArray(object[] arr)
        {
            string s = "[";

            foreach (var value in arr) {
                s += SerializeValue(value) + ", ";
            }

            if (s.Length > 1) s = s.Substring(0, s.Length - 2);

            return s + "]";
        }
    }
}
