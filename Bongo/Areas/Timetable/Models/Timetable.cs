using System.ComponentModel.DataAnnotations;

namespace Bongo.Areas.TimetableArea.Models
{
    public class Timetable
    {
        [Key]
        public int TimetableID { get; set; }

        public string TimetableText { get; set; }

        public string Username { get; set; }
    }
}
