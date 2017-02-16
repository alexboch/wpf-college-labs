using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birzha
{
    class ChartPoint
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public ChartPoint(DateTime date, double value)
        {
            Date = date;
            Value = value;
        }
    }
}
