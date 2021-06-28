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
    public record WorkbanchArticleState(bool IsLoading, WorkbanchArticleVM Article);

    public class WorkbanchArticleFeature : Feature<WorkbanchArticleState>
    {
        public override string GetName() => "WorkbanchArticle";

        protected override WorkbanchArticleState GetInitialState() => new(false, null);
    }

    public static class WorkbanchArticleReducers
    {
        [ReducerMethod]
        public static WorkbanchArticleState ReduceFetchWorkbanchArticleAction(WorkbanchArticleState state, FetchWorkbanchArticleAction action)
        {
            return new WorkbanchArticleState(true, state.Article);
        }

        [ReducerMethod]
        public static WorkbanchArticleState ReduceFetchWorkbanchArticleResultAction(WorkbanchArticleState state, FetchWorkbanchArticleResultAction action)
        {
            return new WorkbanchArticleState(false, action.Article);
        }
    }

    public record FetchWorkbanchArticleAction(Guid Id);

    public class FetchWorkbanchArticleActionEffect : Effect<FetchWorkbanchArticleAction>
    {
        private readonly WorkbanchService _workbanchService;

        public FetchWorkbanchArticleActionEffect(WorkbanchService workbanchService)
        {
            _workbanchService = workbanchService;
        }

        public override async Task HandleAsync(FetchWorkbanchArticleAction action, IDispatcher dispatcher)
        {
            var article = await _workbanchService.GetItem<WorkbanchArticleVM>(action.Id);
            dispatcher.Dispatch(new FetchWorkbanchArticleResultAction(article));
        }
    }

    public record FetchWorkbanchArticleResultAction(WorkbanchArticleVM Article);

    public record EditWorkbanchArticleContentAction(Guid Id, string Content);

    public class EditWorkbanchArticleContentActionEffect : Effect<EditWorkbanchArticleContentAction>
    {
        private readonly WorkbanchService _workbanchService;
        private readonly IState<WorkbanchArticleState> _workbanchArticleState;
        private readonly IMBToastService _toastService;

        public EditWorkbanchArticleContentActionEffect(WorkbanchService workbanchService, IState<WorkbanchArticleState> workbanchArticleState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchArticleState = workbanchArticleState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(EditWorkbanchArticleContentAction action, IDispatcher dispatcher)
        {
            var success = await _workbanchService.EditArticleContent(action.Id, action.Content);
            if (success)
            {
                _toastService.ShowToast(MBToastLevel.Success, "Article was successfully saved!");
                var currentArticle = _workbanchArticleState.Value.Article;
                currentArticle.Content = action.Content;
                dispatcher.Dispatch(new FetchWorkbanchArticleResultAction(currentArticle));
            }
            else
                _toastService.ShowToast(MBToastLevel.Error, "Something went wrong");
        }
    }

    public record EditWorkbanchArticleTitleAction(Guid Id, string Title, Guid? CourseId);

    public class EditWorkbanchArticleTitleActionEffect : Effect<EditWorkbanchArticleTitleAction>
    {
        private readonly WorkbanchService _workbanchService;
        private readonly IState<WorkbanchArticleState> _workbanchArticleState;
        private readonly IMBToastService _toastService;

        public EditWorkbanchArticleTitleActionEffect(WorkbanchService workbanchService, IState<WorkbanchArticleState> workbanchArticleState, IMBToastService toastService)
        {
            _workbanchService = workbanchService;
            _workbanchArticleState = workbanchArticleState;
            _toastService = toastService;
        }

        public override async Task HandleAsync(EditWorkbanchArticleTitleAction action, IDispatcher dispatcher)
        {
            var success = await _workbanchService.EditTitle<WorkbanchArticleVM>(action.Id, action.Title, action.CourseId);
            if (success)
            {
                _toastService.ShowToast(MBToastLevel.Success, "Title was successfully changed!");
                var currentArticle = _workbanchArticleState.Value.Article;
                currentArticle.Title = action.Title;
                dispatcher.Dispatch(new FetchWorkbanchArticleResultAction(currentArticle));
            }
            else
                _toastService.ShowToast(MBToastLevel.Error, "Something went wrong");
        }
    }

}
