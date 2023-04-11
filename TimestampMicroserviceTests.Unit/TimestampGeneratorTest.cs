using Moq;

namespace TimestampMicroserviceTests
{
    public class TimestampGeneratorTest
    {
        [OneTimeSetUp]
        public void TimestampSetup()
        {
            timeProviderMock = new Mock<ITimeProvider>();

            generator = new TimestampGenerator(timeProviderMock.Object);
        }

        [Test]
        public void GenerateCurrentTimestamp_ReturnsAUtcTimestampWithAValidFormat()
        {
            timeProviderMock.SetupGet(mock => mock.UtcNow).Returns(new DateTime(2015, 12, 25));

            var timestamp = generator.GenerateCurrentTimestamp();

            AssertThatTimestampHasAValidUtcDateString(timestamp);
        }

        [Test]
        public void GenerateCurrentTimestamp_ReturnsATimestampForTheCurrentTime()
        {
            DateTime dateTime = new(2015, 12, 25);
            timeProviderMock.SetupGet(mock => mock.UtcNow).Returns(dateTime);

            var timestamp = generator.GenerateCurrentTimestamp();

            AssertThatTimestampEqualsDateTime(timestamp, dateTime);
        }

        [Test]
        public void TryGenerateTimestampFromString_ReturnsAUtcTimestampWithAValidFormat_WhenSuppliedAUnixTimestamp()
        {
            ulong unixTimestamp = GetUnixTimestampFromDate(new DateTime(2015, 12, 25));

            generator.TryGenerateTimestampFromString(unixTimestamp.ToString(), out Timestamp? timestamp);

            AssertThatTimestampHasAValidUtcDateString(timestamp);
        }

        [Test]
        public void TryGenerateTimestampFromString_ReturnsAUtcTimestampWithAValidFormat_WhenSuppliedADateString()
        {
            const string dateString = "2015-12-25";
            generator.TryGenerateTimestampFromString(dateString, out Timestamp? timestamp);

            AssertThatTimestampHasAValidUtcDateString(timestamp);
        }

        [Test]
        public void TryGenerateTimestampFromString_Fails_WhenSuppliedAnInvalidUnixTimestamp()
        {
            const string dateString = "-123456789";
            var couldGenerate = generator.TryGenerateTimestampFromString(dateString, out Timestamp? _);

            Assert.That(couldGenerate, Is.False);
        }

        [Test]
        public void TryGenerateTimestampFromString_Fails_WhenSuppliedAnInvalidDateString()
        {
            const string dateString = "2015-15-34";
            var couldGenerate = generator.TryGenerateTimestampFromString(dateString, out Timestamp? _);

            Assert.That(couldGenerate, Is.False);
        }

        [Test]
        public void TryGenerateTimestampFromString_ReturnsTheSameTimeAsSuppliedUnixTimestamp()
        {
            DateTime dateTime = new(2015, 12, 25);
            ulong unixTimestamp = GetUnixTimestampFromDate(dateTime);

            generator.TryGenerateTimestampFromString(unixTimestamp.ToString(), out Timestamp? timestamp);

            AssertThatTimestampEqualsDateTime(timestamp, dateTime);
        }

        [Test]
        public void TryGenerateTimestampFromString_ReturnsTheSameTimeAsSuppliedDateString()
        {
            const string dateString = "2015-12-25";
            DateTime dateTime = new(2015, 12, 25);

            generator.TryGenerateTimestampFromString(dateString, out Timestamp? timestamp);

            AssertThatTimestampEqualsDateTime(timestamp, dateTime);
        }

        private void AssertThatTimestampEqualsDateTime(Timestamp? timestamp, DateTime dateTime)
        {
            Assert.That(timestamp, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(timestamp.unix, Is.EqualTo(GetUnixTimestampFromDate(dateTime)));

                TryParseDateStringUsingExpectedFormat(timestamp.utc, out DateTime utcDateTime);
                Assert.That(utcDateTime, Is.EqualTo(dateTime));
            });
        }

        private void AssertThatTimestampHasAValidUtcDateString(Timestamp? timestamp)
        {
            Assert.That(timestamp, Is.Not.Null);
            Assert.That(TryParseDateStringUsingExpectedFormat(timestamp.utc, out DateTime _), Is.True);
        }

        private bool TryParseDateStringUsingExpectedFormat(string dateString, out DateTime dateTime)
        {
            const string expectedUtcTimeFormat = "ddd, dd MMM yyyy HH:mm:ss G\\MT";
            return DateTime.TryParseExact(dateString, expectedUtcTimeFormat, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dateTime);
        }

        private ulong GetUnixTimestampFromDate(DateTime dateTime)
        {
            return (ulong)dateTime.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
        }

        private Mock<ITimeProvider> timeProviderMock;
        private TimestampGenerator generator;
    }
}