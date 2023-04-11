using System.Net;

namespace TimestampMicroserviceTests.Integration
{
    // TODO: Use TestData Generator for Unit and IntegrationTests
    public sealed class TimestampEndpointTest : IntegrationTest
    {
        [Test]
        public async Task GetOnApiRoot_ReturnsAValidTimestamp()
        {
            var response = await _testClient.GetAsync("/api");

            AssertThatResponseIsAValidTimestamp(response);
        }

        [Test]
        public async Task Get_ReturnsAValidTimestamp_WhenSuppliedAValidUnixTimestamp()
        {
            ulong unixTimestamp = TimestampGenerator.GetUnixTimestampFromDate(new DateTime(2015, 12, 25));

            var response = await _testClient.GetAsync($"/api/{unixTimestamp}");

            AssertThatResponseIsAValidTimestamp(response);
        }

        [Test]
        public async Task Get_ReturnsAValidTimestamp_WhenSuppliedAValidDateString()
        {
            const string dateString = "2015-12-25";

            var response = await _testClient.GetAsync($"/api/{dateString}");

            AssertThatResponseIsAValidTimestamp(response);
        }

        private void AssertThatResponseIsAValidTimestamp(HttpResponseMessage response)
        {
            Assert.Multiple(async () =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(await response.Content.ReadFromJsonAsync<Timestamp>(), Is.Not.Null);
            });
        }

        [Test]
        public async Task Get_ReturnsBadRequest_WhenSuppliedAnInalidUnixTimestamp()
        {
            const string invalidUnixTimestamp = "-123456789";

            var response = await _testClient.GetAsync($"/api/{invalidUnixTimestamp}");

            AssertResponseIsBadRequest(response);
        }

        [Test]
        public async Task Get_ReturnsBadRequest_WhenSuppliedAValidDateString()
        {
            const string dateString = "2015-12-35";

            var response = await _testClient.GetAsync($"/api/{dateString}");

            AssertResponseIsBadRequest(response);
        }

        private void AssertResponseIsBadRequest(HttpResponseMessage response)
        {
            Assert.Multiple(async () =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("\"Invalid Date\""));
            });
        }
    }
}