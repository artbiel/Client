using AutoMapper;
using Client.Models;
using Client.Services;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Store
{
    public record SupportState(string SearchString, SupportApplicationType? SearchType,
            bool IsPageLoading, List<SupportApplicationMainInfoVM> Applications,
            bool IsCurrentLoading, SupportApplicationFullInfoVM CurrentApplication);


    public class SupportFeature : Feature<SupportState>
    {
        public override string GetName() => "Support";

        protected override SupportState GetInitialState() => new("", default, false, new(), false, default);
    }

    public static class SupportReducers
    {
        [ReducerMethod]
        public static SupportState ReduceSetSupportSearchTypeAction(SupportState state, SetSupportSearchTypeAction action) =>
            new(state.SearchString, action.SearchType, state.IsPageLoading, state.Applications, state.IsCurrentLoading, state.CurrentApplication);

        [ReducerMethod]
        public static SupportState ReduceSetSupportSearchStringAction(SupportState state, SetSupportSearchStringAction action) =>
            new(action.SearchString, state.SearchType, state.IsPageLoading, state.Applications, state.IsCurrentLoading, state.CurrentApplication);

        [ReducerMethod]
        public static SupportState ReduceFetchAllApplicationsAction(SupportState state,
            FetchAllApplicationsAction action) =>
            new(state.SearchString, state.SearchType, true, state.Applications, state.IsCurrentLoading, state.CurrentApplication);


        [ReducerMethod]
        public static SupportState ReduceFetchAllApplicationsResultAction(SupportState state,
            FetchAllApplicationsResultAction action) =>
            new(state.SearchString, state.SearchType, false, action.Applications, state.IsCurrentLoading, state.CurrentApplication);

        [ReducerMethod]
        public static SupportState ReduceFetchApplicationAction(SupportState state,
           FetchApplicationAction action) =>
           new(state.SearchString, state.SearchType, state.IsPageLoading, state.Applications, true, state.CurrentApplication);


        [ReducerMethod]
        public static SupportState ReduceFetchApplicationResultAction(SupportState state,
            FetchApplicationResultAction action) =>
            new(state.SearchString, state.SearchType, state.IsPageLoading, state.Applications, false, action.Application);
    }

    public record SetSupportSearchTypeAction(SupportApplicationType? SearchType);

    public class SetSupportSearchTypeActionEffect : Effect<SetSupportSearchTypeAction>
    {
        private readonly IState<SupportState> _state;

        public SetSupportSearchTypeActionEffect(IState<SupportState> state)
        {
            _state = state;
        }

        public override Task HandleAsync(SetSupportSearchTypeAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new FetchAllApplicationsAction(_state.Value.SearchString, action.SearchType));
            return Task.CompletedTask;
        }
    }

    public record SetSupportSearchStringAction(string SearchString);

    public class SetSupportSearchStringActionEffect : Effect<SetSupportSearchStringAction>
    {
        private readonly IState<SupportState> _state;

        public SetSupportSearchStringActionEffect(IState<SupportState> state)
        {
            _state = state;
        }

        public override Task HandleAsync(SetSupportSearchStringAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new FetchAllApplicationsAction(action.SearchString, _state.Value.SearchType));
            return Task.CompletedTask;
        }
    }

    public record FetchAllApplicationsAction(string SearchString, SupportApplicationType? SearchType);

    public class FetchAllApplicationsActionEffect : Effect<FetchAllApplicationsAction>
    {
        private readonly SupportService _supportService;

        public FetchAllApplicationsActionEffect(SupportService supportService) { _supportService = supportService; }

        public override async Task HandleAsync(FetchAllApplicationsAction action, IDispatcher dispatcher)
        {
            var allApplications = await _supportService.GetAllItems(action.SearchString, action.SearchType);
            dispatcher.Dispatch(new FetchAllApplicationsResultAction(allApplications));
        }
    }

    public record FetchAllApplicationsResultAction(List<SupportApplicationMainInfoVM> Applications);

    public record CreateNewSupportApplicationAction(NewSupportApplicationVM Application);

    public class CreateNewSupportApplicationActionEffect : Effect<CreateNewSupportApplicationAction>
    {
        private readonly SupportService _supportService;
        private readonly IState<SupportState> _supportState;

        public CreateNewSupportApplicationActionEffect(SupportService supportService, IState<SupportState> supportState)
        {
            _supportService = supportService;
            _supportState = supportState;
        }

        public override async Task HandleAsync(CreateNewSupportApplicationAction action, IDispatcher dispatcher)
        {
            var application = new SupportApplicationFullInfoVM
            {
                Id = Guid.NewGuid(),
                Title = action.Application.Title,
                Description = action.Application.Description,
                Type = action.Application.Type,
                Status = SupportApplicationStatus.Waiting,
                Comments = new()
            };
            await _supportService.CreateNewItem(application);
            dispatcher.Dispatch(new FetchAllApplicationsAction(_supportState.Value.SearchString, _supportState.Value.SearchType));
        }
    }

    public record FetchApplicationAction(Guid Id);

    public class FetchApplicationActionEffect : Effect<FetchApplicationAction>
    {
        private readonly SupportService _supportService;

        public FetchApplicationActionEffect(SupportService supportService) { _supportService = supportService; }

        public override async Task HandleAsync(FetchApplicationAction action, IDispatcher dispatcher)
        {
            var application = await _supportService.GetApplication(action.Id);
            dispatcher.Dispatch(new FetchApplicationResultAction(application));
        }
    }

    public record FetchApplicationResultAction(SupportApplicationFullInfoVM Application);

    public record AddApplicationCommentAction(string Content, Guid ApplicationId, Guid? ParentId);

    public class AddApplicationCommentActionEffect : Effect<AddApplicationCommentAction>
    {
        private readonly SupportService _supportService;
        private readonly IState<SupportState> _supportState;

        public AddApplicationCommentActionEffect(SupportService supportService, IState<SupportState> supportState)
        {
            _supportService = supportService;
            _supportState = supportState;
        }

        public override async Task HandleAsync(AddApplicationCommentAction action, IDispatcher dispatcher)
        {
            var comment = new CommentVM
            {
                Id = Guid.NewGuid(),
                Content = action.Content.Trim(),
                CreatedAt = DateTime.Now,
                Children = new(),
                Likes = 0,
                Dislikes = 0,
                UserName = "Admin"
            };
            await _supportService.AddNewComment(comment, action.ApplicationId, action.ParentId);
            dispatcher.Dispatch(new FetchApplicationAction(action.ApplicationId));
        }
    }
}
