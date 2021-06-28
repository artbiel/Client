using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public abstract class BaseWBItem : BaseModel
    {
        [Required]
        public string Title { get; set; }
    }
}
