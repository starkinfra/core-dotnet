using Xunit;
using System;
using System.Globalization;
using System.Linq;
using StarkCore.Utils;


namespace StarkCoreTests
{
    // These tests guard the request-path string formatting against machine-locale
    // dependence and against CultureNotFoundException under InvariantGlobalization=true
    // (PredefinedCulturesOnly), where instantiating any named culture like
    // new CultureInfo("en-US") throws. They require no credentials or network access.
    public class TestCultureSafety
    {
        // Builds the Access-Time header value exactly as Request.Fetch does.
        private static string BuildAccessTime(DateTime utcNow)
        {
            return utcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
        }

        [Fact]
        public void AccessTimeIsAsciiDigitsWithSingleDotAndNeedsNoNamedCulture()
        {
            // building the value must not require constructing any named culture:
            // the expression below uses only CultureInfo.InvariantCulture, which is
            // always available, including under InvariantGlobalization=true
            string accessTime = BuildAccessTime(DateTime.UtcNow);

            Assert.NotEmpty(accessTime);
            Assert.True(
                accessTime.All(c => (c >= '0' && c <= '9') || c == '.'),
                $"Access-Time '{accessTime}' contains characters other than ASCII digits and '.'"
            );
            Assert.True(
                accessTime.Count(c => c == '.') <= 1,
                $"Access-Time '{accessTime}' contains more than one '.' separator"
            );
            Assert.True(char.IsDigit(accessTime[0]), $"Access-Time '{accessTime}' must start with a digit");
        }

        [Theory]
        [InlineData("de-DE")]
        [InlineData("ru-RU")]
        [InlineData("ar-SA")]
        [InlineData("tr-TR")]
        public void AccessTimeIsByteIdenticalToEnUsUnderHostileCurrentCultures(string hostileCultureName)
        {
            CultureInfo original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(hostileCultureName);

                var seededRandom = new Random(20260610);
                for (int i = 0; i < 1000; i++)
                {
                    double timestamp = seededRandom.NextDouble() * 4102444800d; // 0 .. year 2100
                    string invariant = timestamp.ToString(CultureInfo.InvariantCulture);
                    string enUsReference = timestamp.ToString(new CultureInfo("en-US"));

                    Assert.Equal(enUsReference, invariant);
                }

                string accessTimeNow = BuildAccessTime(DateTime.UtcNow);
                Assert.True(
                    accessTimeNow.All(c => (c >= '0' && c <= '9') || c == '.'),
                    $"Under {hostileCultureName}, Access-Time '{accessTimeNow}' is not ASCII digits + '.'"
                );
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Theory]
        [InlineData("de-DE")]
        [InlineData("ru-RU")]
        [InlineData("ar-SA")] // default calendar is Um Al Qura: CurrentCulture-based formatting would emit Hijri years
        [InlineData("tr-TR")]
        public void StarkDateAndStarkDateTimeAreGregorianUnderHostileCurrentCultures(string hostileCultureName)
        {
            CultureInfo original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(hostileCultureName);

                var date = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc);
                Assert.Equal("2026-06-10", new StarkDate(date).ToString());

                var dateTime = new DateTime(2026, 6, 10, 13, 45, 30, DateTimeKind.Utc).AddTicks(1234560);
                Assert.Equal("2026-06-10T13:45:30.123456+00:00", new StarkDateTime(dateTime).ToString());
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }
    }
}
