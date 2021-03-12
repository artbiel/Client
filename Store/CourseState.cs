using Client.Models;
using Fluxor;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Store
{
    public class CourseState
    {
        public CourseFullInfoViewModel CurrentCourse { get; }
        public bool IsLoading { get; }

        public CourseState(bool isLoading, CourseFullInfoViewModel currentCourse)
        {
            IsLoading = isLoading;
            CurrentCourse = currentCourse ?? new();
        }
    }

    public class CourseFeature : Feature<CourseState>
    {
        public override string GetName() => "Course";

        protected override CourseState GetInitialState() => new CourseState(false, null);
    }

    public static class CourseReducers
    {
        [ReducerMethod]
        public static CourseState ReduceFetchCourseAction(CourseState state, FetchCourseAction action) => 
            new CourseState(true, null);

        [ReducerMethod]
        public static CourseState ReduceFetchCourseResultAction(CourseState state, SetCourseAction action) =>
            new CourseState(false, action.Course);
    }

    public class FetchCourseAction
    {
        public int Id { get; }

        public FetchCourseAction(int id)
        {
            Id = id;
        }
    }

    public class SetCourseAction
    {
        public CourseFullInfoViewModel Course { get; }

        public SetCourseAction(CourseFullInfoViewModel course)
        {
            Course = course;
        }
    }

    public class FecthCourseActionEffect : Effect<FetchCourseAction>
    {
        public override Task HandleAsync(FetchCourseAction action, IDispatcher dispatcher)
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
            subchapter1_2.Children = new() { article1_2_2, article1_2_2 };

            var course = new CourseFullInfoViewModel()
            {
                Id = 1,
                Name = "Основы C#",
                Description = "Курс разработан для студентов первого года обучения компьютерных специальностей УрФУ." +
                        " Рассчитан на людей с минимальным опытом программирования и знакомит с основами синтаксиса C#" +
                        " и стандартными классами .NET, с основами ООП и базовыми алгоритмами.",
                ImgSrc = "https://www.secretorange.co.uk/media/TagImage/c-sharp.png",
                Difficulty = Difficulty.Intermediate,
                Records = new() { subchapter1_1, article1_2_1, chapter1, subchapter1_2, article1_1_1, article1_1_2, article1_2_2 }
            };

            dispatcher.Dispatch(new SetCourseAction(course));
            return Task.CompletedTask;
        }

        public class SetCourseActionEffect : Effect<SetCourseAction>
        {
            public override Task HandleAsync(SetCourseAction action, IDispatcher dispatcher)
            {
                dispatcher.Dispatch(new SetDevCourseAction(action.Course));
                return Task.CompletedTask;
            }
        }
    }



}
