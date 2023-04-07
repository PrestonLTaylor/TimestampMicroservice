namespace TimestampMicroservice
{
    public interface ITimeProvider
    {
        public DateTime UtcNow { get; }
    }

    public class TimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
