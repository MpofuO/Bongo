using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;

namespace Bongo.Areas.TimetableArea.Data
{
    public interface IModuleColorRepository : IRepositoryBase<ModuleColor>
    {
        ModuleColor GetModuleColorWithColorDetails(string username, string moduleCode);
    }
}
