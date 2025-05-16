using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreBusinessLogic.Interfaces;

namespace taskoreBusinessLogic.BL_Struct
{
    public class ReviewBL : IReview
    {
        public List<ReviewDBModel> GetReviewsForUser(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting reviews for user {userId}");
                
                using (var context = new ReviewContext())
                {
                    // Get all reviews where the current user is the receiver
                    var reviews = context.Reviews
                        .Where(r => r.ReceiverId == userId)
                        .OrderByDescending(r => r.CreatedAt)
                        .ToList();
                    
                    Debug.WriteLine($"Found {reviews.Count} reviews for user {userId}");
                    return reviews;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting reviews for user {userId}: {ex.Message}");
                return new List<ReviewDBModel>();
            }
        }
        
        public bool CreateReview(ReviewDBModel review)
        {
            try
            {
                Debug.WriteLine($"Creating new review from user {review.SenderId} to user {review.ReceiverId}");
                
                using (var context = new ReviewContext())
                {
                    // Set creation date to now
                    review.CreatedAt = DateTime.Now;
                    
                    // Add the review to the database
                    context.Reviews.Add(review);
                    
                    // Save changes
                    int rowsAffected = context.SaveChanges();
                    
                    Debug.WriteLine($"Review created with ID {review.Id}. Rows affected: {rowsAffected}");
                    
                    // Update the receiver's rating
                    if (review.Rating.HasValue)
                    {
                        UpdateUserRating(review.ReceiverId);
                    }
                    
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating review: {ex.Message}");
                return false;
            }
        }
        
        public bool DeleteReview(int reviewId)
        {
            try
            {
                Debug.WriteLine($"Deleting review {reviewId}");
                
                using (var context = new ReviewContext())
                {
                    // Find the review
                    var review = context.Reviews.Find(reviewId);
                    if (review == null)
                    {
                        Debug.WriteLine($"Review {reviewId} not found");
                        return false;
                    }
                    
                    int receiverId = review.ReceiverId;
                    bool hasRating = review.Rating.HasValue;
                    
                    // Remove the review
                    context.Reviews.Remove(review);
                    
                    // Save changes
                    int rowsAffected = context.SaveChanges();
                    
                    Debug.WriteLine($"Review deleted. Rows affected: {rowsAffected}");
                    
                    // Update the receiver's rating if needed
                    if (hasRating)
                    {
                        UpdateUserRating(receiverId);
                    }
                    
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting review {reviewId}: {ex.Message}");
                return false;
            }
        }
        
        public bool UpdateReview(ReviewDBModel review)
        {
            try
            {
                Debug.WriteLine($"Updating review {review.Id}");
                
                using (var context = new ReviewContext())
                {
                    // Find the existing review
                    var existingReview = context.Reviews.Find(review.Id);
                    if (existingReview == null)
                    {
                        Debug.WriteLine($"Review {review.Id} not found");
                        return false;
                    }
                    
                    bool ratingChanged = existingReview.Rating != review.Rating;
                    
                    // Update the review properties
                    existingReview.Content = review.Content;
                    existingReview.Rating = review.Rating;
                    
                    // Save changes
                    int rowsAffected = context.SaveChanges();
                    
                    Debug.WriteLine($"Review updated. Rows affected: {rowsAffected}");
                    
                    // Update the receiver's rating if the rating changed
                    if (ratingChanged && review.Rating.HasValue)
                    {
                        UpdateUserRating(existingReview.ReceiverId);
                    }
                    
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating review {review.Id}: {ex.Message}");
                return false;
            }
        }
        
        public ReviewDBModel GetReviewById(int reviewId)
        {
            try
            {
                Debug.WriteLine($"Getting review {reviewId}");
                
                using (var context = new ReviewContext())
                {
                    var review = context.Reviews.Find(reviewId);
                    return review;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting review {reviewId}: {ex.Message}");
                return null;
            }
        }
        
        private void UpdateUserRating(int userId)
        {
            try
            {
                Debug.WriteLine($"Updating rating for user {userId}");
                
                using (var reviewContext = new ReviewContext())
                using (var userContext = new UserContext())
                {
                    // Get all ratings for the user
                    var ratings = reviewContext.Reviews
                        .Where(r => r.ReceiverId == userId && r.Rating.HasValue)
                        .Select(r => r.Rating.Value)
                        .ToList();
                    
                    if (ratings.Count > 0)
                    {
                        // Calculate average rating
                        double averageRating = ratings.Average();
                        int ratingCount = ratings.Count;
                        
                        // Find the user
                        var user = userContext.Users.FirstOrDefault(u => u.Id == userId);
                        if (user != null)
                        {
                            // Update user rating
                            user.Rating = averageRating;
                            user.RatingCount = ratingCount;
                            
                            // Save changes
                            userContext.SaveChanges();
                            
                            Debug.WriteLine($"Updated user {userId} rating to {averageRating} based on {ratingCount} reviews");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating rating for user {userId}: {ex.Message}");
            }
        }
    }
} 