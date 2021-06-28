using Client.Models;
using DiffPlex.Model;
using System;
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

        public static T Find<T>(this T root, Guid id) where T : TreeBaseModel<T>
        {
            if (root.Id == id)
                return root;
            if (root?.Children == null)
                return null;
            foreach (var child in root.Children)
            {
                var item = child.Find(id);
                if (item != null)
                    return item as T;
            }
            return null;
        }

        public static T Find<T>(this T root, Predicate<T> predicate) where T : TreeBaseModel<T>
        {
            if (predicate(root))
                return root;
            if (root?.Children == null)
                return null;
            foreach (var child in root.Children)
            {
                var item = child.Find(predicate);
                if (item != null)
                    return item;
            }
            return null;
        }

        public static T Find<T>(this List<T> rootList, Guid id) where T : TreeBaseModel<T>
        {
            for (int i = 0; i < rootList.Count; i++)
            {
                var item = rootList[i].Find(id);
                if (item != null)
                    return item;
            }
            return null;
        }

        public static bool Contains<T>(this T rootRecord, Guid id) where T : TreeBaseModel<T>
        {
            return rootRecord.Find(id) != null;
        }

        public static void Where<T>(this T root, Predicate<T> predicate, ref List<T> list) where T : TreeBaseModel<T>
        {
            if (predicate(root))
                list.Add(root);
            if (root?.Children == null)
                return;
            foreach (var child in root.Children)
            {
                child.Where(predicate, ref list);
            }
        }
    }

}