﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class WorkbanchArticleVM : BaseWBItem
    {
        public Guid? CourseId { get; set; }
        public string Content { get; set; }
    }
}
