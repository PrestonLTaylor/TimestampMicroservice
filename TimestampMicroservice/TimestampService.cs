namespace TimestampMicroservice
{
    public record SuccessfulTimestampResponse(ulong unix, string utc);
    public record InvalidTimestampResponse(string error);

    public class TimestampService
    {
        public TimestampService(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public IResult GenerateCurrentTimestamp()
        {
            return Results.Ok(GenerateTimestampFromDate(_timeProvider.UtcNow));
        }
        
        public IResult GenerateTimestampFromString(string dateString)
        {
            if (ulong.TryParse(dateString, out ulong unixTimestamp))
            {
                return Results.Ok(GenerateTimestampFromUnixTimestamp(unixTimestamp));
            }
            else if (DateTime.TryParse(dateString, out DateTime date))
            {
                return Results.Ok(GenerateTimestampFromDate(date));
            }
            else
            {
                return Results.Ok(GenerateInvalidDateError());
            }
        }

        private SuccessfulTimestampResponse GenerateTimestampFromUnixTimestamp(ulong unixTimestamp)
        {
            return GenerateTimestampFromDate(DateTime.UnixEpoch.AddMilliseconds(unixTimestamp));
        }

        private SuccessfulTimestampResponse GenerateTimestampFromDate(DateTime date)
        {
            return new SuccessfulTimestampResponse(GetUnixTimestampFromDate(date), GetDateAsString(date));
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

        private InvalidTimestampResponse GenerateInvalidDateError()
        {
            return new InvalidTimestampResponse("Invalid Date");
        }

        private readonly ITimeProvider _timeProvider;
    }
}
