﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace StarkCore.Utils
{
    public static class Api
    {
        public delegate SubResource ResourceMaker(dynamic json);

        public static Dictionary<string, object> ApiJson(SubResource entity)
        {
            return CastJsonToApiFormat(entity.ToJson());
        }

        public static Dictionary<string, object> ApiJson(Dictionary<string, object> entity)
        {
            return CastJsonToApiFormat(entity);
        }

        public static Dictionary<string, object> CastJsonToApiFormat(Dictionary<string, object> json)
        {
            Dictionary<string, object> apiJson = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> entry in json)
            {
                if (entry.Value == null)
                {
                    continue;
                }

                string key = Case.PascalToCamel(entry.Key);
                if (key.EndsWith("ID"))
                {
                    key = key.Substring(0, key.Length - 2) + "Id";
                }

                dynamic value = entry.Value;
                if (value is DateTime)
                {
                    DateTime data = value;
                    value = DateToString(data);
                }
                if (value is StarkDateTime)
                {
                    StarkDateTime data = value;
                    value = StarkDateTimeToString(data);
                }
                if (value is StarkDate)
                {
                    StarkDate data = value;
                    value = StarkDateToString(data);
                }
                if (value is IList)
                {
                    bool nested = false;
                    List<object> casted = new List<object>();
                    foreach (dynamic nestedEntry in value)
                    {
                        if (nestedEntry is Dictionary<string, object>)
                        {
                            Dictionary<string, object> castedNestedEntry = nestedEntry as Dictionary<string, object>;
                            casted.Add(CastJsonToApiFormat(castedNestedEntry));
                            nested = true;
                        }
                        if (nestedEntry is SubResource)
                        {
                            casted.Add(ApiJson(nestedEntry));
                            nested = true;
                        }
                    }
                    if (nested)
                    {
                        value = casted;
                    }
                }
                if (value is SubResource)
                {
                    value = ApiJson(value);
                }

                if (value == null)
                {
                    continue;
                }

                apiJson.Add(
                    key,
                    value
                );
            }
            return apiJson;
        }

        public static object DateToString(DateTime dateTime)
        {
            if (dateTime == dateTime.Date)
            {
                return new StarkDate(dateTime).ToString();
            }
            return new StarkDateTime(dateTime).ToString();
        }

        public static object StarkDateToString(StarkDate starkDate)
        {
            return starkDate.ToString();
        }

        public static object StarkDateTimeToString(StarkDateTime starkDateTime)
        {
            return starkDateTime.ToString();
        }

        public static SubResource FromApiJson(Api.ResourceMaker resourceMaker, dynamic json)
        {
            return resourceMaker(json);
        }

        public static string Endpoint(string resourceName)
        {
            string kebab = Case.CamelOrPascalToKebab(resourceName);
            return kebab.Replace("-log", "/log")
                        .Replace("-attempt", "/attempt");
        }

        public static string LastNamePlural(string resourceName)
        {
            string lastName = LastName(resourceName);
            if (lastName.EndsWith("s"))
            {
                return lastName;
            }
            if (lastName.EndsWith("y") && !lastName.EndsWith("ey"))
            {
                return $"{lastName.Remove(lastName.Length - 1)}ies";
            }
            return $"{lastName}s";
        }

        public static string LastName(string resourceName)
        {
            string[] names = Case.CamelOrPascalToKebab(resourceName).Split(new string[] { "-" }, StringSplitOptions.None);
            return names.Last();
        }
    }
}
