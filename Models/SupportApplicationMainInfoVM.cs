using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class SupportApplicationMainInfoVM : BaseModel
    {
        [Required]
        public SupportApplicationType Type { get; set; }
        [Required]
        public SupportApplicationStatus Status { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
