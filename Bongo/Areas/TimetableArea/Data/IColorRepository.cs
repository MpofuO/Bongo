using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;

namespace Bongo.Areas.TimetableArea.Data
{
    public interface IColorRepository : IRepositoryBase<Color>
    {
        public Color GetByName(string name);
    }
}
