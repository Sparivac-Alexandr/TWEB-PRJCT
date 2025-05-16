using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreBusinessLogic.Interfaces;

namespace taskoreBusinessLogic.BL_Struct
{
    public class NewsBL : INews
    {
        public bool CreateNews(NewsDBModel data)
        {
            try
            {
                Debug.WriteLine("Starting news creation process for title: " + data.Title);

                var context = new NewsContext();
                Debug.WriteLine("Database connection established");

                context.News.Add(data);
                Debug.WriteLine("Added news to context");

                int rowsAffected = context.SaveChanges();
                Debug.WriteLine("SaveChanges completed. Rows affected: " + rowsAffected);
                Debug.WriteLine("New news ID assigned by database: " + data.Id);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR in CreateNews: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return false;
            }
        }

        public List<NewsDBModel> GetAllNews()
        {
            try
            {
                Debug.WriteLine("Getting all news");
                
                using (var context = new NewsContext())
                {
                    return context.News
                        .OrderByDescending(n => n.PublishDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR in GetAllNews: " + ex.Message);
                return new List<NewsDBModel>();
            }
        }

        public List<NewsDBModel> GetNewsByCategory(string category)
        {
            try
            {
                Debug.WriteLine($"Getting news for category: {category}");
                
                using (var context = new NewsContext())
                {
                    return context.News
                        .Where(n => n.Category == category)
                        .OrderByDescending(n => n.PublishDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetNewsByCategory for category {category}: " + ex.Message);
                return new List<NewsDBModel>();
            }
        }

        public List<NewsDBModel> GetNewsByPriority(string priority)
        {
            try
            {
                Debug.WriteLine($"Getting news with priority: {priority}");
                
                using (var context = new NewsContext())
                {
                    return context.News
                        .Where(n => n.Priority == priority)
                        .OrderByDescending(n => n.PublishDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetNewsByPriority for priority {priority}: " + ex.Message);
                return new List<NewsDBModel>();
            }
        }

        public NewsDBModel GetNewsById(int id)
        {
            try
            {
                Debug.WriteLine($"Getting news with ID: {id}");
                
                using (var context = new NewsContext())
                {
                    return context.News
                        .FirstOrDefault(n => n.Id == id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetNewsById for ID {id}: " + ex.Message);
                return null;
            }
        }

        public bool DeleteNews(int id)
        {
            try
            {
                using (var context = new NewsContext())
                {
                    var news = context.News.Find(id);
                    if (news != null)
                    {
                        context.News.Remove(news);
                        int rowsAffected = context.SaveChanges();
                        return rowsAffected > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in DeleteNews for ID {id}: " + ex.Message);
                return false;
            }
        }
        
        public bool UpdateNews(NewsDBModel news)
        {
            try
            {
                using (var context = new NewsContext())
                {
                    var existingNews = context.News.Find(news.Id);
                    if (existingNews != null)
                    {
                        existingNews.Title = news.Title;
                        existingNews.Content = news.Content;
                        existingNews.Category = news.Category;
                        existingNews.Priority = news.Priority;
                        existingNews.Author = news.Author;
                        existingNews.ImageUrl = news.ImageUrl;
                        
                        int rowsAffected = context.SaveChanges();
                        return rowsAffected > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in UpdateNews for ID {news.Id}: " + ex.Message);
                return false;
            }
        }
    }
} 