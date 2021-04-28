using DiffPlex.Model;
using System;
using System.Collections.Generic;

namespace Client.Models
{
    public class ArticleCommitViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string[] DiffWords { get; set; }
        public CommitState CommitState { get; set; }
        public ArticleCommitType Type { get; set; }
        public List<ArticleDiffBlock> DiffBlocks { get; set; }
        public ArticleCommitViewModel PrevCommit { get; set; }
    }

    public enum ArticleCommitType
    {
        Initital,
        Typo,
        VersionChanged,
        Correction,
        Addition
    }

    public enum CommitState
    {
        Developing,
        Checking,
        Commited
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
