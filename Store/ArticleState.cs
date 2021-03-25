using System;
using Client.Models;
using Fluxor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffPlex.Model;
using Client.Store;
using Client.Services;

namespace Client.Store
{
    public class ArticleState
    {
        public bool IsLoading { get; }
        public ArticleViewModel CurrentArticle { get; }
        public List<RecordMainInfoViewModel> ActiveRecords { get; }
        public CommitViewModel CurrentCommit { get; }
        public string[] CurrentWords { get; }

        public ArticleState(bool isLoading, ArticleViewModel currentArticle,
            List<RecordMainInfoViewModel> activeRecords, CommitViewModel currentCommit, string[] currentWords)
        {
            IsLoading = isLoading;
            CurrentArticle = currentArticle ?? new();
            ActiveRecords = activeRecords ?? new();
            CurrentCommit = currentCommit ?? new();
            CurrentWords = currentWords ?? new[] { "" };
        }
    }

    public class ArticleFeature : Feature<ArticleState>
    {
        public override string GetName() => "Article";

        protected override ArticleState GetInitialState() => new ArticleState(false, null, null, null, null);
    }

    public static class ArticleReducers
    {
        [ReducerMethod]
        public static ArticleState ReduceFetchArticleAction(ArticleState state, FetchArticleAction action)
        {
            return new ArticleState(true, null, null, null, null);
        }

        [ReducerMethod]
        public static ArticleState ReduceFetchArticleResultAction(ArticleState state, FetchArticleResultAction action)
        {
            var article = action.Article?.ArticleInfo;
            var activeRecords = GetActiveRecords(action.Article.CourseInfo.Records, article.Id);
            var currentCommit = article?.Commits?.Last();
            var currentWords = GetCurrentWords(article.Commits, currentCommit);
            return new ArticleState(false, article, activeRecords, currentCommit, currentWords);
        }

        private static List<RecordMainInfoViewModel> GetActiveRecords(List<RecordMainInfoViewModel> records, int currentId)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }
            IEnumerable<RecordMainInfoViewModel> activeList = new List<RecordMainInfoViewModel>();
            var active = records.FirstOrDefault(rec => rec.TargetId == currentId);
            while (active != null)
            {
                activeList = activeList.Prepend(active);
                active = records.FirstOrDefault(r => r.Children != null ? r.Children.Contains(active) : false);
            }
            return activeList.ToList();
        }

        private static string[] GetCurrentWords(List<CommitViewModel> commits, CommitViewModel currentCommit)
        {
            List<string> currentWords = new List<string>();
            for (int i = 0; i < commits.Count; i++)
            {
                int blockSeek = 0;
                foreach (var block in commits[i].DiffBlocks)
                {
                    if (block.DeleteCountA > 0)
                    {
                        currentWords.RemoveRange(block.DeleteStartA, block.DeleteCountA);
                    }
                    if (block.InsertCountB > 0)
                    {
                        currentWords.AddRange(commits[i].DiffWords.Skip(blockSeek).Take(block.InsertCountB));
                    }
                }
                if (commits[i] == currentCommit)
                    break;
            }
            return currentWords.ToArray();
        }
    }
}

public class FetchArticleAction
{
    public int CourseId { get; set; }
    public int ArticleId { get; set; }

    public FetchArticleAction(int courseId, int articleId)
    {
        CourseId = courseId;
        ArticleId = articleId;
    }
}

public class FetchArticleResultAction
{
    public ArticleAggregatedViewModel Article { get; }

    public FetchArticleResultAction(ArticleAggregatedViewModel article)
    {
        Article = article;
    }
}

public class FecthArticleActionEffect : Effect<FetchArticleAction>
{
    private readonly CourseService _courseService;
    private readonly ArticleService _articleService;

    public FecthArticleActionEffect(CourseService courseService, ArticleService articleService)
    {
        _courseService = courseService;
        _articleService = articleService;
    }

    public override async Task HandleAsync(FetchArticleAction action, IDispatcher dispatcher)
    {
        var course = await _courseService.GetCourse(action.CourseId);
        var article = await _articleService.GetArticle(action.ArticleId);
        dispatcher.Dispatch(new SetCourseAction(course));
        dispatcher.Dispatch(new FetchArticleResultAction(new() { CourseInfo = course, ArticleInfo = article }));
    }
}