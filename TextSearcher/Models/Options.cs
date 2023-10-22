using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Models
{
    internal class Options
    {
        public List<Models.FolderInfo> folderInfos = new List<FolderInfo>();
        public List<int> lvColWidths = new List<int>();
        public bool isFormMaximumSize = false;
        public DateTime lastScan = new DateTime(1970, 1, 1);

        public Options() { }
    }
}
