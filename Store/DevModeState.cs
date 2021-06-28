//using Fluxor;

//namespace Client.Store
//{
//    public class DevModeState
//    {
//        public bool Enabled { get; }

//        public DevModeState(bool enabled)
//        {
//            Enabled = enabled;
//        }
//    }

//    public class DevModeFeature : Feature<DevModeState>
//    {
//        public override string GetName() => "DevMode";

//        protected override DevModeState GetInitialState() => new(false);
//    }

//    public static class DevModeReducers
//    {
//        [ReducerMethod]
//        public static DevModeState ChangeDevModeReducer(DevModeState state,
//            ChangeDevModeAction action) => new(action.Enabled); 
//    }

//    public class ChangeDevModeAction
//    {
//        public bool Enabled { get; }

//        public ChangeDevModeAction(bool enabled)
//        {
//            Enabled = enabled;
//        }
//    }
//}
