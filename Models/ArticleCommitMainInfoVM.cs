using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ArticleCommitMainInfoVM : BaseCommit
    {
        public ArticleCommitType Type { get; set; }
        public string Content { get; set; }
    }
}
