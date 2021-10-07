using System;

namespace VCollections.Structures
{
    public struct Time
    {
        #region Operator Overloads

        public static bool operator >(Time time1, Time time2) => time1.DateTime > time2.DateTime;

        public static bool operator >=(Time time1, Time time2) => time1.DateTime >= time2.DateTime;

        public static bool operator <(Time time1, Time time2) => time1.DateTime < time2.DateTime;

        public static bool operator <=(Time time1, Time time2) => time1.DateTime <= time2.DateTime;

        public static bool operator ==(Time time1, Time time2) => time1.DateTime == time2.DateTime;

        public static bool operator !=(Time time1, Time time2) => time1.DateTime != time2.DateTime;

        public static Time operator +(Time time, TimeSpan timeSpan) => new Time(time.DateTime + timeSpan);

        public static DateTime operator +(Time time, Date date) => 
            new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

        public static TimeSpan operator -(Time time1, Time time2) => time1.DateTime - time2.DateTime;

        #endregion

        #region Implicit Convertions

        public static implicit operator DateTime(Time time) => time.DateTime;

        public static implicit operator Time(DateTime dateTime) => new Time(dateTime);

        public static implicit operator Time(string stringFormat)
        {
            if (!DateTime.TryParse(stringFormat, out DateTime aux))
                throw new FormatException("Error when trying to convert the string format to Time.");
            return new Time(aux);
        }

        #endregion

        #region Public Propierties

        public static Time Now => new Time(DateTime.Now);

        public int Hour => DateTime.Hour;

        public DateTimeKind Kind => DateTime.Kind;

        public int Milisecond => DateTime.Millisecond;

        public int Minute => DateTime.Minute;

        public int Second => DateTime.Second;

        public long Ticks => DateTime.Ticks;

        public TimeSpan TimeOfDay => DateTime.TimeOfDay;

        public DateTime DateTime { get; }

        public static Time MinValue => new Time(DateTime.MinValue);

        public static Time MaxValue => new Time(DateTime.MaxValue);

        #endregion

        public Time(DateTime dateTime) => 
            DateTime = new DateTime(1, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second);

        public Time(int hour, int minute, int second) : this(new DateTime(1, 1, 1, hour, minute, second)) { }

        public Time(int hour, int minute, int second, DateTimeKind kind) 
            : this(new DateTime(1, 1, 1, hour, minute, second, kind)) { }

        public Time(long ticks) : this(new DateTime(ticks)) { }

        public Time(long ticks, DateTimeKind kind) : this(new DateTime(ticks, kind)) { }

        #region Public Methods

        public Time Add(TimeSpan timeSpan) => new Time(DateTime.Add(timeSpan));

        public Time AddMiliseconds(double mili) => new Time(DateTime.AddMilliseconds(mili));

        public Time AddHours(double hours) => new Time(DateTime.AddHours(hours));

        public Time AddMinutes(double minutes) => new Time(DateTime.AddMinutes(minutes));

        public Time AddSeconds(double seconds) => new Time(DateTime.AddSeconds(seconds));

        public Time AddTicks(long ticks) => new Time(DateTime.AddTicks(ticks));

        public static int Compare(Time time1, Time time2) => DateTime.Compare(time1.DateTime, time2.DateTime);

        public int CompareTo(Time time) => DateTime.CompareTo(time.DateTime);

        public int CompareTo(object value) => DateTime.CompareTo(value);

        public static bool TryParse(string stringFormat, out Time time)
        {
            time = MinValue;
            bool trans;
            if (trans = DateTime.TryParse(stringFormat, out DateTime aux)) time = new Time(aux);
            return trans;
        }

        public static Time Parse(string stringFormat)
        {
            if (!TryParse(stringFormat, out Time parsed))
                throw new FormatException("Error when trying to parse the string format to DateTime.");
            return parsed;
        }

        public bool Equals(Time time) => DateTime.Equals(time.DateTime);

        public static bool Equals(Time time1, Time time2) => time1.Equals(time2);

        public DateTime ToDateTimePickerValue() => new DateTime(1901, 1, 1, Hour, Minute, Second);

        public Time ToLocalTime() => new Time(DateTime.ToLocalTime());

        public override bool Equals(object obj) => DateTime.Equals(obj);

        public override int GetHashCode() => DateTime.GetHashCode();

        #endregion

        #region String Format

        public string ToLongTimeString() => DateTime.ToLongTimeString();

        public string ToShortTimeString() => DateTime.ToShortTimeString();

        public override string ToString() => DateTime.ToString("HH:mm:ss");

        public string ToString(string format) => DateTime.ToString(format);

        #endregion
    }
}
