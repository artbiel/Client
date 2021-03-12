using Fluxor;

namespace Client.Store
{
    public class LeftSidebarState
    {
        public bool IsDisplay { get; }

        public LeftSidebarState(bool isDisplay)
        {
            IsDisplay = isDisplay;
        }
    }

    public class LeftSidebarFeature : Feature<LeftSidebarState>
    {
        public override string GetName() => "LeftSidebar";

        protected override LeftSidebarState GetInitialState() => new(false);
    }

    public static class LeftSidebarReducers
    {
        [ReducerMethod]
        public static LeftSidebarState ReduceToggleLeftSidebarAction(LeftSidebarState state, ToggleLeftSidebarAction action) =>
            new LeftSidebarState(!state.IsDisplay);
    }

    public class ToggleLeftSidebarAction { }
}