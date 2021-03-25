using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class RecordMainInfoViewModel : ICloneable
    {
        public int Id { get; set; }
        public int TargetId { get; set; }
        public RecordType Type { get; set; }
        public string Name { get; set; }
        public List<RecordMainInfoViewModel> Children { get; set; }

        public object Clone()
        {
            var record = (RecordMainInfoViewModel)MemberwiseClone();
            if (Children != null)
                record.Children = Children.Select(r => (RecordMainInfoViewModel)r.Clone()).ToList();
            return record;
        }
    }

    public enum RecordType
    {
        Article,
        Group,
        Root
    }
}
