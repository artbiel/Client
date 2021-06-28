using System;
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public abstract class BaseCommit : BaseModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public CommitState CommitState { get; set; }
        [Required]
        public Guid? PrevCommitId { get; set; }
    }


    public enum CommitState
    {
        Unsent,
        Invalid,
        Checking,
        Rejected,
        Commited
    }
}
