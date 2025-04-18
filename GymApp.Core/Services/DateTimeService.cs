public abstract class DateTimeService
{
    private static DateTimeService _current = Current;

    public static DateTimeService Current
    {
        get
        {
            return _current ?? (_current = new DefaultDateTimeProvider());
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            _current = value;
        }
    }

    public abstract DateTime Now { get; }

    public static void ResetToDefault()
    {
        _current = Current;
    }
}