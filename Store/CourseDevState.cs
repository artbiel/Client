using Client.Models;
using Fluxor;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Client.Store
{
    public class CourseDevState
    {
        public List<RecordMainInfoViewModel> Records { get; }

        public CourseDevState(List<RecordMainInfoViewModel> records)
        {
            Records = records ?? new();
        }
    }

    public class CourseDevFeature : Feature<CourseDevState>
    {
        public override string GetName() => "CourseDev";

        protected override CourseDevState GetInitialState() => new CourseDevState(null);
    }

    public static class CourseDevReducers
    {
        [ReducerMethod]
        public static CourseDevState ReduceSetDevCourseAction(CourseDevState state, SetDevCourseAction action)
        {
            var list = JsonSerializer.Deserialize<List<RecordMainInfoViewModel>>(JsonSerializer.Serialize(action.Course.Records, options: new() { MaxDepth = 2000}));
            return new CourseDevState(list);
        }
        [ReducerMethod]
        public static CourseDevState ReduceAppendDevCourseRecordAction(CourseDevState state, AppendDevCourseRecordAction action)
        {
            foreach (var rec in state.Records)
            {
                System.Console.WriteLine("form action " + rec.Parent?.Id);
            }
            var parent = state.Records.Find(r => r == action.Record.Parent);
            System.Console.WriteLine("parent Id: " + action.Record.Parent.Id);
            if (parent != null)
            {
                System.Console.WriteLine("Found");
                state.Records.Find(r => r.Id == action.Record.Parent.Id).Children.Add(action.Record);
                return new CourseDevState(state.Records.Append(action.Record).ToList());
            }
            System.Console.WriteLine("Not Found");
            return new CourseDevState(state.Records);
        }
    }


    public class SetDevCourseAction
    {
        public CourseFullInfoViewModel Course { get; }

        public SetDevCourseAction(CourseFullInfoViewModel course)
        {
            Course = course;
        }
    }

    public class AppendDevCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }

        public AppendDevCourseRecordAction(RecordMainInfoViewModel record)
        {
            Record = record;
        }
    }
}
