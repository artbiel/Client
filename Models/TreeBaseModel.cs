using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public abstract class TreeBaseModel<T> : BaseModel
    {
        public List<T> Children { get; set; }
    }
}
