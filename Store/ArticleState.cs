using System;
using Client.Models;
using Fluxor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffPlex.Model;
using Client.Store;

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
                active = active.Parent;
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
    public int Id { get; set; }

    public FetchArticleAction(int id)
    {
        Id = id;
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
    public override Task HandleAsync(FetchArticleAction action, IDispatcher dispatcher)
    {
        var chapter1 = new RecordMainInfoViewModel
        {
            Id = 1,
            Type = RecordType.Group,
            Name = "Глава 1"
        };
        var subchapter1_1 = new RecordMainInfoViewModel
        {
            Id = 2,
            Type = RecordType.Group,
            Name = "Подглава 1",
            Parent = chapter1
        };
        var article1_1_1 = new RecordMainInfoViewModel
        {
            Id = 3,
            Type = RecordType.Article,
            TargetId = 1,
            Name = "Статья 1",
            Parent = subchapter1_1
        };
        var article1_1_2 = new RecordMainInfoViewModel
        {
            Id = 4,
            Type = RecordType.Article,
            TargetId = 2,
            Name = "Статья 2",
            Parent = subchapter1_1
        };
        var subchapter1_2 = new RecordMainInfoViewModel
        {
            Id = 5,
            Type = RecordType.Group,
            Name = "Подглава 2",
            Parent = chapter1
        };
        var article1_2_1 = new RecordMainInfoViewModel
        {
            Id = 6,
            Type = RecordType.Article,
            TargetId = 3,
            Name = "Статья 1",
            Parent = subchapter1_2
        };
        var article1_2_2 = new RecordMainInfoViewModel
        {
            Id = 7,
            Type = RecordType.Article,
            TargetId = 4,
            Name = "Статья 2",
            Parent = subchapter1_2
        };
        chapter1.Children = new() { subchapter1_1, subchapter1_2 };
        subchapter1_1.Children = new() { article1_1_1, article1_1_2 };
        subchapter1_2.Children = new() { article1_2_1, article1_2_2 };

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
        var article = new ArticleAggregatedViewModel()
        {
            ArticleInfo = new()
                {
                Id = action.Id,
                Title = "Hello World",
                Commits = new List<CommitViewModel>() { commit }
            },
            CourseInfo = new()
            {
                Id = 1,
                Name = "Основы C#",
                Description = "Курс разработан для студентов первого года обучения компьютерных специальностей УрФУ." +
                    " Рассчитан на людей с минимальным опытом программирования и знакомит с основами синтаксиса C#" +
                    " и стандартными классами .NET, с основами ООП и базовыми алгоритмами.",
                ImgSrc = "https://www.secretorange.co.uk/media/TagImage/c-sharp.png",
                Difficulty = Difficulty.Intermediate,
                Records = new() { subchapter1_1, article1_2_1, chapter1, subchapter1_2, article1_1_1, article1_1_2, article1_2_2 }
            },
        };
        dispatcher.Dispatch(new SetCourseAction(article.CourseInfo));
        dispatcher.Dispatch(new FetchArticleResultAction(article));
        return Task.CompletedTask;
    }


}

