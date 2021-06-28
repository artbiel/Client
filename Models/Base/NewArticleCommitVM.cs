using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class NewArticleCommitVM
    {
        [Required]
        public ArticleCommitType Type { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
