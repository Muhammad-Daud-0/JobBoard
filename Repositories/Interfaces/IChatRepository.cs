using JobBoard.Models.Entities;

namespace JobBoard.Repositories.Interfaces
{
    public interface IChatRepository : IRepository<ChatMessage>
    {
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string user1Id, string user2Id);
        Task<IEnumerable<ChatMessage>> GetRecentChatsForUserAsync(string userId);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<IEnumerable<ChatMessage>> GetNewUnreadMessagesAsync(string userId, int lastSeenMessageId);
    }
}
