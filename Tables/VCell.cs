using System;
using VCollections.Structures;
using VC = VCollections.VCollection;

namespace VCollections.Tables
{
    public class VCell
    {
        private const string db2False = "\u0000";
        private const string db2True = "\u0001";
        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                if (IsNull(value)) throw new InvalidCastException(
                    "El campo no puede tener un valor nulo");
                _value = value;
            }
        }

        public VCell(object value) => Value = value;

        #region Tool Methods

        private bool IsNull(object value) => value == null;

        public bool IsDBNull() => _value == DBNull.Value;

        public T ToType<T>() => !IsDBNull() && Value is T ? (T)Value : default;

        public short ToShort() => IsDBNull() ? (short)0 : Value is short ? (short)Value : Convert.ToInt16(Value);

        public int ToInt() => IsDBNull() ? 0 : Value is int ? (int)Value : Convert.ToInt32(Value);

        public long ToLong() => IsDBNull() ? 0 : Value is long ? (long)Value : Convert.ToInt64(Value);

        public float ToFloat() => (float)ToDouble();

        public double ToDouble() => IsDBNull() ? 0 : Value is double ? (double)Value : Convert.ToDouble(Value);

        public bool ToBool()
        {
            if (IsDBNull()) return false;
            else if (Value is bool) return (bool)Value;
            else if (Value.Equals(db2False)) return false;
            else if (Value.Equals(db2True)) return true;
            else return Convert.ToBoolean(Value);
        }

        public DateTime ToDateTime()
        {
            if (IsDBNull()) return DateTime.MinValue;
            if (Value is DateTime) return (DateTime)Value;
            else if (Value is Date) return ((Date)Value).DateTime;
            else if (Value is Time) return ((Time)Value).DateTime;
            else if (Value is TimeSpan) return new DateTime(((TimeSpan)Value).Ticks);
            else return Convert.ToDateTime(Value);
        }

        public Date ToDate()
        {
            if (IsDBNull()) return Date.MinValue;
            else if (Value is Date) return (Date)Value;
            else if (Value is Time) return ((Time)Value).DateTime;
            else if (Value is DateTime) return (DateTime)Value;
            else if (Value is TimeSpan) return new Date(((TimeSpan)Value).Ticks);
            else return Convert.ToDateTime(Value);
        }

        public Time ToTime()
        {
            if (IsDBNull()) return Time.MinValue;
            else if (Value is Date) return ((Date)Value).DateTime;
            else if (Value is Time) return (Time)Value;
            else if (Value is DateTime) return (DateTime)Value;
            else if (Value is TimeSpan) return new Time(((TimeSpan)Value).Ticks);
            else return Convert.ToDateTime(Value);
        }

        public TimeSpan ToTimeSpan()
        {
            if (IsDBNull()) return TimeSpan.MinValue;
            else if (Value is TimeSpan) return (TimeSpan)Value;
            else if (Value is Date) return new TimeSpan(((Date)Value).Ticks);
            else if (Value is Time) return new TimeSpan(((Time)Value).Ticks);
            else if (Value is DateTime) return new TimeSpan(((DateTime)Value).Ticks);
            else return new TimeSpan(Convert.ToDateTime(Value).Ticks);
        }

        public override string ToString() => IsDBNull() ? "" : (Value is string ? (string)Value : Value.ToString()).Trim();

        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object obj) => Value.Equals(obj);

        public bool Equals(VCell cell) => Value.Equals(cell.Value);

        public bool In(params object[] array) => VC.Contains(ref array, Value);

        public int Compare<T>(params T[] withArray) => VC.Comparer(ToType<T[]>(), withArray);

        #region Between (startRange, endRange)

        public bool Between(short startRange, short endRange) => ToShort() >= startRange && ToShort() <= endRange;

        public bool Between(int startRange, int endRange) => ToInt() >= startRange && ToInt() <= endRange;

        public bool Between(long startRange, long endRange) => ToLong() >= startRange && ToLong() <= endRange;

        public bool Between(float startRange, float endRange) => ToFloat() >= startRange && ToFloat() <= endRange;

        public bool Between(double startRange, double endRange) => ToDouble() >= startRange && ToDouble() <= endRange;

        public bool Between(DateTime startRange, DateTime endRange) => ToDateTime() >= startRange && ToDateTime() <= endRange;

        public bool Between(Date startRange, Date endRange) => ToDate() >= startRange && ToDate() <= endRange;

        public bool Between(Time startRange, Time endRange) => ToTime() >= startRange && ToTime() <= endRange;

        #endregion

        #region String Methods

        public int StringCompare(string text) => ToString().CompareTo(text);

        public bool StartsWith(string text) => ToString().StartsWith(text);

        public bool EndsWith(string text) => ToString().EndsWith(text);

        public bool Contains(string text) => ToString().Contains(text);

        public bool RegExp(string regularExpression) => 
            string.IsNullOrEmpty(regularExpression) ? false 
            : System.Text.RegularExpressions.Regex.IsMatch(ToString(), regularExpression);

        #endregion

        #endregion
    }
}
