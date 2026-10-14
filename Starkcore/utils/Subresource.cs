using System;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace StarkCore.Utils
{
    public abstract class SubResource
    {
        public override string ToString()
        {
            Dictionary<string, object> dict = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(this));

            string spacer = "  ";
            string newLine = Environment.NewLine;
            List<string> fields = new List<string>();
            foreach (KeyValuePair<string, object> entry in dict)
            {
                string stringValue;
                if (entry.Value is null)
                {
                    stringValue = "null";
                }
                else if (entry.Value is List<string> stringList)
                {
                    stringValue = "{ " + string.Join(", ", stringList) + " }";
                }
                else if (entry.Value is List<Dictionary<string, object>> objectDictList)
                {
                    List<string> stringDictList = new List<string>();
                    foreach (Dictionary<string, object> subDict in objectDictList)
                    {
                        List<string> stringDictSubList = new List<string>();
                        foreach (KeyValuePair<string, object> subEntry in subDict)
                        {
                            stringDictSubList.Add("{ " + subEntry.Key + ", " + subEntry.Value.ToString() + " }");
                        }
                        stringDictList.Add("{ " + string.Join(", ", stringDictSubList) + " }");
                    }
                    stringValue = "{ " + string.Join(", ", stringDictList) + " }";
                }
                else if (entry.Value is List<Dictionary<string, string>> dictList)
                {
                    List<string> stringDictList = new List<string>();
                    foreach (Dictionary<string, string> subDict in dictList)
                    {
                        List<string> stringDictSubList = new List<string>();
                        foreach (KeyValuePair<string, string> subEntry in subDict)
                        {
                            stringDictSubList.Add("{ " + subEntry.Key + ", " + subEntry.Value.ToString() + " }");
                        }
                        stringDictList.Add("{ " + string.Join(", ", stringDictSubList) + " }");
                    }
                    stringValue = "{ " + string.Join(", ", stringDictList) + " }";
                }
                else if (entry.Value is Dictionary<string, object> || entry.Value is Dictionary<string, string>)
                {
                    stringValue = DictionaryToString(entry.Value);
                }
                else
                {
                    stringValue = entry.Value.ToString();
                }
                string[] splitValue = stringValue.Split(
                    new[] { newLine },
                    StringSplitOptions.None
                );
                fields.Add(entry.Key + ": " + string.Join(newLine + spacer, splitValue));
            }
            string core = string.Join("," + newLine + spacer, fields);

            return GetType().Name + "(" + newLine + spacer + core + newLine + ")";
        }

        // Values parsed from API responses are JTokens, not CLR collections.
        private static string ValueToString(object value)
        {
            if (value is null)
            {
                return "null";
            }
            if (value is JValue jValue)
            {
                return ValueToString(jValue.Value);
            }
            if (value is JArray jArray)
            {
                return "{ " + string.Join(", ", jArray.Select(item => ValueToString(item))) + " }";
            }
            if (value is JObject jObject)
            {
                return "{ " + string.Join(", ", jObject.Properties().Select(prop => PairToString(prop.Name, prop.Value))) + " }";
            }
            if (value is List<string> stringList)
            {
                return "{ " + string.Join(", ", stringList) + " }";
            }
            if (value is Dictionary<string, object> || value is Dictionary<string, string>)
            {
                return DictionaryToString(value);
            }
            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, CultureInfo.InvariantCulture);
            }
            return value.ToString();
        }

        private static string DictionaryToString(object dictionary)
        {
            List<string> pairs = new List<string>();
            foreach (DictionaryEntry entry in (IDictionary)dictionary)
            {
                pairs.Add(PairToString(entry.Key.ToString(), entry.Value));
            }
            return "{ " + string.Join(", ", pairs) + " }";
        }

        private static string PairToString(string key, object value)
        {
            return "{ " + key + ", " + ValueToString(value) + " }";
        }

        public Dictionary<string, object> ToJson()
        {
            return GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(this));
        }
    }
}
