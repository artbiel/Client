using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class RecordVM : TreeBaseModel<RecordVM>, ICloneable
    {
        [Required]
        public RecordType Type { get; set; }
        [Required]
        public string Title { get; set; }

        public object Clone() => MemberwiseClone();
    }

    public enum RecordType
    {
        Article,
        Group,
        Root
    }
}
