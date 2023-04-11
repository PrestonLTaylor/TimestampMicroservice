using Microsoft.AspNetCore.Mvc.Testing;

namespace TimestampMicroserviceTests.Integration
{
    public class IntegrationTest
    {
        protected IntegrationTest()
        {
            var factory = new WebApplicationFactory<Program>();
            _testClient = factory.CreateClient();
        }

        protected readonly HttpClient _testClient;
    }
}
