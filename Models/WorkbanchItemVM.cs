using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class WorkbanchItemVM : BaseWBItem
    {
        public WorkbanchItemType Type { get; set; }
    }

    public enum WorkbanchItemType
    {
        Course,
        Article
    }
}
