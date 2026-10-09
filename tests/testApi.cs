using Xunit;
using StarkCore.Utils;

namespace StarkCoreTests
{
    public class TestApi
    {
        [Theory]
        [InlineData("AiSpeech", "speeches")]
        [InlineData("Dish", "dishes")]
        [InlineData("Box", "boxes")]
        [InlineData("Log", "logs")]
        [InlineData("Entity", "entities")]
        [InlineData("Key", "keys")]
        [InlineData("Address", "address")]
        [InlineData("TransferLogs", "logs")]
        public void LastNamePlural(string resourceName, string expected)
        {
            Assert.Equal(expected, Api.LastNamePlural(resourceName));
        }
    }
}
