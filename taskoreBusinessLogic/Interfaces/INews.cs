using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;

namespace taskoreBusinessLogic.Interfaces
{
    public interface INews
    {
        bool CreateNews(NewsDBModel data);
        List<NewsDBModel> GetAllNews();
        List<NewsDBModel> GetNewsByCategory(string category);
        List<NewsDBModel> GetNewsByPriority(string priority);
        NewsDBModel GetNewsById(int id);
        bool DeleteNews(int id);
        bool UpdateNews(NewsDBModel news);
    }
} 