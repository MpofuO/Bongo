namespace Bongo.Areas.TimetableArea.Models.ViewModels
{
    public class MergerIndexViewModel
    {
        public List<string> MergedUsers { get; set; }
        public Session[,] Sessions { get; set; }
    }
}
