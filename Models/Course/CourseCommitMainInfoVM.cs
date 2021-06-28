using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class CourseCommitMainInfoVM : BaseCommit
    {
        public CourseCommitType Type { get; set; }
    }

    public enum CourseCommitType
    {
        Added,
        Changed,
        Deleted
    }
}
