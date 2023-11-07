using Bongo.Areas.TimetableArea.Models;
using Bongo.Areas.TimetableArea.Models.User;
using Bongo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bongo.Data
{
    public class AppDbContext : IdentityDbContext<BongoUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ModuleColor> ModuleColors { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }  

    }
}
