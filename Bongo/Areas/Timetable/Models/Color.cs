using System.ComponentModel.DataAnnotations;

namespace Bongo.Areas.TimetableArea.Models
{
    public class Color
    {
        [Key]
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorValue { get; set; }
    }
}
