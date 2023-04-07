using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TimestampMicroserviceTests
{
    public class TimestampServiceTest
    {
        [OneTimeSetUp]
        public void TimestampSetup()
        {
            timeProviderMock = new Mock<ITimeProvider>();

            service = new TimestampService(timeProviderMock.Object);
        }

        [Test]
        public void GenerateCurrentTimestamp_ReturnsAUtcTimestampWithAValidFormat()
        {
            timeProviderMock.SetupGet(mock => mock.UtcNow).Returns(new DateTime(2015, 12, 25));

            var result = service.GenerateCurrentTimestamp();

            AssertThatResultHasAValidUtcDateString(result);
        }

        [Test]
        public void GenerateCurrentTimestamp_ReturnsATimestampForTheCurrentTime()
        {
            DateTime dateTime = new(2015, 12, 25);
            timeProviderMock.SetupGet(mock => mock.UtcNow).Returns(dateTime);

            var result = service.GenerateCurrentTimestamp();

            AssertThatResultEqualsDateTime(result, dateTime);
        }

        [Test]
        public void GenerateTimestampFromString_ReturnsAUtcTimestampWithAValidFormat_WhenSuppliedAUnixTimestamp()
        {
            ulong unixTimestamp = GetUnixTimestampFromDate(new DateTime(2015, 12, 25));
            var result = service.GenerateTimestampFromString(unixTimestamp.ToString());

            AssertThatResultHasAValidUtcDateString(result);
        }

        [Test]
        public void GenerateTimestampFromString_ReturnsAUtcTimestampWithAValidFormat_WhenSuppliedADateString()
        {
            const string dateString = "2015-12-25";
            var result = service.GenerateTimestampFromString(dateString);

            AssertThatResultHasAValidUtcDateString(result);
        }

        [Test]
        public void GenerateTimestampFromString_ReturnsAnError_WhenSuppliedAnInvalidUnixTimestamp()
        {
            const string dateString = "-123456789";
            var result = service.GenerateTimestampFromString(dateString);

            AssertThatResultIsAnError(result);
        }

        [Test]
        public void GenerateTimestampFromString_ReturnsAnError_WhenSuppliedAnInvalidDateString()
        {
            const string dateString = "2015-15-34";
            var result = service.GenerateTimestampFromString(dateString);

            AssertThatResultIsAnError(result);
        }

        [Test]
        public void GenerateTimestampFromString_ReturnsTheSameTime_WhenSuppliedAUnixTimestamp()
        {
            DateTime dateTime = new(2015, 12, 25);
            ulong unixTimestamp = GetUnixTimestampFromDate(dateTime);

            var result = service.GenerateTimestampFromString(unixTimestamp.ToString());

            AssertThatResultEqualsDateTime(result, dateTime);
        }

        [Test]
        public void GenerateTimestampFromString_ReturnsTheSameTime_WhenSuppliedADateString()
        {
            const string dateString = "2015-12-25";
            DateTime dateTime = new(2015, 12, 25);

            var result = service.GenerateTimestampFromString(dateString);

            AssertThatResultEqualsDateTime(result, dateTime);
        }

        private void AssertThatResultEqualsDateTime(IResult result, DateTime dateTime)
        {
            var okResult = result as Ok<SuccessfulTimestampResponse>;
            Assert.Multiple(() => {
                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult!.Value!.unix, Is.EqualTo(GetUnixTimestampFromDate(dateTime)));

                TryParseDateStringUsingExpectedFormat(okResult!.Value!.utc, out DateTime utcDateTime);
                Assert.That(utcDateTime, Is.EqualTo(dateTime));
            });
        }

        private void AssertThatResultHasAValidUtcDateString(IResult result)
        {
            var okResult = result as Ok<SuccessfulTimestampResponse>;
            Assert.Multiple(() => {
                Assert.That(okResult, Is.Not.Null);
                Assert.That(TryParseDateStringUsingExpectedFormat(okResult!.Value!.utc, out DateTime _), Is.True);
            });
        }

        private bool TryParseDateStringUsingExpectedFormat(string dateString, out DateTime dateTime)
        {
            const string expectedUtcTimeFormat = "ddd, dd MMM yyyy HH:mm:ss G\\MT";
            return DateTime.TryParseExact(dateString, expectedUtcTimeFormat, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dateTime);
        }

        private void AssertThatResultIsAnError(IResult result)
        {
            var okResult = result as Ok<InvalidTimestampResponse>;
            Assert.Multiple(() => {
                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult!.Value!.error, Is.EqualTo("Invalid Date"));
            });
        }

        private ulong GetUnixTimestampFromDate(DateTime dateTime)
        {
            return (ulong)dateTime.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
        }

        private Mock<ITimeProvider> timeProviderMock;
        private TimestampService service;
    }
}