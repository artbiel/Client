using Fluxor;

namespace Client.Store
{
    public class RightSidebarState
    {
        public bool Exists { get; }
        public bool IsDisplay { get; }

        public RightSidebarState(bool exists, bool isDisplay)
        {
            Exists = exists;
            IsDisplay = isDisplay;
        }
    }

    public class RightSidebarFeature : Feature<RightSidebarState>
    {
        public override string GetName() => "RightSidebar";

        protected override RightSidebarState GetInitialState() => new(false, false);
    }

    public static class RightSidebarReducers
    {
        [ReducerMethod]
        public static RightSidebarState ReduceToggleRightSidebarAction(RightSidebarState state, ToggleRightSidebarAction action) =>
            new RightSidebarState(state.Exists, !state.IsDisplay);

        [ReducerMethod]
        public static RightSidebarState ReduceRightSidebarExistsAction(RightSidebarState state, RightSidebarExistsAction action) =>
            new RightSidebarState(action.Exists, state.IsDisplay);
    }

    public class ToggleRightSidebarAction { }

    public class RightSidebarExistsAction 
    {
        public bool Exists { get; }

        public RightSidebarExistsAction(bool exists)
        {
            Exists = exists;
        }
    }
}

