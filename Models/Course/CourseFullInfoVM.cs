using System;
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class CourseFullInfoVM : BaseWBItem
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public Difficulty Difficulty { get; set; }
        public RatingModel Rating { get; set; }
        public RecordVM RootRecord { get; set; }
    }

    public class CourseEditingModel : BaseWBItem
    {
        [Required]
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public Difficulty Difficulty { get; set; }
    }

    public enum Difficulty
    {
        Beginer,
        Intermediate,
        Expert
    }
}
