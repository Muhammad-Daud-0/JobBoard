using JobBoard.Models.DTOs.Chat;
using JobBoard.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobBoard.Services.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string user1Id, string user2Id);
        Task<IEnumerable<ChatConversationDto>> GetRecentConversationsAsync(string userId);
        Task<ChatMessage> SendMessageAsync(string senderId, string receiverId, string content);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<IEnumerable<ChatMessage>> GetNewUnreadMessagesAsync(string userId, int lastSeenMessageId);
    }
}
