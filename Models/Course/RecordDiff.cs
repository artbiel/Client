using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public abstract class RecordDiff : BaseModel { }

    public class RecordAddDiff : RecordDiff
    {
        public NewRecordVM Record { get; set; }
        public Guid ParentId { get; set; }
    }

    public class RecordChangeDiff : RecordDiff
    {
        public string Title { get; set; }
    }

    public class RecordDeleteDiff : RecordDiff { }
}
