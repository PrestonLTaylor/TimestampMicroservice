namespace TimestampMicroservice
{
    public interface ITimeProvider
    {
        public DateTime UtcNow { get; }
    }

    public sealed class TimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
