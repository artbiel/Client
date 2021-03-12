using DiffPlex.Model;
using System;
using System.Collections.Generic;

namespace Client.Models
{
    public class CommitViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string[] DiffWords { get; set; }
        public IList<DiffBlock> DiffBlocks { get; set; }
    }
}
