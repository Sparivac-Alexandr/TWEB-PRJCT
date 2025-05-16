using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;

namespace taskoreBusinessLogic.Interfaces
{
    public interface IReview
    {
        List<ReviewDBModel> GetReviewsForUser(int userId);
        bool CreateReview(ReviewDBModel review);
        bool DeleteReview(int reviewId);
        bool UpdateReview(ReviewDBModel review);
        ReviewDBModel GetReviewById(int reviewId);
    }
} 