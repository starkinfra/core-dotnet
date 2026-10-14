using Xunit;
using System;
using StarkCore.Utils;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace StarkCoreTests
{
    public class Printable : SubResource
    {
        public object Value { get; }

        public Printable(object value)
        {
            Value = value;
        }
    }

    public class TestSubResourceToString
    {
        private static string Expected(string value)
        {
            string newLine = Environment.NewLine;
            return "Printable(" + newLine + "  Value: " + value + newLine + ")";
        }

        [Fact]
        public void PrintsNull()
        {
            Assert.Equal(Expected("null"), new Printable(null).ToString());
        }

        [Fact]
        public void PrintsStringAndInt()
        {
            Assert.Equal(Expected("Ana"), new Printable("Ana").ToString());
            Assert.Equal(Expected("42"), new Printable(42).ToString());
        }

        [Fact]
        public void PrintsStringList()
        {
            Printable resource = new Printable(new List<string> { "pix", "card" });

            Assert.Equal(Expected("{ pix, card }"), resource.ToString());
        }

        [Fact]
        public void PrintsListOfObjectDictionaries()
        {
            Printable resource = new Printable(new List<Dictionary<string, object>> {
                new Dictionary<string, object> { { "a", 1 }, { "b", "x" } },
                new Dictionary<string, object> { { "c", 2 } }
            });

            Assert.Equal(Expected("{ { { a, 1 }, { b, x } }, { { c, 2 } } }"), resource.ToString());
        }

        [Fact]
        public void PrintsListOfStringDictionaries()
        {
            Printable resource = new Printable(new List<Dictionary<string, string>> {
                new Dictionary<string, string> { { "a", "1" }, { "b", "x" } }
            });

            Assert.Equal(Expected("{ { { a, 1 }, { b, x } } }"), resource.ToString());
        }

        [Fact]
        public void PrintsFlatObjectDictionary()
        {
            Printable resource = new Printable(new Dictionary<string, object> {
                { "balance", 1520.33 },
                { "name", "Ana" }
            });

            Assert.Equal(Expected("{ { balance, 1520.33 }, { name, Ana } }"), resource.ToString());
        }

        [Fact]
        public void PrintsFlatStringDictionary()
        {
            Printable resource = new Printable(new Dictionary<string, string> {
                { "a", "1" },
                { "b", "x" }
            });

            Assert.Equal(Expected("{ { a, 1 }, { b, x } }"), resource.ToString());
        }

        [Fact]
        public void PrintsNestedListAndDictionary()
        {
            Printable resource = new Printable(new Dictionary<string, object> {
                { "products", new List<string> { "pix", "card" } },
                { "owner", new Dictionary<string, object> { { "name", "Ana" } } }
            });

            Assert.Equal(
                Expected("{ { products, { pix, card } }, { owner, { { name, Ana } } } }"),
                resource.ToString()
            );
        }

        [Fact]
        public void PrintsDictionaryParsedFromJson()
        {
            JObject parsed = JObject.Parse("{\"balance\": 1520.33, \"products\": [\"pix\", \"card\"], \"owner\": {\"name\": \"Ana\"}, \"note\": null}");
            Printable resource = new Printable(parsed.ToObject<Dictionary<string, object>>());

            Assert.Equal(
                Expected("{ { balance, 1520.33 }, { products, { pix, card } }, { owner, { { name, Ana } } }, { note, null } }"),
                resource.ToString()
            );
        }

        [Fact]
        public void PrintsEmptyDictionary()
        {
            Assert.Equal(Expected("{  }"), new Printable(new Dictionary<string, object>()).ToString());
        }

        [Fact]
        public void PrintsNullDictionaryValue()
        {
            Printable resource = new Printable(new Dictionary<string, object> { { "a", null } });

            Assert.Equal(Expected("{ { a, null } }"), resource.ToString());
        }
    }
}
