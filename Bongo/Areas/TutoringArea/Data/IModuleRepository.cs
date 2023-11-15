using Bongo.Areas.TutoringArea.Models;

namespace Bongo.Areas.TutoringArea.Data
{
    public interface IModuleRepository
    {
        void Add(Module module);
        void Delete(Module module);
        void SaveChanges();
        IEnumerable<Module> GetAllSessions();
    }
}
