using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class NewCommentVM
    {
        public Guid? ParentId { get; set; }
        public string Content { get; set; }
    }
}
