using JobBoard.Data;
using JobBoard.Models.Entities;
using JobBoard.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _db;

        public ChatRepository(AppDbContext db) => _db = db;

        public async Task<ChatMessage?> GetByIdAsync(int id) =>
            await _db.ChatMessages.FindAsync(id);

        public async Task<IEnumerable<ChatMessage>> GetAllAsync() =>
            await _db.ChatMessages.ToListAsync();

        public async Task AddAsync(ChatMessage entity) =>
            await _db.ChatMessages.AddAsync(entity);

        public void Update(ChatMessage entity) => _db.ChatMessages.Update(entity);

        public void Delete(ChatMessage entity) => _db.ChatMessages.Remove(entity);

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string user1Id, string user2Id) =>
            await _db.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

        public async Task<IEnumerable<ChatMessage>> GetRecentChatsForUserAsync(string userId)
        {
            var messages = await _db.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            return messages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.First())
                .ToList();
        }

        public async Task MarkMessagesAsReadAsync(string senderId, string receiverId)
        {
            var unread = await _db.ChatMessages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            foreach (var m in unread)
            {
                m.IsRead = true;
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId) =>
            await _db.ChatMessages.CountAsync(m => m.ReceiverId == userId && !m.IsRead);

        public async Task<IEnumerable<ChatMessage>> GetNewUnreadMessagesAsync(string userId, int lastSeenMessageId) =>
            await _db.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == userId && !m.IsRead && m.Id > lastSeenMessageId)
                .ToListAsync();
    }
}
