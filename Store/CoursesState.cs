using Client.Models;
using Client.Models.Commands;
using Client.Services;
using Fluxor;
using Material.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Store
{
    public record CoursesState(bool IsLoading, List<CourseMainInfoVM> Courses, string SearchString, CourseSearchType SearchType);

    public class CoursesFeature : Feature<CoursesState>
    {
        public override string GetName() => "Courses";

        protected override CoursesState GetInitialState() => new(false, default, "", default);
    }

    public static class CoursesReducers
    {
        [ReducerMethod]
        public static CoursesState ReduceSetCoursesSearchStringAction(CoursesState state, SetCoursesSearchStringAction action) =>
            new CoursesState(state.IsLoading, state.Courses, action.SearchString, state.SearchType);

        [ReducerMethod]
        public static CoursesState ReduceSetCoursesSearchTypeAction(CoursesState state, SetCoursesSearchTypeAction action) =>
            new CoursesState(state.IsLoading, state.Courses, state.SearchString, action.SearchType);

        [ReducerMethod]
        public static CoursesState ReduceFetchCoursesItemsAction(CoursesState state, FetchCoursesItemsAction action) =>
            new CoursesState(true, state.Courses, state.SearchString, state.SearchType);

        [ReducerMethod]
        public static CoursesState ReduceFetchCoursesItemsResultAction(CoursesState state, FetchCoursesItemsResultAction action) =>
            new CoursesState(false, action.Courses, state.SearchString, state.SearchType);
    }

    public record SetCoursesSearchStringAction(string SearchString);

    public class SetCoursesSearchStringActionEffect : Effect<SetCoursesSearchStringAction>
    {
        private readonly IState<CoursesState> _state;

        public SetCoursesSearchStringActionEffect(IState<CoursesState> state)
        {
            _state = state;
        }

        public override Task HandleAsync(SetCoursesSearchStringAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new FetchCoursesItemsAction(action.SearchString, _state.Value.SearchType));
            return Task.CompletedTask;
        }
    }

    public record SetCoursesSearchTypeAction(CourseSearchType SearchType);

    public class SetCoursesSearchTypeActionEffect : Effect<SetCoursesSearchTypeAction>
    {
        private readonly IState<CoursesState> _state;

        public SetCoursesSearchTypeActionEffect(IState<CoursesState> state)
        {
            _state = state;
        }

        public override Task HandleAsync(SetCoursesSearchTypeAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new FetchCoursesItemsAction(_state.Value.SearchString, action.SearchType));
            return Task.CompletedTask;
        }
    }

    public record FetchCoursesItemsAction(string SearchString, CourseSearchType SearchType);

    public class FetchCoursesItemsActionEffect : Effect<FetchCoursesItemsAction>
    {
        private readonly CourseService _courseService;

        public FetchCoursesItemsActionEffect(CourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task HandleAsync(FetchCoursesItemsAction action, IDispatcher dispatcher)
        {
            var courses = await _courseService.GetAllCourses(action.SearchString, action.SearchType);
            dispatcher.Dispatch(new FetchCoursesItemsResultAction(courses));
        }
    }

    public record FetchCoursesItemsResultAction(List<CourseMainInfoVM> Courses);

    public record CreateCourseAction(CreateCourseCommand Course);

    public class CreateCourseActionEffect : Effect<CreateCourseAction>
    {
        private readonly CourseService _courseService;

        public CreateCourseActionEffect(CourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task HandleAsync(CreateCourseAction action, IDispatcher dispatcher)
        {
            await _courseService.CreateCourse(action.Course);
        }
    }

}