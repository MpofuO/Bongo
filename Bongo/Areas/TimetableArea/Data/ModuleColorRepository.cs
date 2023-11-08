using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;
using Microsoft.EntityFrameworkCore;

namespace Bongo.Areas.TimetableArea.Data
{
    public class ModuleColorRepository : RepositoryBase<ModuleColor>, IModuleColorRepository
    {
        public ModuleColorRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public  ModuleColor GetModuleColorWithColorDetails(string username, string moduleCode)
        {
            return  _appDbContext.ModuleColors.Include(m => m.Color)
                .FirstOrDefault(m => m.ModuleCode == moduleCode && m.Username == username);
        }
    }
}
