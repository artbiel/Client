using Client.Infrastructure;
using Client.Models;
using Client.Services;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Store
{
    public record CourseState(bool IsLoading, CourseFullInfoVM CurrentCourse, List<CourseCommitMainInfoVM> Commits,
        List<RecordVM> ActiveRecords);

    public class CourseFeature : Feature<CourseState>
    {
        public override string GetName() => "Course";

        protected override CourseState GetInitialState() => new CourseState(false, null, null, null);
    }

    public static class CourseReducers
    {
        [ReducerMethod]
        public static CourseState ReduceFetchCourseAction(CourseState state, FetchCourseAction action) =>
            new CourseState(true, state.CurrentCourse, state.Commits, state.ActiveRecords);

        [ReducerMethod]
        public static CourseState ReduceSetCourseAction(CourseState state, SetCourseAction action)
        {
            return new CourseState(false, action.Course, state.Commits, null);
        }

        [ReducerMethod]
        public static CourseState ReduceFetchCourseHistoryResultAction(CourseState state, FetchCourseHistoryResultAction action)
        {
            return new CourseState(state.IsLoading, state.CurrentCourse, action.Commits, state.ActiveRecords);
        }

        [ReducerMethod]
        public static CourseState ReduceSetActiveRecordsAction(CourseState state, SetActiveRecordsAction action)
        {
            if (state.CurrentCourse?.RootRecord != null)
            {
                return new CourseState(state.IsLoading, state.CurrentCourse, state.Commits, GetActiveRecords(state.CurrentCourse.RootRecord, action.ArticleId));
            }
            return new CourseState(state.IsLoading, state.CurrentCourse, state.Commits, state.ActiveRecords);
        }

        private static List<RecordVM> GetActiveRecords(RecordVM rootRecord, Guid currentId)
        {
            if (rootRecord == null)
            {
                throw new ArgumentNullException(nameof(rootRecord));
            }
            IEnumerable<RecordVM> activeList = new List<RecordVM>();

            var active = rootRecord.Find(currentId);
            while (active != null)
            {
                activeList = activeList.Prepend(active);
                active = rootRecord.Find(r => r.Children != null && r.Children.Contains(active));
            }
            return activeList.ToList();
        }

    }

    public record FetchCourseAction(Guid Id);

    public record SetCourseAction(CourseFullInfoVM Course);

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
            dispatcher.Dispatch(new FetchCourseHistoryAction(action.Id));
        }
    }

    public record SetActiveRecordsAction(Guid ArticleId);

    public record EditCourseAction(CourseFullInfoVM Course);

    public class EditCourseActionEffect : Effect<EditCourseAction>
    {
        private readonly CourseService _courseService;

        public EditCourseActionEffect(CourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task HandleAsync(EditCourseAction action, IDispatcher dispatcher)
        {
            await _courseService.EditCourse(action.Course);
        }
    }

    public record FetchCourseHistoryAction(Guid Id);

    public record FetchCourseHistoryResultAction(List<CourseCommitMainInfoVM> Commits);

    public class FetchCourseHistoryActionEffect : Effect<FetchCourseHistoryAction>
    {
        private readonly CourseService _courseService;

        public FetchCourseHistoryActionEffect(CourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task HandleAsync(FetchCourseHistoryAction action, IDispatcher dispatcher)
        {
            var history = await _courseService.GetHistory(action.Id);
            dispatcher.Dispatch(new FetchCourseHistoryResultAction(history));
        }
    }

    //public record AddCourseRecordAction(RecordVM Record, RecordVM ParentRecord);
}
