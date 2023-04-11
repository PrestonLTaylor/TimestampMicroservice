namespace TimestampMicroservice
{
    public sealed record Timestamp(ulong unix, string utc);

    public sealed class TimestampGenerator
    {
        static public ulong GetUnixTimestampFromDate(DateTime dateTime)
        {
            return (ulong)dateTime.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
        }

        public TimestampGenerator(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public Timestamp GenerateCurrentTimestamp()
        {
            return GenerateTimestampFromDate(_timeProvider.UtcNow);
        }

        public Timestamp GenerateTimestampFromUnixTimestamp(ulong unixTimestamp)
        {
            return GenerateTimestampFromDate(DateTime.UnixEpoch.AddMilliseconds(unixTimestamp));
        }

        public bool TryGenerateTimestampFromDateString(string dateString, out Timestamp? timestamp)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
            {
                timestamp = GenerateTimestampFromDate(date);
                return true;
            }

            timestamp = null;
            return false;
        }

        private Timestamp GenerateTimestampFromDate(DateTime date)
        {
            return new Timestamp(GetUnixTimestampFromDate(date), GetDateAsString(date));
        }

        private string GetDateAsString(DateTime date)
        {
            const string utcTimeFormat = "ddd, dd MMM yyyy HH:mm:ss G\\MT";
            return date.ToString(utcTimeFormat);
        }

        private readonly ITimeProvider _timeProvider;
    }
}
