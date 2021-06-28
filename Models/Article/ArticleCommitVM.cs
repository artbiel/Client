using DiffPlex.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class ArticleCommitVM : BaseCommit
    {
        [Required]
        public string[] DiffWords { get; set; }
        [Required]
        public List<ArticleDiffBlock> DiffBlocks { get; set; }
        [Required]
        public ArticleCommitType Type { get; set; }
    }

    public enum ArticleCommitType
    {
        Typo,
        VersionChanged,
        Correction,
        Addition
    }

    public class ArticleDiffBlock
    {
        public ArticleDiffBlock(int deleteStartA, int deleteCountA, int insertStartB, int insertCountB)
        {
            DeleteStartA = deleteStartA;
            DeleteCountA = deleteCountA;
            InsertStartB = insertStartB;
            InsertCountB = insertCountB;
        }

        public int DeleteStartA { get; set; }
        public int DeleteCountA { get; set; }
        public int InsertStartB { get; set; }
        public int InsertCountB { get; set; }
    }
}
