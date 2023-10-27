using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Models
{
    internal class StrRange
    {
        public int start;
        public int end;

        public StrRange(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public bool Fix(int len)
        {
            if (start > end)
            {
                var t = start;
                start = end;
                end = t;
            }

            if (start < 0)
            {
                start = 0;
            }

            if (end > len)
            {
                end = len;
            }

            return start < end;
        }
    }
}
