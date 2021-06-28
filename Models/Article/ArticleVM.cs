using System;
using System.Collections.Generic;

namespace Client.Models
{
    public class ArticleVM : BaseWBItem
    {
        public string Content { get; set; }
        public Guid CurrentCommitId { get; set; }
    }
}

