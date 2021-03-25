using Client.Models;
using Fluxor;

namespace Client.Store
{
    public class ArticleDevState
    {
        public ArticleViewModel CurrentArticle { get; }

        public ArticleDevState(ArticleViewModel currentArticle)
        {
            CurrentArticle = currentArticle ?? new();
        }
    }

    public class ArticleDevFeature : Feature<ArticleDevState>
    {
        public override string GetName() => "ArticleDev";

        protected override ArticleDevState GetInitialState() => new ArticleDevState(null);
    }

    public class 
}
