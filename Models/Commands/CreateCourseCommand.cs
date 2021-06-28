using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models.Commands
{
    public class CreateCourseCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public Difficulty Difficulty { get; set; }
    }
}
