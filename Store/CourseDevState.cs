using Client.Models;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (state.Records.Count > 0)
                return new CourseDevState(state.Records);
            var list = new List<RecordMainInfoViewModel>();
            foreach (var item in action.Course.Records)
            {
                list.Add((RecordMainInfoViewModel)item.Clone());
            }
            foreach (var item in list)
            {
                item.Children = list.Where(r => item.Children != null && item.Children.Any(i => i.Id == r.Id)).ToList();
            }
            return new CourseDevState(list);
        }
        [ReducerMethod]
        public static CourseDevState ReduceAddDevCourseRecordAction(CourseDevState state, AddDevCourseRecordAction action)
        {
            if (state.Records.Contains(action.ParentRecord))
            {
                if (action.ParentRecord.Children == null)
                    action.ParentRecord.Children = new();
                if (action.Record.Type == RecordType.Article)
                    action.Record.TargetId = new Random().Next();
                else if (action.Record.Children == null)
                    action.Record.Children = new();
                action.ParentRecord.Children = new(action.ParentRecord.Children.Prepend(action.Record));
                state.Records.Add(action.Record);
            }
            return new CourseDevState(state.Records);
        }

        [ReducerMethod]
        public static CourseDevState ReduceEditDevCourseRecordAction(CourseDevState state, EditDevCourseRecordAction action)
        {
            var record = state.Records.Find(r => r.Id == action.Record.Id);
            if (record != null)
            {
                record = action.Record;
            }
            return new CourseDevState(state.Records);
        }

        [ReducerMethod]
        public static CourseDevState ReduceRemoveDevCourseRecordAction(CourseDevState state, RemoveDevCourseRecordAction action)
        {
            state.Records.Remove(action.Record);
            state.Records.Find(r => r.Children.Contains(action.Record))?.Children.Remove(action.Record);
            return new CourseDevState(state.Records);
        }

        [ReducerMethod]
        public static CourseDevState ReduceMoveDevCourseRecordAction(CourseDevState state, MoveDevCourseRecordAction action)
        {
            if (state.Records.Contains(action.Record))
            {
                if (state.Records.Contains(action.PreviousElement))
                {
                    var destParent = state.Records.Contains(action.DestinationParent) ? action.DestinationParent :
                        state.Records.Find(r => r.Children.Contains(action.PreviousElement));
                    state.Records.Find(r => r.Children.Contains(action.Record)).Children.Remove(action.Record);
                    destParent.Children.Insert(destParent.Children.IndexOf(action.PreviousElement) + 1, action.Record);
                }
                else
                {
                    if(state.Records.Contains(action.DestinationParent) && action.Record != action.DestinationParent)
                    {
                        Console.WriteLine(action.DestinationParent.Name);
                        state.Records.Find(r => r.Children.Contains(action.Record)).Children.Remove(action.Record);
                        action.DestinationParent.Children = action.DestinationParent.Children.Prepend(action.Record).ToList();
                    }
                }
            }
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

    public class AddDevCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }
        public RecordMainInfoViewModel ParentRecord { get; }

        public AddDevCourseRecordAction(RecordMainInfoViewModel record, RecordMainInfoViewModel parentRecord)
        {
            Record = record;
            ParentRecord = parentRecord;
        }
    }

    public class EditDevCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }

        public EditDevCourseRecordAction(RecordMainInfoViewModel record)
        {
            Record = record;
        }
    }

    public class RemoveDevCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }

        public RemoveDevCourseRecordAction(RecordMainInfoViewModel record)
        {
            Record = record;
        }
    }

    public class MoveDevCourseRecordAction
    {
        public RecordMainInfoViewModel Record { get; }
        public RecordMainInfoViewModel PreviousElement { get; }
        public RecordMainInfoViewModel DestinationParent { get; }

        public MoveDevCourseRecordAction(RecordMainInfoViewModel record, RecordMainInfoViewModel destinationParent,
            RecordMainInfoViewModel previousElement)
        {
            Record = record;
            DestinationParent = destinationParent;
            PreviousElement = previousElement;
        }
    }
}
