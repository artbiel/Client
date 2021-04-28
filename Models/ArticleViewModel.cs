using System.Collections.Generic;

namespace Client.Models
{
    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<ArticleCommitViewModel> Commits { get; set; }
    }
}

