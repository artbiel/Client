using Client.Models;
using Fluxor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Services;
using System;
using DiffPlex;
using Client.Infrastructure;
using Radzen;

namespace Client.Store
{
    public class ArticleState
    {
        public bool IsLoading { get; }
        public ArticleViewModel CurrentArticle { get; }
        public ArticleCommitViewModel CurrentCommit { get; }
        public string[] CurrentWords { get; }

        public ArticleState(bool isLoading, ArticleViewModel currentArticle, ArticleCommitViewModel currentCommit, string[] currentWords)
        {
            IsLoading = isLoading;
            CurrentArticle = currentArticle;
            CurrentCommit = currentCommit;
            CurrentWords = currentWords;
        }
    }

    public class ArticleFeature : Feature<ArticleState>
    {
        public override string GetName() => "Article";

        protected override ArticleState GetInitialState() => new ArticleState(false, null, null, null);
    }

    public static class ArticleReducers
    {
        [ReducerMethod]
        public static ArticleState ReduceFetchArticleAction(ArticleState state, FetchArticleAction action)
        {
            return new ArticleState(true, null, null, null);
        }

        [ReducerMethod]
        public static ArticleState ReduceFetchArticleResultAction(ArticleState state, FetchArticleResultAction action)
        {
            var article = action.Article?.ArticleInfo;
            Console.WriteLine(article.Commits.Count);
            var currentCommit = article?.Commits?.LastOrDefault();
            var currentWords = ArticleHelpers.GetCurrentWords(article?.Commits, currentCommit);
            return new ArticleState(false, article, currentCommit, currentWords);
        }

        [ReducerMethod]
        public static ArticleState ReduceAddArticleAction(ArticleState state, AddArticleAction action)
        {
            return new ArticleState(state.IsLoading, state.CurrentArticle, state.CurrentCommit, state.CurrentWords);
        }

        [ReducerMethod]
        public static ArticleState ReduceRemoveArticleAction(ArticleState state, RemoveArticleAction action)
        {
            return new ArticleState(state.IsLoading, state.CurrentArticle, state.CurrentCommit, state.CurrentWords);
        }

        [ReducerMethod]
        public static ArticleState ReduceClearArticleStateAction(ArticleState state, ClearArticleStateAction action)
        {
            return new ArticleState(false, null, null, null);
        }

        [ReducerMethod]
        public static ArticleState ReduceSetCurrentCommitAction(ArticleState state, SetCurrentCommitAction action)
        {
            var currentCommit = state.CurrentArticle.Commits.FirstOrDefault(c => c.Id == action.CommitId) ?? state.CurrentCommit;
            string[] currentWords = currentCommit != state.CurrentCommit ? ArticleHelpers.GetCurrentWords(state.CurrentArticle.Commits,
                currentCommit) : state.CurrentWords;
            Console.WriteLine(currentCommit.Title);
            Console.WriteLine(currentWords.Last());
            return new ArticleState(state.IsLoading, state.CurrentArticle, currentCommit, currentWords);
        }

    }

    public static class ArticleHelpers
    {

        public static string[] GetCurrentWords(List<ArticleCommitViewModel> commits, ArticleCommitViewModel currentCommit)
        {
            if (commits == null)
                return new string[] { "" };
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
                if (commits[i].Id == currentCommit.Id)
                    break;
            }
            return currentWords.ToArray();
        }
    }

    public class FetchArticleAction
    {
        public int CourseId { get; set; }
        public int ArticleId { get; set; }
        public bool DevMode { get; set; }

        public FetchArticleAction(int courseId, int articleId, bool devMode)
        {
            CourseId = courseId;
            ArticleId = articleId;
            DevMode = devMode;
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

    public class ClearArticleStateAction { }

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
            var course = await _courseService.GetCourse(action.CourseId, action.DevMode);
            var article = await _articleService.GetArticle(action.ArticleId, action.DevMode);
            dispatcher.Dispatch(new SetCourseAction(course, action.DevMode));
            dispatcher.Dispatch(new SetActiveRecordsAction(action.ArticleId));
            dispatcher.Dispatch(new FetchArticleResultAction(new() { CourseInfo = course, ArticleInfo = article }));
        }
    }

    public class AddArticleAction
    {
        public int Id { get; set; }
        public string Name { get; }

        public AddArticleAction(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class AddArticleActionEffect : Effect<AddArticleAction>
    {
        private ArticleService _articleService;

        public AddArticleActionEffect(ArticleService articleService)
        {
            _articleService = articleService;
        }

        public override async Task HandleAsync(AddArticleAction action, IDispatcher dispatcher)
        {
            var initialCommit = new ArticleCommitViewModel
            {
                CreatedAt = DateTime.Now,
                CreatedBy = "Тёмыч",
                DiffBlocks = new List<ArticleDiffBlock>(),
                Id = new Random().Next(),
                Description = "Initial Commit",
                Title = "Initial Commit",
                CommitState = CommitState.Commited,
                Type = ArticleCommitType.Initital
            };
            var article = new ArticleViewModel() { Id = action.Id, Title = action.Name, Commits = new() { initialCommit } };
            await _articleService.AddArticleToLocalStorage(article);
        }
    }

    public class RemoveArticleAction
    {
        public int Id { get; set; }

        public RemoveArticleAction(int id)
        {
            Id = id;
        }
    }

    public class RemoveArticleActionEffect : Effect<RemoveArticleAction>
    {
        private ArticleService _articleService;
        private IState<ArticleState> _articleState;

        public RemoveArticleActionEffect(ArticleService articleService, IState<ArticleState> articleState)
        {
            _articleService = articleService;
            _articleState = articleState;
        }

        public override async Task HandleAsync(RemoveArticleAction action, IDispatcher dispatcher)
        {
            await _articleService.RemoveArticleFromLocalStorage(action.Id);
            if (action.Id == _articleState.Value.CurrentArticle.Id)
                dispatcher.Dispatch(new ClearArticleStateAction());
        }
    }

    public class SaveArticleContentAction
    {
        public int ArticleId { get; }
        public int CurrentCommitId { get; }
        public string Content { get; }

        public SaveArticleContentAction(int articleId, int currentCommitId, string content)
        {
            ArticleId = articleId;
            CurrentCommitId = currentCommitId;
            Content = content;
        }
    }

    public class SaveArticleActionEffect : Effect<SaveArticleContentAction>
    {
        private ArticleService _articleService;
        private IState<CourseState> _courseState;
        private NotificationService _notificationService;

        public SaveArticleActionEffect(ArticleService articleService, IState<CourseState> courseState, NotificationService notificationService)
        {
            _articleService = articleService;
            _courseState = courseState;
            _notificationService = notificationService;
        }

        public override async Task HandleAsync(SaveArticleContentAction action, IDispatcher dispatcher)
        {
            var article = await _articleService.GetArticle(action.ArticleId, devMode: true);
            var currentCommit = article.Commits.FirstOrDefault(c => c.Id == action.CurrentCommitId);
            if (currentCommit.CommitState == CommitState.Developing)
            {
                var diffResult = Differ.Instance.CreateWordDiffs(
                    string.Join("", ArticleHelpers.GetCurrentWords(article.Commits, currentCommit.PrevCommit)),
                    action.Content, false, false, new[] { ' ' });
                var diffWords = new List<string>();
                foreach (var diff in diffResult.DiffBlocks)
                {
                    if (diff.InsertCountB != 0)
                    {
                        diffWords.AddRange(diffResult.PiecesNew.Skip(diff.InsertStartB).Take(diff.InsertCountB));
                    }
                }
                currentCommit.DiffBlocks = diffResult.DiffBlocks.ToArticleDiffBlocks();
                currentCommit.DiffWords = diffWords.ToArray();
                await _articleService.AddArticleToLocalStorage(article);
            }
            else
            {
                var oldContent = string.Join("", ArticleHelpers.GetCurrentWords(article.Commits, currentCommit));
                var diffResult = Differ.Instance.CreateWordDiffs(oldContent, action.Content, false, false, new[] { ' ' });
                var diffWords = new List<string>();
                foreach (var diff in diffResult.DiffBlocks)
                {
                    if (diff.InsertCountB != 0)
                    {
                        diffWords.AddRange(diffResult.PiecesNew.Skip(diff.InsertStartB).Take(diff.InsertCountB));
                    }
                }
                var newDevCommit = new ArticleCommitViewModel
                {
                    Id = new Random().Next(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Тёмыч",
                    Title = "123",
                    CommitState = CommitState.Developing,
                    DiffBlocks = diffResult.DiffBlocks.ToArticleDiffBlocks(),
                    DiffWords = diffWords.ToArray(),
                    PrevCommit = currentCommit,
                };
                article.Commits = article.Commits.Append(newDevCommit).ToList();
                await _articleService.AddArticleToLocalStorage(article);
                dispatcher.Dispatch(new FetchArticleAction(_courseState.Value.CurrentCourse.Id, action.ArticleId, devMode: true));
            }
            _notificationService.Notify(new() { Severity = NotificationSeverity.Success, Summary = "Successfully saved!", Duration = 20000, Style = "position: absolute; right: -10px; bottom: 200px" });
        }
    }

    public class SetCurrentCommitAction
    {
        public int CommitId { get; }

        public SetCurrentCommitAction(int commitId)
        {
            CommitId = commitId;
        }
    }
}