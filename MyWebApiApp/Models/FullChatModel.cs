using System;

namespace MyWebApiApp.Models
{
    public class FullChatModel
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public Guid ChatId { get; set; }
        public DateTime DateSend { get; set; }
        public string Message { get; set; }
        public byte MessageType { get; set; }

    }
}
