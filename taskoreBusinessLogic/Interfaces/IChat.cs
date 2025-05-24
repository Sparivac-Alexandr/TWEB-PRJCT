using System;
using System.Collections.Generic;
using taskoreBusinessLogic.DBModel;

namespace taskoreBusinessLogic.Interfaces
{
    public interface IChat
    {
        List<taskoreBusinessLogic.DBModel.ChatMessageDBModel> GetUserMessages(int userId);
        List<int> GetUserIdsWithMessages(int userId);
        List<UDBModel> GetChatUsers(int userId);
        Dictionary<int, int> GetNewMessagesPerUser(int userId, DateTime lastVisit);
        int GetTotalNewMessagesCount(int userId, DateTime lastVisit);
        UDBModel GetUserById(int userId);
        void UpdateUserLastActive(int userId);
        bool SendMessage(int fromUserId, int toUserId, string message);
        List<taskoreBusinessLogic.DBModel.ChatMessageDBModel> GetConversation(int user1Id, int user2Id);
    }
} 