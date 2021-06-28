using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class SupportApplicationFullInfoVM : BaseModel
    {
        [Required]
        public SupportApplicationType Type { get; set; }
        [Required]
        public SupportApplicationStatus Status { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        public List<CommentVM> Comments { get; set; }
    }

    public enum SupportApplicationType
    {
        Question,
        Bug,
        Proposal
    }

    public enum SupportApplicationStatus
    {
        Waiting,
        Processing,
        Completed
    }
}
