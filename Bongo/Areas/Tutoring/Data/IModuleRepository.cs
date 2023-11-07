using Bongo.Areas.Tutoring.Models;

namespace Bongo.Areas.Tutoring.Data
{
    public interface IModuleRepository
    {
        void Add(Module module);
        void Delete(Module module);
        void SaveChanges();
        IEnumerable<Module> GetAllSessions();
    }
}
