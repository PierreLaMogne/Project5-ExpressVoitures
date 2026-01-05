using System.ComponentModel.DataAnnotations;

namespace Net_P5.Attributes
{
    public class CurrentYearAttribute : RangeAttribute
    {
        public CurrentYearAttribute(double minimum) : base(minimum, DateTime.Now.Year)
        {
        }
    }
}