using Client.Models;
using Client.Services;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Store
{
    public class CourseState
    {
        public bool IsLoading { get; }
        public CourseFullInfoViewModel CurrentCourse { get; }
        public List<RecordMainInfoViewModel> ActiveRecords { get; }

        public CourseState(bool isLoading, CourseFullInfoViewModel currentCourse, List<RecordMainInfoViewModel> activeRecords)
        {
            IsLoading = isLoading;
            CurrentCourse = currentCourse ?? new();
            ActiveRecords = activeRecords ?? new();
        }
    }

    public class CourseFeature : Feature<CourseState>
    {
        public override string GetName() => "Course";

        protected override CourseState GetInitialState() => new CourseState(false, null, null);
    }

    public static class CourseReducers
    {
        [ReducerMethod]
        public static CourseState ReduceFetchCourseAction(CourseState state, FetchCourseAction action) =>
            new CourseState(true, state.CurrentCourse, state.ActiveRecords);

        [ReducerMethod]
        public static CourseState ReduceSetCourseAction(CourseState state, SetCourseAction action)
        {
            if (action.DevMode && action.Course != null)
            {
                foreach (var record in action.Course.Records)
                {
                    if (record.Children != null)
                    {
                        var list = new List<RecordMainInfoViewModel>(action.Course.Records.Count);
                        foreach (var child in record.Children)
                        {
                            list.Add(action.Course.Records.Find(r => r.Id == child.Id));
                        }
                        record.Children = list;
                    }
                }
            }
            return new CourseState(false, action.Course, null);
        }

        [ReducerMethod]
        public static CourseState ReduceSetActiveRecordsAction(CourseState state, SetActiveRecordsAction action)
        {
            if (state.CurrentCourse?.Records != null)
            {
                return new CourseState(state.IsLoading, state.CurrentCourse, GetActiveRecords(state.CurrentCourse.Records, action.ArticleId));
            }
            return new CourseState(state.IsLoading, state.CurrentCourse, state.ActiveRecords);
        }

        [ReducerMethod]
        public static CourseState ReduceAddCourseRecordAction(CourseState state, AddCourseRecordAction action)
        {
            if (state.CurrentCourse.Records.Contains(action.ParentRecord))
            {
                if (action.ParentRecord.Children == null)
                    action.ParentRecord.Children = new();
                if (action.Record.Type == RecordType.Article)
                    action.Record.TargetId = new Random().Next();
                else if (action.Record.Children == null)
                    action.Record.Children = new();
                action.ParentRecord.Children = new(action.ParentRecord.Children.Prepend(action.Record));
                state.CurrentCourse.Records.Add(action.Record);
            }
            return new CourseState(state.IsLoading, state.CurrentCourse, state.ActiveRecords);
        }

        [ReducerMethod]
        public static CourseState ReduceEditCourseRecordAction(CourseState state, EditCourseRecordAction action)
        {
            var record = state.CurrentCourse.Records.Find(r => r.Id == action.Record.Id);
            if (record != null)
            {
                record = action.Record;
            }
            return new CourseState(state.IsLoading, state.CurrentCourse, state.ActiveRecords);
        }

        [ReducerMethod]
        public static CourseState ReduceRemoveCourseRecordAction(CourseState state, RemoveCourseRecordAction action)
        {
            state.CurrentCourse.Records.Remove(action.Record);
            state.CurrentCourse.Records.FirstOrDefault(r => r.Children != null && r.Children.Contains(action.Record))?.Children.Remove(action.Record);
            return new CourseState(state.IsLoading, state.CurrentCourse, state.ActiveRecords);
        }

        [ReducerMethod]
        public static CourseState ReduceMoveCourseRecordAction(CourseState state, MoveCourseRecordAction action)
        {
            if (state.CurrentCourse.Records.Contains(action.Record))
            {
                if (state.CurrentCourse.Records.Contains(action.PreviousElement))
                {
                    var destParent = state.CurrentCourse.Records.Contains(action.DestinationParent) ? action.DestinationParent :
                        state.CurrentCourse.Records.Find(r => r.Children != null && r.Children.Contains(action.PreviousElement));
                    state.CurrentCourse.Records.Find(r => r.Children != null && r.Children.Contains(action.Record)).Children.Remove(action.Record);
                    destParent.Children.Insert(destParent.Children.IndexOf(action.PreviousElement) + 1, action.Record);
                }
                else
                {
                    if (state.CurrentCourse.Records.Contains(action.DestinationParent) && action.Record != action.DestinationParent)
                    {
                        state.CurrentCourse.Records.Find(r => r.Children != null && r.Children.Contains(action.Record)).Children.Remove(action.Record);
                        action.DestinationParent.Children = action.DestinationParent.Children.Prepend(action.Record).ToList();
                    }
                }
            }
            return new CourseState(state.IsLoading, state.CurrentCourse, state.ActiveRecords);
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
                active = records.FirstOrDefault(r => r.Children != null && r.Children.Contains(active));
            }
            return activeList.ToList();
        }

    }

    public class FetchCourseAction
    {
        public int Id { get; }
        public bool DevMode { get; set; }

        public FetchCourseAction(int id, bool devMode)
        {
            Id = id;
            DevMode = devMode;
        }
    }

    public class SetCourseAction
    {
        public CourseFullInfoViewModel Course { get; }
        public bool DevMode { get; set; }

        public SetCourseAction(CourseFullInfoViewModel course, bool devMode)
        {
            Course = course;
            DevMode = devMode;
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
            var course = await _courseService.GetCourse(action.Id, action.DevMode);
            dispatcher.Dispatch(new SetCourseAction(course, action.DevMode));
        }
    }

    public class SetActiveRecordsAction
    {
        public int ArticleId { get; }

        public SetActiveRecordsAction(int articleId)
        {
            ArticleId = articleId;
        }
    }

    public class AddCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }
        public RecordMainInfoViewModel ParentRecord { get; }

        public AddCourseRecordAction(RecordMainInfoViewModel record, RecordMainInfoViewModel parentRecord)
        {
            Record = record;
            ParentRecord = parentRecord;
        }
    }



    public class AddCourseRecordActionEffect : Effect<AddCourseRecordAction>
    {
        private IState<CourseState> _state;
        private ArticleService _articleService;
        private CourseService _courseService;

        public AddCourseRecordActionEffect(IState<CourseState> state, ArticleService articleService, CourseService courseService)
        {
            _state = state;
            _articleService = articleService;
            _courseService = courseService;
        }

        public override async Task HandleAsync(AddCourseRecordAction action, IDispatcher dispatcher)
        {
            if (_state.Value.CurrentCourse.Records.Contains(action.Record))
            {
                await _courseService.SaveCourseToLocalStorage(_state.Value.CurrentCourse);
                if (action.Record.Type == RecordType.Article)
                    dispatcher.Dispatch(new AddArticleAction(action.Record.TargetId, action.Record.Name));
            }
        }
    }

    public class EditCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }

        public EditCourseRecordAction(RecordMainInfoViewModel record)
        {
            Record = record;
        }
    }

    public class EditCourseRecordActionEffect : Effect<EditCourseRecordAction>
    {
        private IState<CourseState> _state;
        private CourseService _courseService;

        public EditCourseRecordActionEffect(IState<CourseState> state, CourseService courseService)
        {
            _state = state;
            _courseService = courseService;
        }

        public override async Task HandleAsync(EditCourseRecordAction action, IDispatcher dispatcher)
        {
            if (_state.Value.CurrentCourse.Records.Contains(action.Record))
            {
                await _courseService.SaveCourseToLocalStorage(_state.Value.CurrentCourse);
            }
        }
    }

    public class RemoveCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }

        public RemoveCourseRecordAction(RecordMainInfoViewModel record)
        {
            Record = record;
        }
    }

    public class RemoveCourseRecordActionEffect : Effect<RemoveCourseRecordAction>
    {
        private IState<CourseState> _state;
        private CourseService _courseService;
        private ArticleService _articleService;

        public RemoveCourseRecordActionEffect(IState<CourseState> state, CourseService courseService, ArticleService articleService)
        {
            _state = state;
            _courseService = courseService;
            _articleService = articleService;
        }

        public override async Task HandleAsync(RemoveCourseRecordAction action, IDispatcher dispatcher)
        {
            if (!_state.Value.CurrentCourse.Records.Contains(action.Record))
            {
                await _courseService.SaveCourseToLocalStorage(_state.Value.CurrentCourse);
                if (action.Record.Type == RecordType.Article)
                    dispatcher.Dispatch(new RemoveArticleAction(action.Record.Id));
            }
        }
    }

    public class MoveCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }
        public RecordMainInfoViewModel PreviousElement { get; }
        public RecordMainInfoViewModel DestinationParent { get; }

        public MoveCourseRecordAction(RecordMainInfoViewModel record, RecordMainInfoViewModel destinationParent,
            RecordMainInfoViewModel previousElement)
        {
            Record = record;
            DestinationParent = destinationParent;
            PreviousElement = previousElement;
        }
    }

    public class MoveCourseRecordActionEffect : Effect<MoveCourseRecordAction>
    {
        private IState<CourseState> _state;
        private CourseService _courseService;

        public MoveCourseRecordActionEffect(IState<CourseState> state, CourseService courseService)
        {
            _state = state;
            _courseService = courseService;
        }

        public override async Task HandleAsync(MoveCourseRecordAction action, IDispatcher dispatcher)
        {
            if (_state.Value.CurrentCourse.Records.Contains(action.Record))
            {
                await _courseService.SaveCourseToLocalStorage(_state.Value.CurrentCourse);
            }
        }
    }
}
