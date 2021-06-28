using System.Collections.Generic;

namespace Client.Models
{
    public class ArticleAggregatedVM
    {
        public ArticleVM ArticleInfo { get; set; }
        public List<ArticleCommitMainInfoVM> Commits { get; set; }
    }
}
