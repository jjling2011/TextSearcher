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

        public string[] ToArray()
        {
            return new string[]
            {
                "",
                file,
                path,
                $"{modify.ToLongDateString()} {modify.ToShortTimeString()}",
                content,
            };
        }
    }
}
