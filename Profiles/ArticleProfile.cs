using AutoMapper;
using Client.Models;

namespace Client.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<NewArticleCommitVM, ArticleCommitVM>();
        }
    }
}
