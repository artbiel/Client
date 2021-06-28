using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class CourseAggregatedVM
    {
        public CourseFullInfoVM Course { get; set; }
        public List<CourseCommitMainInfoVM> Commits { get; set; }
    }
}
