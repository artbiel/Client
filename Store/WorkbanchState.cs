using Client.Models;
using Client.Services;
using Fluxor;
using Material.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Store
{
    public record WorkbanchState(bool IsLoading, List<WorkbanchItemVM> Items, string SearchString, WorkbanchItemType? SearchType);

    public class WorkbanchFeature : Feature<WorkbanchState>
    {
        public override string GetName() => "Workbanch";

        protected override WorkbanchState GetInitialState() => new(false, new(), "", default);
    }

    public static class WorkbanchReducers
    {
        [ReducerMethod]
        public static WorkbanchState ReduceSetWorkbanchSearchStringAction(WorkbanchState state, SetWorkbanchSearchStringAction action) =>
            new WorkbanchState(true, state.Items, action.SearchString, state.SearchType);

        [ReducerMethod]
        public static WorkbanchState ReduceSetWorkbanchSearchTypeAction(WorkbanchState state, SetWorkbanchSearchTypeAction action) =>
            new WorkbanchState(true, state.Items, state.SearchString, action.SearchType);

        [ReducerMethod]
        public static WorkbanchState ReduceFetchWorkbanchItemsAction(WorkbanchState state, FetchWorkbanchItemsAction action) =>
            new WorkbanchState(true, state.Items, state.SearchString, state.SearchType);

        [ReducerMethod]
        public static WorkbanchState ReduceFetchWorkbanchItemsResultAction(WorkbanchState state, FetchWorkbanchItemsResultAction action) =>
            new WorkbanchState(false, action.Items, state.SearchString, state.SearchType);
    }

    public record SetWorkbanchSearchStringAction(string SearchString);

    public class SetWorkbanchSearchStringActionEffect : Effect<SetWorkbanchSearchStringAction>
    {
        private readonly IState<WorkbanchState> _state;

        public SetWorkbanchSearchStringActionEffect(IState<WorkbanchState> state)
        {
            _state = state;
        }

        public override Task HandleAsync(SetWorkbanchSearchStringAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new FetchWorkbanchItemsAction(action.SearchString/*, _state.Value.SearchType*/));
            return Task.CompletedTask;
        }
    }

    public record SetWorkbanchSearchTypeAction(WorkbanchItemType? SearchType);

    public class SetWorkbanchSearchTypeActionEffect : Effect<SetWorkbanchSearchTypeAction>
    {
        private readonly IState<WorkbanchState> _state;

        public SetWorkbanchSearchTypeActionEffect(IState<WorkbanchState> state)
        {
            _state = state;
        }

        public override Task HandleAsync(SetWorkbanchSearchTypeAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new FetchWorkbanchItemsAction(_state.Value.SearchString/*, action.SearchType*/));
            return Task.CompletedTask;
        }
    }

    public record FetchWorkbanchItemsAction(string SearchString/*, WorkbanchItemType? Type*/);

    public class FetchWorkbanchItemsActionEffect : Effect<FetchWorkbanchItemsAction>
    {
        private readonly WorkbanchService _workbanchService;

        public FetchWorkbanchItemsActionEffect(WorkbanchService workbanchService)
        {
            _workbanchService = workbanchService;
        }

        public override async Task HandleAsync(FetchWorkbanchItemsAction action, IDispatcher dispatcher)
        {
            var items = await _workbanchService.GetAllItems(action.SearchString/*, action.Type*/);
            dispatcher.Dispatch(new FetchWorkbanchItemsResultAction(items));
        }
    }

    public record FetchWorkbanchItemsResultAction(List<WorkbanchItemVM> Items);

    public record CreateNewWorkbanchItemAction(string Title, WorkbanchItemType Type, Guid? Id = null);

    public class CreateNewWorkbanchItemActionEffect : Effect<CreateNewWorkbanchItemAction>
    {
        private readonly WorkbanchService _workbanchService;
        private readonly IState<WorkbanchState> _workbanchState;
        private readonly IMBToastService _toastService;

        public CreateNewWorkbanchItemActionEffect(WorkbanchService workbanchService, IState<WorkbanchState> workbanchState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchState = workbanchState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(CreateNewWorkbanchItemAction action, IDispatcher dispatcher)
        {
            var id = action.Id ?? Guid.NewGuid();
            var newWBItem = new WorkbanchItemVM
            {
                Id = id,
                Title = action.Title,
                Type = action.Type,
            };
            switch (newWBItem.Type)
            {
                case WorkbanchItemType.Article:
                    var newArticle = new WorkbanchArticleVM
                    {
                        Id = id,
                        Title = action.Title,
                        Content = "",
                        CourseId = null
                    };
                    await _workbanchService.CreateNewItem(newWBItem, newArticle);
                    break;
                case WorkbanchItemType.Course:
                    var newCourse = new CourseFullInfoVM
                    {
                        Id = id,
                        Title = action.Title,
                        RootRecord = new RecordVM()
                        {
                            Id = Guid.NewGuid(),
                            Title = "Root",
                            Type = RecordType.Root,
                            Children = new()
                        }
                    };
                    await _workbanchService.CreateNewItem(newWBItem, newCourse);
                    break;
            }
            _toastService.ShowToast(MBToastLevel.Success, $"Item was succesfully created!");
            dispatcher.Dispatch(new FetchWorkbanchItemsAction(_workbanchState.Value.SearchString/* _workbanchState.Value.SearchType*/));
        }
    }

    public record DeleteWorkBanchItemAction(Guid Id);

    public class DeleteWorkBanchItemActionEffect : Effect<DeleteWorkBanchItemAction>
    {
        private readonly WorkbanchService _workbanchService;
        private readonly IState<WorkbanchState> _workbanchState;
        private readonly IMBToastService _toastService;

        public DeleteWorkBanchItemActionEffect(WorkbanchService workbanchService, IState<WorkbanchState> workbanchState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchState = workbanchState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(DeleteWorkBanchItemAction action, IDispatcher dispatcher)
        {
            await _workbanchService.DeleteItem(action.Id);
            _toastService.ShowToast(MBToastLevel.Success, $"Item was succesfully deleted!");
            Console.WriteLine(_workbanchState.Value.SearchString);
            dispatcher.Dispatch(new FetchWorkbanchItemsAction(_workbanchState.Value.SearchString/*, _workbanchState.Value.SearchType*/));
        }
    }


    public record ForkItemToWorkbanchAction(BaseWBItem Item, WorkbanchItemType Type);

    public class ForkItemToWorkbanchActionEffect : Effect<ForkItemToWorkbanchAction>
    {
        private readonly WorkbanchService _workbanchService;

        public ForkItemToWorkbanchActionEffect(WorkbanchService workbanchService)
        {
            _workbanchService = workbanchService;
        }

        public override async Task HandleAsync(ForkItemToWorkbanchAction action, IDispatcher dispatcher)
        {
            if (!await _workbanchService.ContainsItem(action.Item.Id))
            {
                var wbitem = new WorkbanchItemVM
                {
                    Id = action.Item.Id,
                    Title = action.Item.Title,
                    Type = action.Type
                };
                switch (action.Type)
                {
                    case WorkbanchItemType.Article:
                        var article = action.Item as WorkbanchArticleVM;
                        Console.WriteLine(article.Content);
                        await _workbanchService.CreateNewItem(wbitem, article);
                        dispatcher.Dispatch(new FetchWorkbanchArticleResultAction(article));
                        break;
                    case WorkbanchItemType.Course:
                        var course = action.Item as CourseFullInfoVM;
                        await _workbanchService.CreateNewItem(wbitem, course);
                        dispatcher.Dispatch(new FetchWorkbanchCourseAction(course.Id));
                        break;
                }
            }
        }
    }
}

