using Bongo.Areas.TimetableArea.Models.User;
using Bongo.Data;

namespace Bongo.Areas.TimetableArea.Data
{
    public class UserReviewRepository : RepositoryBase<UserReview>, IUserReviewRepository
    {
        public UserReviewRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
