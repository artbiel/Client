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
using Material.Blazor;

namespace Client.Store
{
    public record ArticleState(bool IsLoading, ArticleVM CurrentArticle, List<ArticleCommitMainInfoVM> Commits);

    public class ArticleFeature : Feature<ArticleState>
    {
        public override string GetName() => "Article";

        protected override ArticleState GetInitialState() => new ArticleState(false, null, null);
    }

    public static class ArticleReducers
    {
        [ReducerMethod]
        public static ArticleState ReduceFetchArticleAction(ArticleState state, FetchArticleAction action)
        {
            return new ArticleState(true, null, null);
        }

        [ReducerMethod]
        public static ArticleState ReduceFetchArticleResultAction(ArticleState state, FetchArticleResultAction action)
        {
            //var currentCommit = action.Article?.Commits?.Where(c => c.CommitState == CommitState.Commited).LastOrDefault();
            //var currentWords = ArticleHelpers.GetCurrentWords(action.Article?.Commits, currentCommit);
            return new ArticleState(false, action.Article.ArticleInfo, action.Article.Commits);
        }

        [ReducerMethod]
        public static ArticleState ReduceClearArticleStateAction(ArticleState state, ClearArticleStateAction action)
        {
            return new ArticleState(false, null, null);
        }

        [ReducerMethod]
        public static ArticleState ReduceSetCurrentCommitAction(ArticleState state, SetCurrentArticleCommitAction action)
        {
            var currentCommit = state.Commits.FirstOrDefault(c => c.Id == action.CommitId) 
                ?? state.Commits.FirstOrDefault(c => c.Id == state.CurrentArticle.CurrentCommitId);
            state.CurrentArticle.Content = currentCommit.Content;
            state.CurrentArticle.CurrentCommitId = action.CommitId;
            return new ArticleState(state.IsLoading, state.CurrentArticle, state.Commits);
        }

        //[ReducerMethod]
        //public static ArticleState ReduceSetArticleCommitsAction(ArticleState state, SetArticleCommitsAction action)
        //{
        //    string[] currentWords = action.CurrentCommit != state.CurrentCommit ? ArticleHelpers.GetCurrentWords(action.Commits,
        //        action.CurrentCommit) : state.CurrentWords;
        //    state.CurrentArticle.Commits = action.Commits;
        //    Console.WriteLine(state.CurrentArticle.Commits.Count);
        //    return new ArticleState(state.IsLoading, state.CurrentArticle, action.CurrentCommit, currentWords);
        //}

    }

    public static class ArticleHelpers
    {

        public static string[] GetCurrentWords(List<ArticleCommitVM> commits, ArticleCommitVM currentCommit)
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

    public record FetchArticleAction(Guid CourseId, Guid ArticleId, Guid? CommitId);

    public record FetchArticleResultAction(ArticleAggregatedVM Article);

    public class ClearArticleStateAction { }

    public class FecthArticleActionEffect : Effect<FetchArticleAction>
    {
        private readonly CourseService _courseService;
        private readonly ArticleService _articleService;
        private readonly IState<CourseState> _courseState;

        public FecthArticleActionEffect(CourseService courseService, ArticleService articleService, IState<CourseState> courseState)
        {
            _courseService = courseService;
            _articleService = articleService;
            _courseState = courseState;
        }

        public override async Task HandleAsync(FetchArticleAction action, IDispatcher dispatcher)
        {
            var articleAggreate = await _articleService.GetArticle(action.ArticleId, action.CommitId);
            if(_courseState.Value.CurrentCourse == null || _courseState.Value.CurrentCourse.Id != action.CourseId)
            {
                var course = await _courseService.GetCourse(action.CourseId);
                dispatcher.Dispatch(new SetCourseAction(course));
            }
            dispatcher.Dispatch(new SetActiveRecordsAction(action.ArticleId));
            dispatcher.Dispatch(new FetchArticleResultAction(articleAggreate));
        }
    }

    public record SaveArticleContentAction(Guid ArticleId, Guid CurrentCommitId, string Content);

    public class SaveArticleActionEffect : Effect<SaveArticleContentAction>
    {
        private ArticleService _articleService;
        private IState<CourseState> _courseState;

        public SaveArticleActionEffect(ArticleService articleService, IState<CourseState> courseState)
        {
            _articleService = articleService;
            _courseState = courseState;
        }

        public override async Task HandleAsync(SaveArticleContentAction action, IDispatcher dispatcher)
        {
            //var article = await _articleService.GetArticle(action.ArticleId);
            //var currentCommit = article.Commits.FirstOrDefault(c => c.Id == action.CurrentCommitId);
            //if (currentCommit.CommitState == CommitState.Unsent)
            //{
            //    var diffResult = Differ.Instance.CreateWordDiffs(
            //        string.Join("", ArticleHelpers.GetCurrentWords(article.Commits, currentCommit.PrevCommit as ArticleCommitVM)),
            //        action.Content, false, false, new[] { ' ' });
            //    var diffWords = new List<string>();
            //    foreach (var diff in diffResult.DiffBlocks)
            //    {
            //        if (diff.InsertCountB != 0)
            //        {
            //            diffWords.AddRange(diffResult.PiecesNew.Skip(diff.InsertStartB).Take(diff.InsertCountB));
            //        }
            //    }
            //    currentCommit.DiffBlocks = diffResult.DiffBlocks.ToArticleDiffBlocks();
            //    currentCommit.DiffWords = diffWords.ToArray();
            //    //await _articleService.AddArticleToLocalStorage(article);
            //}
            //else
            //{
            //    var oldContent = string.Join("", ArticleHelpers.GetCurrentWords(article.Commits, currentCommit));
            //    var diffResult = Differ.Instance.CreateWordDiffs(oldContent, action.Content, false, false, new[] { ' ' });
            //    var diffWords = new List<string>();
            //    foreach (var diff in diffResult.DiffBlocks)
            //    {
            //        if (diff.InsertCountB != 0)
            //        {
            //            diffWords.AddRange(diffResult.PiecesNew.Skip(diff.InsertStartB).Take(diff.InsertCountB));
            //        }
            //    }
            //    var newDevCommit = new ArticleCommitVM
            //    {
            //        Id = Guid.NewGuid(),
            //        CreatedAt = DateTime.Now,
            //        CreatedBy = "Тёмыч",
            //        Title = "123",
            //        CommitState = CommitState.Unsent,
            //        DiffBlocks = diffResult.DiffBlocks.ToArticleDiffBlocks(),
            //        DiffWords = diffWords.ToArray(),
            //        PrevCommitId = currentCommit.Id,
            //    };
            //    article.Commits = article.Commits.Append(newDevCommit).ToList();
            //    //await _articleService.AddArticleToLocalStorage(article);
            //    //dispatcher.Dispatch(new FetchArticleAction(_courseState.Value.CurrentCourse.Id, action.ArticleId));
            //}
        }
    }

    public record SetCurrentArticleCommitAction(Guid CommitId);

    //public record SetArticleCommitsAction(List<ArticleCommitVM> Commits, ArticleCommitVM CurrentCommit);

    public record AddArticleCommitAction(Guid ArticleId, Guid PrevCommitId, string Title, string Content,
        string Description, ArticleCommitType CommitType);

    public class AddArticleCommitActionEffect : Effect<AddArticleCommitAction>
    {
        private ArticleService _articleService;
        private IMBToastService _toastService;

        public AddArticleCommitActionEffect(ArticleService articleService, IMBToastService toastService)
        {
            _articleService = articleService;
            _toastService = toastService;
        }

        public override async Task HandleAsync(AddArticleCommitAction action, IDispatcher dispatcher)
        {
            var article = await _articleService.GetArticle(action.ArticleId, action.PrevCommitId);
            var prevCommit = article?.Commits?.FirstOrDefault(c => c.Id == action.PrevCommitId);
            if (prevCommit != null)
            {
                var oldContent = article.ArticleInfo.Content;
                var diffResult = Differ.Instance.CreateWordDiffs(oldContent, action.Content, false, false, new[] { ' ' });
                var diffWords = new List<string>();
                foreach (var diff in diffResult.DiffBlocks)
                {
                    if (diff.InsertCountB != 0)
                    {
                        diffWords.AddRange(diffResult.PiecesNew.Skip(diff.InsertStartB).Take(diff.InsertCountB));
                    }
                }
                //var commit = new ArticleCommitVM
                //{
                //    Id = Guid.NewGuid(),
                //    Title = action.Title,
                //    Description = action.Description,
                //    Type = action.CommitType,
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = "artbiel",
                //    CommitState = CommitState.Unsent,
                //    DiffBlocks = diffResult.DiffBlocks.ToArticleDiffBlocks(),
                //    DiffWords = diffWords.ToArray(),
                //    PrevCommitId = action.PrevCommitId
                //};
                //article.Commits.Add(commit);
                //dispatcher.Dispatch(new SetArticleCommitsAction(article.Commits, commit));
            }
            var commit = new ArticleCommitMainInfoVM
            {
                Id = Guid.NewGuid(),
                Title = action.Title,
                Description = action.Description,
                Type = action.CommitType,
                CreatedAt = DateTime.Now,
                CreatedBy = "artbiel",
                CommitState = CommitState.Unsent,
                PrevCommitId = action.PrevCommitId,
                Content = action.Content
            };
            await _articleService.AddCommit(action.ArticleId, commit);
            _toastService.ShowToast(MBToastLevel.Success, "Commit was successfully added!");
        }
    }


}