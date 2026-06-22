using JobBoard.Models.DTOs.Chat;
using JobBoard.Models.Entities;
using JobBoard.Repositories;
using JobBoard.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string user1Id, string user2Id)
        {
            return await _uow.ChatMessages.GetChatHistoryAsync(user1Id, user2Id);
        }

        public async Task<IEnumerable<ChatConversationDto>> GetRecentConversationsAsync(string userId)
        {
            var recentMessages = await _uow.ChatMessages.GetRecentChatsForUserAsync(userId);
            var dtos = new List<ChatConversationDto>();

            foreach (var m in recentMessages)
            {
                var otherUser = m.SenderId == userId ? m.Receiver : m.Sender;
                if (otherUser == null) continue;

                var otherUserId = otherUser.Id;
                var roles = await _userManager.GetRolesAsync(otherUser);
                var role = roles.FirstOrDefault() ?? "User";

                // Check if the last message was from the other user and is unread
                var hasUnread = !m.IsRead && m.SenderId == otherUserId;

                dtos.Add(new ChatConversationDto
                {
                    OtherUserId = otherUserId,
                    OtherUserName = !string.IsNullOrEmpty(otherUser.CompanyName) ? otherUser.CompanyName : otherUser.FullName,
                    OtherUserRole = role,
                    LastMessageContent = m.Content,
                    LastMessageTimestamp = m.Timestamp,
                    HasUnread = hasUnread
                });
            }

            return dtos.OrderByDescending(d => d.LastMessageTimestamp);
        }

        public async Task<ChatMessage> SendMessageAsync(string senderId, string receiverId, string content)
        {
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await _uow.ChatMessages.AddAsync(message);
            await _uow.SaveChangesAsync();
            return message;
        }

        public async Task MarkMessagesAsReadAsync(string senderId, string receiverId)
        {
            await _uow.ChatMessages.MarkMessagesAsReadAsync(senderId, receiverId);
            await _uow.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _uow.ChatMessages.GetUnreadCountAsync(userId);
        }

        public async Task<IEnumerable<ChatMessage>> GetNewUnreadMessagesAsync(string userId, int lastSeenMessageId)
        {
            return await _uow.ChatMessages.GetNewUnreadMessagesAsync(userId, lastSeenMessageId);
        }
    }
}
