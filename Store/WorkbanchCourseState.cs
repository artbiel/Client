using AutoMapper;
using Client.Infrastructure;
using Client.Models;
using Client.Services;
using Fluxor;
using Material.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Store
{
    public record WorkbanchCourseState(bool IsLoading, CourseFullInfoVM Course);

    public class WorkbanchCourseFeature : Feature<WorkbanchCourseState>
    {
        public override string GetName() => "WorkbanchCourse";

        protected override WorkbanchCourseState GetInitialState() => new WorkbanchCourseState(false, default);
    }

    public static class WorkbanchCourseReducers
    {
        [ReducerMethod]
        public static WorkbanchCourseState ReduceFetchWorkbanchCourseAction(WorkbanchCourseState state, FetchWorkbanchCourseAction action) =>
            new WorkbanchCourseState(true, state.Course);

        [ReducerMethod]
        public static WorkbanchCourseState ReduceSetWorkbanchCourseAction(WorkbanchCourseState state, SetWorkbanchCourseAction action)
        {
            return new WorkbanchCourseState(false, action.Course);
        }

        [ReducerMethod]
        public static WorkbanchCourseState ReduceAddWorkbanchCourseRecordAction(WorkbanchCourseState state, AddWorkbanchCourseRecordAction action)
        {
            if (state.Course.RootRecord.Contains(action.ParentRecord.Id))
            {
                if (action.ParentRecord.Children == null)
                    action.ParentRecord.Children = new();
                if (action.Record.Type == RecordType.Article)
                    action.Record.Id = Guid.NewGuid();
                else if (action.Record.Children == null)
                    action.Record.Children = new();
                action.ParentRecord.Children = new(action.ParentRecord.Children.Prepend(action.Record));
            }
            return new WorkbanchCourseState(state.IsLoading, state.Course);
        }

        [ReducerMethod]
        public static WorkbanchCourseState ReduceEditWorkbanchCourseRecordAction(WorkbanchCourseState state, EditWorkbanchCourseRecordAction action)
        {
            state.Course.RootRecord.Find(action.Record.Id).Title = action.Record.Title;
            return new WorkbanchCourseState(state.IsLoading, state.Course);
        }

        [ReducerMethod]
        public static WorkbanchCourseState ReduceRemoveWorkbanchCourseRecordAction(WorkbanchCourseState state, RemoveWorkbanchCourseRecordAction action)
        {
            var rec = state.Course.RootRecord.Find(action.Record.Id);
            if (rec != null)
            {
                var recParent = state.Course.RootRecord.Find(r => r.Children != null && r.Children.Contains(action.Record));
                if (recParent != null)
                    recParent.Children.Remove(action.Record);
            }
            return new WorkbanchCourseState(state.IsLoading, state.Course);
        }
        [ReducerMethod]
        public static WorkbanchCourseState ReduceMoveWorkbanchCourseRecordAction(WorkbanchCourseState state, MoveWorkbanchCourseRecordAction action)
        {
            if (state.Course.RootRecord.Contains(action.Record.Id))
            {
                if (action.PreviousElement != null && state.Course.RootRecord.Contains(action.PreviousElement.Id))
                {
                    var destParent = action.DestinationParent != null && state.Course.RootRecord.Contains(action.DestinationParent.Id) ? action.DestinationParent :
                        state.Course.RootRecord.Find(r => r.Children != null && r.Children.Contains(action.PreviousElement));
                    state.Course.RootRecord.Find(r => r.Children != null && r.Children.Contains(action.Record)).Children.Remove(action.Record);
                    destParent.Children.Insert(destParent.Children.IndexOf(action.PreviousElement) + 1, action.Record);
                }
                else
                {
                    if (action.DestinationParent != null && state.Course.RootRecord.Contains(action.DestinationParent.Id) && action.Record != action.DestinationParent)
                    {
                        state.Course.RootRecord.Find(r => r.Children != null && r.Children.Contains(action.Record)).Children.Remove(action.Record);
                        action.DestinationParent.Children = action.DestinationParent.Children.Prepend(action.Record).ToList();
                    }
                }
            }
            return new WorkbanchCourseState(state.IsLoading, state.Course);
        }
    }

    public record FetchWorkbanchCourseAction(Guid Id);

    public record SetWorkbanchCourseAction(CourseFullInfoVM Course);

    public class FecthWorkbanchCourseActionEffect : Effect<FetchWorkbanchCourseAction>
    {
        private readonly WorkbanchService _workbanchService;

        public FecthWorkbanchCourseActionEffect(WorkbanchService workbanchService)
        {
            _workbanchService = workbanchService;
        }

        public override async Task HandleAsync(FetchWorkbanchCourseAction action, IDispatcher dispatcher)
        {
            var WorkbanchCourse = await _workbanchService.GetItem<CourseFullInfoVM>(action.Id);
            dispatcher.Dispatch(new SetWorkbanchCourseAction(WorkbanchCourse));
        }
    }

    public record EditWorkbanchCourseInfoAction(CourseEditingModel Course);

    public class EditWorkbanchCourseInfoActionEffect : Effect<EditWorkbanchCourseInfoAction>
    {
        private readonly WorkbanchService _workbanchService;
        private readonly IState<WorkbanchCourseState> _workbanchCourseState;
        private readonly IMBToastService _toastService;
        private readonly IMapper _mapper;

        public EditWorkbanchCourseInfoActionEffect(WorkbanchService workbanchService,
            IState<WorkbanchCourseState> workbanchCourseState, IMapper mapper, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchCourseState = workbanchCourseState;
            _toastService = toastService;
            _mapper = mapper;
        }

        public override async Task HandleAsync(EditWorkbanchCourseInfoAction action, IDispatcher dispatcher)
        {
            var mappedEditedCourse = _mapper.Map<CourseFullInfoVM>(action.Course);
            mappedEditedCourse.RootRecord = _workbanchCourseState.Value.Course.RootRecord;
            mappedEditedCourse.Rating = _workbanchCourseState.Value.Course.Rating;

            bool success = await _workbanchService.EditCourse(mappedEditedCourse);
            if (success)
            {
                _toastService.ShowToast(MBToastLevel.Success, "Course info was successfully changed!");
                dispatcher.Dispatch(new SetWorkbanchCourseAction(mappedEditedCourse));
            }
            else
                _toastService.ShowToast(MBToastLevel.Error, "Something went wrong");
        }
    }

    public record AddWorkbanchCourseRecordAction(RecordVM Record, RecordVM ParentRecord);

    public class AddWorkbanchCourseRecordActionEffect : Effect<AddWorkbanchCourseRecordAction>
    {
        private IState<WorkbanchCourseState> _workbanchCourseState;
        private WorkbanchService _workbanchService;
        private readonly IMBToastService _toastService;

        public AddWorkbanchCourseRecordActionEffect(WorkbanchService workbanchService,
            IState<WorkbanchCourseState> workbanchCourseState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchCourseState = workbanchCourseState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(AddWorkbanchCourseRecordAction action, IDispatcher dispatcher)
        {
            var rootRecord = _workbanchCourseState.Value.Course.RootRecord;
            if (rootRecord.Contains(action.Record.Id))
            {
                await _workbanchService.EditCourse(_workbanchCourseState.Value.Course);
                if (action.Record.Type == RecordType.Article)
                {
                    dispatcher.Dispatch(new CreateNewWorkbanchItemAction(action.Record.Title, WorkbanchItemType.Article, action.Record.Id));
                }
            }
        }
    }

    public record EditWorkbanchCourseRecordAction(RecordVM Record);

    public class EditWorkbanchCourseRecordActionEffect : Effect<EditWorkbanchCourseRecordAction>
    {
        private IState<WorkbanchCourseState> _workbanchCourseState;
        private WorkbanchService _workbanchService;
        private readonly IMBToastService _toastService;

        public EditWorkbanchCourseRecordActionEffect(WorkbanchService workbanchService,
            IState<WorkbanchCourseState> workbanchCourseState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchCourseState = workbanchCourseState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(EditWorkbanchCourseRecordAction action, IDispatcher dispatcher)
        {
            if (_workbanchCourseState.Value.Course.RootRecord.Contains(action.Record.Id))
            {
                await _workbanchService.EditCourse(_workbanchCourseState.Value.Course);
                _toastService.ShowToast(MBToastLevel.Success, "Item was successfully changed!");
            }
        }
    }

    public record RemoveWorkbanchCourseRecordAction(RecordVM Record);

    public class RemoveWorkbanchCourseRecordActionEffect : Effect<RemoveWorkbanchCourseRecordAction>
    {
        private IState<WorkbanchCourseState> _workbanchCourseState;
        private WorkbanchService _workbanchService;
        private readonly IMBToastService _toastService;

        public RemoveWorkbanchCourseRecordActionEffect(WorkbanchService workbanchService,
            IState<WorkbanchCourseState> workbanchCourseState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchCourseState = workbanchCourseState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(RemoveWorkbanchCourseRecordAction action, IDispatcher dispatcher)
        {
            if (!_workbanchCourseState.Value.Course.RootRecord.Contains(action.Record.Id))
            {
                await _workbanchService.EditCourse(_workbanchCourseState.Value.Course);
                if (action.Record.Type == RecordType.Article)
                    dispatcher.Dispatch(new DeleteWorkBanchItemAction(action.Record.Id));
            }
        }
    }

    public record MoveWorkbanchCourseRecordAction(RecordVM Record, RecordVM DestinationParent, RecordVM PreviousElement);

    public class MoveWorkbanchCourseRecordActionEffect : Effect<MoveWorkbanchCourseRecordAction>
    {
        private IState<WorkbanchCourseState> _workbanchCourseState;
        private WorkbanchService _workbanchService;
        private readonly IMBToastService _toastService;

        public MoveWorkbanchCourseRecordActionEffect(WorkbanchService workbanchService,
            IState<WorkbanchCourseState> workbanchCourseState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchCourseState = workbanchCourseState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(MoveWorkbanchCourseRecordAction action, IDispatcher dispatcher)
        {
            if (_workbanchCourseState.Value.Course.RootRecord.Contains(action.Record.Id))
            {
                if (action.PreviousElement != null && _workbanchCourseState.Value.Course.RootRecord.Contains(action.PreviousElement.Id))
                {
                    var destParent = action.DestinationParent != null && _workbanchCourseState.Value.Course.RootRecord.Contains(action.DestinationParent.Id) ? action.DestinationParent :
                        _workbanchCourseState.Value.Course.RootRecord.Find(r => r.Children != null && r.Children.Contains(action.PreviousElement));
                    if (destParent.Children.IndexOf(action.Record) == destParent.Children.IndexOf(action.PreviousElement) + 1)
                    {
                        await Success();
                        return;
                    }
                }
                else
                {
                    if (action.DestinationParent != null && _workbanchCourseState.Value.Course.RootRecord.Contains(action.DestinationParent.Id) && action.Record != action.DestinationParent)
                    {
                        var destParent = _workbanchCourseState.Value.Course.RootRecord.Find(r => r.Children != null && r.Children.Contains(action.Record));
                        if (destParent.Children.IndexOf(action.Record) == 0)
                        {
                            await Success();
                            return;
                        }
                    }
                }
            }
            _toastService.ShowToast(MBToastLevel.Error, "Someting went wrong");

            async Task Success()
            {
                await _workbanchService.EditCourse(_workbanchCourseState.Value.Course);
                _toastService.ShowToast(MBToastLevel.Success, "Item was successfully moved!");
            }
        }
    }
}
