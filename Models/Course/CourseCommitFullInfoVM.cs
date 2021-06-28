using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class CourseCommitVM : BaseModel
    {
        public List<RecordDiff> RecordsDiff { get; set; }
    }
}
