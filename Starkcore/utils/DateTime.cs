using System;
using System.Globalization;


namespace StarkCore.Utils
{
    public class StarkDate
    {
        public DateTime? Value;

        public StarkDate(DateTime? value)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value == null)
                return null;
            DateTime value = (DateTime)Value;
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }

    public class StarkDateTime
    {
        public DateTime? Value;

        public StarkDateTime(DateTime? value)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value == null)
                return null;
            DateTime value = (DateTime)Value;
            return value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.ffffff", CultureInfo.InvariantCulture) + "+00:00";
        }
    }
}
