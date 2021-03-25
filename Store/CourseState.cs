using Client.Models;
using Client.Services;
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
        public static CourseState ReduceSetCourseAction(CourseState state, SetCourseAction action) =>
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
        private readonly CourseService _courseService;

        public FecthCourseActionEffect(CourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task HandleAsync(FetchCourseAction action, IDispatcher dispatcher)
        {
            var course = await _courseService.GetCourse(action.Id);
            dispatcher.Dispatch(new SetCourseAction(course));
        }
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
