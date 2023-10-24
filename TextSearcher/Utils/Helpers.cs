using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Utils
{
    internal static class Helpers
    {
        public static string ToYmdHms(DateTime date)
        {
            return date.ToString(
                "yyyy-MM-dd hh:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture
            );
        }
    }
}
