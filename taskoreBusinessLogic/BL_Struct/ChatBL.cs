using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreBusinessLogic.Interfaces;

namespace taskoreBusinessLogic.BL_Struct
{
    public class ChatBL : IChat
    {
        public List<ChatMessageDBModel> GetUserMessages(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting messages for user ID: {userId}");
                
                using (var context = new UserContext())
                {
                    return context.ChatMessages
                        .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                        .OrderByDescending(m => m.SentAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetUserMessages: {ex.Message}");
                return new List<ChatMessageDBModel>();
            }
        }

        public List<int> GetUserIdsWithMessages(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting user IDs with messages for user ID: {userId}");
                
                using (var context = new UserContext())
                {
                    return context.ChatMessages
                        .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                        .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                        .Distinct()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetUserIdsWithMessages: {ex.Message}");
                return new List<int>();
            }
        }

        public List<UDBModel> GetChatUsers(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting chat users for user ID: {userId}");
                
                using (var context = new UserContext())
                {
                    var userIdsWithMessages = GetUserIdsWithMessages(userId);
                    return context.Users.Where(u => userIdsWithMessages.Contains(u.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetChatUsers: {ex.Message}");
                return new List<UDBModel>();
            }
        }

        public Dictionary<int, int> GetNewMessagesPerUser(int userId, DateTime lastVisit)
        {
            try
            {
                Debug.WriteLine($"Getting new messages count per user for user ID: {userId}");
                
                using (var context = new UserContext())
                {
                    var users = GetChatUsers(userId);
                    var newMessagesPerUser = users.ToDictionary(
                        u => u.Id,
                        u => context.ChatMessages.Count(m => m.ReceiverId == userId && m.SenderId == u.Id && m.SentAt > lastVisit)
                    );
                    return newMessagesPerUser;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetNewMessagesPerUser: {ex.Message}");
                return new Dictionary<int, int>();
            }
        }

        public int GetTotalNewMessagesCount(int userId, DateTime lastVisit)
        {
            try
            {
                var newMessagesPerUser = GetNewMessagesPerUser(userId, lastVisit);
                return newMessagesPerUser.Values.Sum();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetTotalNewMessagesCount: {ex.Message}");
                return 0;
            }
        }

        public UDBModel GetUserById(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting user with ID: {userId}");
                
                using (var context = new UserContext())
                {
                    return context.Users.FirstOrDefault(u => u.Id == userId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetUserById: {ex.Message}");
                return null;
            }
        }

        public void UpdateUserLastActive(int userId)
        {
            try
            {
                Debug.WriteLine($"Updating last active time for user ID: {userId}");
                
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        user.LastActiveAt = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in UpdateUserLastActive: {ex.Message}");
            }
        }

        public bool SendMessage(int fromUserId, int toUserId, string message)
        {
            try
            {
                Debug.WriteLine($"Sending message from user {fromUserId} to user {toUserId}");
                
                using (var context = new UserContext())
                {
                    // Update sender's last active time
                    UpdateUserLastActive(fromUserId);
                    
                    // Check if recipient exists
                    var toUser = context.Users.FirstOrDefault(u => u.Id == toUserId);
                    if (toUser == null)
                    {
                        Debug.WriteLine($"Recipient user with ID {toUserId} not found");
                        return false;
                    }
                    
                    // Create and save the message
                    var chatMsg = new ChatMessageDBModel
                    {
                        SenderId = fromUserId,
                        ReceiverId = toUserId,
                        Content = message,
                        SentAt = DateTime.Now
                    };
                    
                    context.ChatMessages.Add(chatMsg);
                    int rowsAffected = context.SaveChanges();
                    
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in SendMessage: {ex.Message}");
                return false;
            }
        }

        public List<ChatMessageDBModel> GetConversation(int user1Id, int user2Id)
        {
            try
            {
                Debug.WriteLine($"Getting conversation between users {user1Id} and {user2Id}");
                
                using (var context = new UserContext())
                {
                    return context.ChatMessages
                        .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                                   (m.SenderId == user2Id && m.ReceiverId == user1Id))
                        .OrderBy(m => m.SentAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetConversation: {ex.Message}");
                return new List<ChatMessageDBModel>();
            }
        }
    }
} 