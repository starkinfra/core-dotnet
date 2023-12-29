using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace StarkCore.Utils
{
    public static class Json
    {
        public static string Encode(object payload)
        {
            return JsonConvert.SerializeObject(payload);
        }

        public static JObject Decode(string content)
        {
            using (var reader = new JsonTextReader(new StringReader(content)) { DateParseHandling = DateParseHandling.None })
                return JObject.Load(reader);
        }
    }
}
