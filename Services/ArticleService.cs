using Blazored.LocalStorage;
using Client.Infrastructure;
using Client.Models;
using DiffPlex.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Services
{
    public class ArticleService
    {
        private const string key = "article_commit_";

        private ILocalStorageService _localStorageService;
        private CourseService _courseService;

        public ArticleService(ILocalStorageService localStorageService, CourseService courseService)
        {
            _localStorageService = localStorageService;
            _courseService = courseService;
        }

        public async Task<ArticleAggregatedVM> GetArticle(Guid id, Guid? commitId)
        {
            //if (devMode)
            //{
            //    if (await _localStorageService.ContainKeyAsync(devKey + id))
            //    {
            //        var article = await _localStorageService.GetItemAsync<ArticleViewModel>(devKey + id);
            //        return ArticleConverter.FromStorage(article);
            //    }
            //    else
            //    {
            //        if ((await _courseService.GetCourse(1, devMode)).Records.Any(r => r.TargetId == id))
            //        {
            //            var article = GetExample(id);
            //            await AddArticleToLocalStorage(ArticleConverter.ToStorage(article));
            //            return article;
            //        }
            //    }
            //}
            if (await _localStorageService.ContainKeyAsync("workbanch_" + id))
            {
                var commit1 = new ArticleCommitMainInfoVM
                {
                    Id = new Guid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                    Title = "Created",
                    CreatedAt = new DateTime(2021, 05, 15, 22, 23, 22),
                    CreatedBy = "artbiel",
                    Description = "Article created",
                    CommitState = CommitState.Commited,
                    Type = ArticleCommitType.Addition,
                    Content = ""
                };
                var commits = new List<ArticleCommitMainInfoVM>() { commit1 };
                if (await _localStorageService.ContainKeyAsync(key + id))
                {
                    var commitsFromStorage = await _localStorageService.GetItemAsync<List<ArticleCommitMainInfoVM>>(key + id);
                    commits.AddRange(commitsFromStorage);
                }
                var article = await _localStorageService.GetItemAsync<ArticleVM>("workbanch_" + id);
                return new ArticleAggregatedVM { ArticleInfo = article, Commits = commits};
            }
            Guid courseId = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 });
            if ((await _courseService.GetCourse(courseId)).RootRecord.Find(r => r.Id == id) != null)
            {
                var article = await GetExample(id, commitId);
                //if (await _localStorageService.ContainKeyAsync(key + article.Id))
                //{
                //    var unsendCommits = await _localStorageService.GetItemAsync<List<ArticleCommitVM>>(key + article.Id);
                //    if (unsendCommits != null && unsendCommits.Count > 0)
                //        article.Commits.AddRange(unsendCommits);
                //}
                return article;
            }
            return null;
        }

        //public async Task AddArticleToLocalStorage(ArticleViewModel article)
        //{
        //    await _localStorageService.SetItemAsync(devKey + article.Id, ArticleConverter.ToStorage(article));
        //}

        //public async Task RemoveArticleFromLocalStorage(int id)
        //{
        //    await _localStorageService.RemoveItemAsync(devKey + id);
        //}

        //public async Task AddCommit(Guid articleId, ArticleCommitVM commit)
        //{
        //    if (await _localStorageService.ContainKeyAsync(key + articleId))
        //    {
        //        var unsendCommits = await _localStorageService.GetItemAsync<List<ArticleCommitVM>>(key + articleId);
        //        await _localStorageService.SetItemAsync(key + articleId, unsendCommits.Append(commit).ToList());
        //    }
        //    else
        //    {
        //        await _localStorageService.SetItemAsync(key + articleId, new List<ArticleCommitVM>() { commit });
        //    }
        //}

        public async Task AddCommit(Guid articleId, ArticleCommitMainInfoVM commit)
        {
            if (await _localStorageService.ContainKeyAsync(key + articleId))
            {
                var commits = await _localStorageService.GetItemAsync<List<ArticleCommitMainInfoVM>>(key + articleId);
                await _localStorageService.SetItemAsync(key + articleId, commits.Append(commit));
            }
            else
            {
                await _localStorageService.SetItemAsync(key + articleId, new List<ArticleCommitMainInfoVM>() { commit });
            }
        }

        private async Task<ArticleAggregatedVM> GetExample(Guid id, Guid? commitId)
        {
            var commit1 = new ArticleCommitMainInfoVM
            {
                Id = new Guid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Title = "Created",
                CreatedAt = new DateTime(2021, 05, 15, 22, 23, 22),
                CreatedBy = "artbiel",
                Description = "Article created",
                CommitState = CommitState.Commited,
                Type = ArticleCommitType.Addition,
                Content = ""
            };
            var commits = new List<ArticleCommitMainInfoVM>() { commit1 };
            if(await _localStorageService.ContainKeyAsync(key + id))
            {
                var commitsFromStorage = await _localStorageService.GetItemAsync<List<ArticleCommitMainInfoVM>>(key + id);
                commits.AddRange(commitsFromStorage);
            }
            var article = new ArticleVM()
            {
                Id = id,
                Title = "Язык C# и платформа .NET",
                Content = commits.Last().Content,
                CurrentCommitId = commitId ?? commits.Last().Id,
            };
            return new() { ArticleInfo = article, Commits = commits };
        }

        //private static class ArticleConverter
        //{
        //    public static ArticleVM FromStorage(ArticleVM articleViewModel)
        //    {
        //        if (articleViewModel?.Commits != null)
        //        {
        //            foreach (var commit in articleViewModel.Commits)
        //            {
        //                if (commit.PrevCommit != null)
        //                    commit.PrevCommit = articleViewModel.Commits.FirstOrDefault(c => c.Id == commit.PrevCommit.Id);
        //            }
        //        }
        //        return articleViewModel;
        //    }

        //    public static ArticleVM ToStorage(ArticleVM articleViewModel)
        //    {
        //        //if (articleViewModel?.Commits != null)
        //        //{
        //        //    foreach (var commit in articleViewModel.Commits)
        //        //    {
        //        //        if (commit.PrevCommit != null)
        //        //            commit.PrevCommit = new() { Id = commit.PrevCommit.Id };
        //        //    }
        //        //}
        //        return articleViewModel;
        //    }
        //}
    }

}
