using System;

namespace VCollections.Structures
{
    public struct Date
    {
        #region Operator Overload

        public static bool operator >(Date date1, Date date2) => date1.DateTime > date2.DateTime;

        public static bool operator >=(Date date1, Date date2) => date1.DateTime >= date2.DateTime;

        public static bool operator <(Date date1, Date date2) => date1.DateTime < date2.DateTime;

        public static bool operator <=(Date date1, Date date2) => date1.DateTime <= date2.DateTime;

        public static bool operator ==(Date date1, Date date2) => date1.DateTime == date2.DateTime;

        public static bool operator !=(Date date1, Date date2) => date1.DateTime != date2.DateTime;

        public static Date operator +(Date time, TimeSpan timeSpan) => new Date(time.DateTime + timeSpan);

        public static DateTime operator +(Date date, Time time) => 
            new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

        public static TimeSpan operator -(Date date1, Date date2) => date1.DateTime - date2.DateTime;

        #endregion

        #region Implicit Convertions

        public static implicit operator DateTime(Date date) => date.DateTime;

        public static implicit operator Date(DateTime dateTime) => new Date(dateTime);

        public static implicit operator Date(string stringFormat)
        {
            if (!DateTime.TryParse(stringFormat, out DateTime aux)) 
                throw new FormatException( "Error when trying to convert the string format to Date");
            return new Date(aux);
        }

        #endregion

        #region Main value of Date


        #endregion

        #region Public Properties

        public DateTime DateTime { get; }

        public static Date Now => new Date(DateTime.Now);

        public static Date Today => new Date(DateTime.Today);

        public int Day => DateTime.Day;

        public DayOfWeek DayOfWeek => DateTime.DayOfWeek;

        public int DayOfYear => DateTime.DayOfYear;

        public int Month => DateTime.Month;

        public int Year => DateTime.Year;

        public long Ticks => DateTime.Ticks;

        public static Date MinValue => new Date(DateTime.MinValue);

        public static Date MaxValue => new Date(DateTime.MaxValue);

        #endregion

        public Date(DateTime dateTime) => DateTime = 
            new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

        public Date(int year, int month, int day) : this(new DateTime(year, month, day)) { }

        public Date(int year, int month, int day, DateTimeKind kind) 
            : this(new DateTime(year, month, day, 0, 0, 0, kind)) { }

        public Date(long ticks) : this(new DateTime(ticks)) { }

        public Date(long ticks, DateTimeKind kind) : this(new DateTime(ticks, kind)) { }

        #region Public and Static Methods

        public Date AddDays(int days) => new Date(DateTime.AddDays(days));

        public Date AddMonths(int months) => new Date(DateTime.AddMonths(months));

        public Date AddYears(int years) => new Date(DateTime.AddTicks(years));

        public static int Compare(Date date1, Date date2) => DateTime.Compare(date1.DateTime, date2.DateTime);

        public int CompareTo(Date date) => DateTime.CompareTo(date.DateTime);

        public int CompareTo(object value) => DateTime.CompareTo(value);

        public static bool TryParse(string stringFormat, out Date date)
        {
            date = MinValue;
            bool trans;
            if (trans = DateTime.TryParse(stringFormat, out DateTime aux)) date = new Date(aux);
            return trans;
        }

        public static Date Parse(string stringFormat)
        {
            if (!TryParse(stringFormat, out Date parsed))
                throw new FormatException("Error when trying to parse the string format to Date");
            return parsed;
        }

        public static int DaysInMonth(int year, int month) => DateTime.DaysInMonth(year, month);

        public bool Equals(Date date) => DateTime.Equals(date.DateTime);

        public static bool Equals(Date date1, Date date2) => date1.DateTime.Equals(date2.DateTime);

        public override bool Equals(object obj) => DateTime.Equals(obj);

        public override int GetHashCode() => DateTime.GetHashCode();

        #endregion

        #region String Format

        public string ToLongDateString() => DateTime.ToLongDateString();

        public string ToShortDateString() => DateTime.ToShortDateString();

        public override string ToString() => DateTime.ToString("dd/MM/yyyy");

        public string ToString(string format) => DateTime.ToString(format);

        #endregion
    }
}
