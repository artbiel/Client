using Blazored.LocalStorage;
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
        private const string devKey = "article_dev_";
        private const string tempContentKey = "article_dev_temp_content_";

        private ILocalStorageService _localStorageService;
        private CourseService _courseService;

        public ArticleService(ILocalStorageService localStorageService, CourseService courseService)
        {
            _localStorageService = localStorageService;
            _courseService = courseService;
        }

        public async Task<ArticleViewModel> GetArticle(int id, bool devMode)
        {
            if (devMode)
            {
                if (await _localStorageService.ContainKeyAsync(devKey + id))
                {
                    var article = await _localStorageService.GetItemAsync<ArticleViewModel>(devKey + id);
                    return ArticleConverter.FromStorage(article);
                }
                else
                {
                    if ((await _courseService.GetCourse(1, devMode)).Records.Any(r => r.TargetId == id))
                    {
                        var article = GetExample(id);
                        await AddArticleToLocalStorage(ArticleConverter.ToStorage(article));
                        return article;
                    }
                }
            }
            if ((await _courseService.GetCourse(1, devMode)).Records.Any(r => r.TargetId == id))
            {
                return GetExample(id);
            }
            return null;
        }

        public async Task AddArticleToLocalStorage(ArticleViewModel article)
        {
            await _localStorageService.SetItemAsync(devKey + article.Id, ArticleConverter.ToStorage(article));
        }

        public async Task RemoveArticleFromLocalStorage(int id)
        {
            await _localStorageService.RemoveItemAsync(devKey + id);
        }

        private ArticleViewModel GetExample(int id)
        {
            var commit = new ArticleCommitViewModel
            {
                Id = 0,
                Title = "First Commit",
                CreatedAt = new DateTime(2010, 11, 2, 14, 23, 22),
                CreatedBy = "Тёмыч",
                Description = "Первый коммит Тёмыча",
                DiffWords = new string[] { "Эта", " ", "статья", " ", "про", " ", "Тёмыча" },
                DiffBlocks = new List<ArticleDiffBlock>() { new(0, 0, 0, 7) },
                CommitState = CommitState.Commited,
                Type = ArticleCommitType.Initital
            };
            var article = new ArticleViewModel()
            {
                Id = id,
                Title = "Hello World",
                Commits = new List<ArticleCommitViewModel>() { commit }
            };
            return article;
        }

        private static class ArticleConverter
        {
            public static ArticleViewModel FromStorage(ArticleViewModel articleViewModel)
            {
                if (articleViewModel?.Commits != null)
                {
                    foreach (var commit in articleViewModel.Commits)
                    {
                        if (commit.PrevCommit != null)
                            commit.PrevCommit = articleViewModel.Commits.FirstOrDefault(c => c.Id == commit.PrevCommit.Id);
                    }
                }
                return articleViewModel;
            }

            public static ArticleViewModel ToStorage(ArticleViewModel articleViewModel)
            {
                if (articleViewModel?.Commits != null)
                {
                    foreach (var commit in articleViewModel.Commits)
                    {
                        if (commit.PrevCommit != null)
                            commit.PrevCommit = new() { Id = commit.PrevCommit.Id };
                    }
                }
                return articleViewModel;
            }
        }
    }

}
