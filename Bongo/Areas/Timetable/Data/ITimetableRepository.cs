using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;

namespace Bongo.Areas.TimetableArea.Data
{
    public interface ITimetableRepository : IRepositoryBase<Timetable>
    {
        public Timetable GetUserTimetable(string username);
    }
}
