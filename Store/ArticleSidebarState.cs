using Fluxor;

namespace Client.Store
{
    public class ArticleSidebarState
    {
        public ArticleSidebarTab CurrentTab { get; }

        public ArticleSidebarState(ArticleSidebarTab tab)
        {
            CurrentTab = tab;
        }
    }

    public class ArticleSidebarFeature : Feature<ArticleSidebarState>
    {
        public override string GetName() => "ArticleSidebar";

        protected override ArticleSidebarState GetInitialState() => new(ArticleSidebarTab.CourseStructure);
    }

    public static class ArticleSidebarReducers
    {
        [ReducerMethod]
        public static ArticleSidebarState ReduceToggleArticleSidebarTabAction(ArticleSidebarState state,
            ToggleArticleSidebarTabAction action) => new ArticleSidebarState(action.Tab);
    }

    public class ToggleArticleSidebarTabAction
    {
        public ArticleSidebarTab Tab { get; }

        public ToggleArticleSidebarTabAction(ArticleSidebarTab tab)
        {
            Tab = tab;
        }
    }

    public enum ArticleSidebarTab
    {
        CourseStructure,
        HistoryBranch
    }
}