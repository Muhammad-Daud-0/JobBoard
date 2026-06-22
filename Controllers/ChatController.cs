using JobBoard.Models.DTOs.Chat;
using JobBoard.Models.Entities;
using JobBoard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobBoard.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(IChatService chatService, UserManager<ApplicationUser> userManager)
        {
            _chatService = chatService;
            _userManager = userManager;
        }

        [HttpGet("Messages")]
        public async Task<IActionResult> Index(string? withUserId)
        {
            var currentUserId = _userManager.GetUserId(User)!;
            var conversations = await _chatService.GetRecentConversationsAsync(currentUserId);
            
            ViewBag.Conversations = conversations;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.WithUserId = withUserId;

            if (!string.IsNullOrEmpty(withUserId))
            {
                var otherUser = await _userManager.FindByIdAsync(withUserId);
                if (otherUser != null)
                {
                    ViewBag.OtherUserName = !string.IsNullOrEmpty(otherUser.CompanyName) ? otherUser.CompanyName : otherUser.FullName;
                    ViewBag.OtherUserRole = (await _userManager.GetRolesAsync(otherUser)).FirstOrDefault() ?? "User";
                    
                    // Mark messages as read
                    await _chatService.MarkMessagesAsReadAsync(withUserId, currentUserId);
                    
                    // Fetch history
                    var history = await _chatService.GetChatHistoryAsync(currentUserId, withUserId);
                    return View(history);
                }
            }

            return View();
        }

        [HttpPost("Chat/SendMessage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content) || string.IsNullOrWhiteSpace(dto.ReceiverId))
            {
                return BadRequest("Invalid message content or receiver.");
            }

            var currentUserId = _userManager.GetUserId(User)!;
            var message = await _chatService.SendMessageAsync(currentUserId, dto.ReceiverId, dto.Content);

            return Json(new
            {
                success = true,
                message = new
                {
                    id = message.Id,
                    content = message.Content,
                    timestamp = message.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    senderId = message.SenderId,
                    receiverId = message.ReceiverId,
                    isRead = message.IsRead
                }
            });
        }

        [HttpGet("Chat/GetMessages")]
        public async Task<IActionResult> GetMessages(string withUserId)
        {
            if (string.IsNullOrEmpty(withUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            var currentUserId = _userManager.GetUserId(User)!;
            
            // Mark incoming messages as read
            await _chatService.MarkMessagesAsReadAsync(withUserId, currentUserId);
            
            var history = await _chatService.GetChatHistoryAsync(currentUserId, withUserId);
            
            var result = history.Select(m => new
            {
                id = m.Id,
                content = m.Content,
                timestamp = m.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                isRead = m.IsRead
            });

            return Json(result);
        }

        [HttpGet("Chat/GetConversations")]
        public async Task<IActionResult> GetConversations()
        {
            var currentUserId = _userManager.GetUserId(User)!;
            var conversations = await _chatService.GetRecentConversationsAsync(currentUserId);
            var result = conversations.Select(c => new
            {
                otherUserId = c.OtherUserId,
                otherUserName = c.OtherUserName,
                otherUserRole = c.OtherUserRole,
                lastMessageContent = c.LastMessageContent,
                lastMessageTimestamp = c.LastMessageTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                hasUnread = c.HasUnread
            });
            return Json(result);
        }

        [HttpGet("Chat/GetUnreadStatus")]
        public async Task<IActionResult> GetUnreadStatus(int lastSeenMessageId)
        {
            var currentUserId = _userManager.GetUserId(User)!;
            var unreadCount = await _chatService.GetUnreadCountAsync(currentUserId);
            
            var messages = await _chatService.GetNewUnreadMessagesAsync(currentUserId, lastSeenMessageId);
            var newMessages = messages.Select(m => new
            {
                id = m.Id,
                senderName = !string.IsNullOrEmpty(m.Sender?.CompanyName) ? m.Sender.CompanyName : m.Sender?.FullName ?? "User",
                senderId = m.SenderId,
                content = m.Content
            });

            return Json(new
            {
                unreadCount = unreadCount,
                newMessages = newMessages
            });
        }
    }
}
