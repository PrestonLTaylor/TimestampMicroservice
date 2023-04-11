namespace TimestampMicroservice
{
    public sealed record Timestamp(ulong unix, string utc);

    public sealed class TimestampGenerator
    {
        public TimestampGenerator(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public Timestamp GenerateCurrentTimestamp()
        {
            return GenerateTimestampFromDate(_timeProvider.UtcNow);
        }

        public bool TryGenerateTimestampFromString(string dateString, out Timestamp? timestamp)
        {
            if (ulong.TryParse(dateString, out ulong unixTimestamp))
            {
                timestamp = GenerateTimestampFromUnixTimestamp(unixTimestamp);
                return true;
            }
            else if (DateTime.TryParse(dateString, out DateTime date))
            {
                timestamp = GenerateTimestampFromDate(date);
                return true;
            }

            timestamp = null;
            return false;
        }

        private Timestamp GenerateTimestampFromUnixTimestamp(ulong unixTimestamp)
        {
            return GenerateTimestampFromDate(DateTime.UnixEpoch.AddMilliseconds(unixTimestamp));
        }

        private Timestamp GenerateTimestampFromDate(DateTime date)
        {
            return new Timestamp(GetUnixTimestampFromDate(date), GetDateAsString(date));
        }

        private ulong GetUnixTimestampFromDate(DateTime dateTime)
        {
            return (ulong)dateTime.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
        }

        private string GetDateAsString(DateTime date)
        {
            const string utcTimeFormat = "ddd, dd MMM yyyy HH:mm:ss G\\MT";
            return date.ToString(utcTimeFormat);
        }

        private readonly ITimeProvider _timeProvider;
    }
}
