using Client.Models;
using DiffPlex.Model;
using System.Collections.Generic;
using System.Linq;

namespace Client.Infrastructure
{
    public static class Extentions
    {
        public static List<ArticleDiffBlock> ToArticleDiffBlocks(this IList<DiffBlock> diffBlocks)
        {
            return diffBlocks.Select(block =>
                new ArticleDiffBlock(block.DeleteStartA, block.DeleteCountA, block.InsertStartB, block.InsertCountB)).ToList();
        }
    }
}
