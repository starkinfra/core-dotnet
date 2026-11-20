using System;
using System.Collections.Generic;
using System.Linq;
using StarkCore;
using StarkCore.Error;
using StarkCore.Utils;
using Xunit;
using static StarkCore.Utils.Api;

namespace StarkCoreTests
{
    // There is no seam to replace the outgoing HTTP call of the core (the HttpClient is a private static readonly field), so,
    // like TestRestGet, these tests talk to the sandbox and every case is a rejection by the API. Only the invalidHolder case
    // tells PATCH from POST (the sandbox answers the same item with invalidCard to a POST); the other one proves that the items
    // travel under the plural resource name. Reading a successful response into entities is covered by the SDK tests that
    // assign holders in the sandbox.
    public class TestPatchMulti
    {
        public class PatchMultiCard : Resource
        {
            public PatchMultiCard(string id = null) : base(id)
            {
            }

            internal static Resource ResourceMaker(dynamic json)
            {
                string id = json.id;
                return new PatchMultiCard(id: id);
            }
        }

        private static List<string> RejectedCodes(List<Dictionary<string, object>> items)
        {
            try
            {
                Rest.PatchMulti(
                    resourceName: "IssuingCard",
                    resourceMaker: PatchMultiCard.ResourceMaker,
                    entities: items,
                    user: TestUser.GetProject(),
                    host: "infra",
                    apiVersion: "v2",
                    sdkVersion: "0.0.0"
                ).ToList();
            }
            catch (InputErrors errors)
            {
                return errors.Errors.Select(error => error.Code).ToList();
            }
            throw new Exception("the API should have rejected the list");
        }

        [Fact]
        public void ItemWithAPartialHolderIsRejectedAsInvalidHolder()
        {
            List<string> codes = RejectedCodes(new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "id", "0000000000000001" }, { "holderName", "Tony Stark" } }
            });

            Assert.Contains("invalidHolder", codes);
        }

        [Fact]
        public void ItemWithAnUnknownCardIsRejectedAsInvalidCard()
        {
            List<string> codes = RejectedCodes(new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "id", "0000000000000001" }, { "tags", new List<string> { "sdk-test" } } }
            });

            Assert.Contains("invalidCard", codes);
        }
    }
}
