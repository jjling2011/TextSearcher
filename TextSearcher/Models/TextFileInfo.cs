using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Models
{
    internal class TextFileInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string file { get; set; }

        public string ext { get; set; }

        [Index]
        public string path { get; set; }

        public DateTime modify { get; set; }

        public bool deleted { get; set; }

        public string content { get; set; }

        public void Copy(TextFileInfo o)
        {
            file = o.file;
            ext = o.ext;
            path = o.path;
            modify = o.modify;
            deleted = o.deleted;
            content = o.content;
        }

        public string[] ToArray(IEnumerable<string> keywords)
        {
            var parts = Utils.Helpers.GetKeywordParts(content, keywords);
            return new string[] { "", file, path, Utils.Helpers.ToYmdHms(modify), parts };
        }
    }
}
