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
        private const string key = "article_";
        private const string devKey = "article_dev_";

        private readonly ILocalStorageService _localStorageService;

        public ArticleService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<ArticleViewModel> GetArticle(int id)
        {
            var article = GetExample(id);
            return article;
        }

        private ArticleViewModel GetExample(int id)
        {
            var commit = new CommitViewModel
            {
                Id = 0,
                Title = "First Commit",
                CreatedAt = new DateTime(2010, 11, 2, 14, 23, 22),
                CreatedBy = "Тёмыч",
                Description = "Первый коммит Тёмыча",
                DiffWords = new string[] { "Эта", " ", "статья", " ", "про", " ", "Тёмыча" },
                DiffBlocks = new List<DiffBlock>() { new DiffBlock(0, 0, 0, 7) }
            };
            var article = new ArticleViewModel()
            {
                Id = id,
                Title = "Hello World",
                Commits = new List<CommitViewModel>() { commit }
            };
            return article;
        }
    }
}
