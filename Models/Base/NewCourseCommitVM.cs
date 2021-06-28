using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class NewCourseCommitVM
    {
        [Required]
        public CourseCommitType Type { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public RecordVM RootRecord { get; set; }
    }
}
