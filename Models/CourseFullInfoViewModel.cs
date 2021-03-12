using System.Collections.Generic;

namespace Client.Models
{
    public class CourseFullInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public Difficulty Difficulty { get; set; }
        public List<RecordMainInfoViewModel> Records { get; set; }
    }

    public enum Difficulty
    {
        Beginer,
        Intermediate,
        Expert
    }
}
