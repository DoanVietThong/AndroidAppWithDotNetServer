using Microsoft.EntityFrameworkCore;
using MyWebApiApp.Data;
using MyWebApiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWebApiApp.Services
{
    public class ChatInfoRepository : IChatInfoRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 10;
        public ChatInfoRepository(MyDbContext context) 
        {
            _context = context;
        }
        ChatInfoVM IChatInfoRepository.Add(int SenderId, int ReceiverId, ChatInfoModel model)
        {
            var chatInfo = new Data.ChatInfo 
            {
                ChatId = Guid.NewGuid(),
                SendDate = model.DateSend,
                Message = model.Message,
                MessageType = model.MessageType
            };

            var chat = new Data.Chat
            {
                SenderId = SenderId,
                ReceiverId = ReceiverId,
                ChatId = chatInfo.ChatId,
            };


            _context.Add(chatInfo);
            _context.Add(chat);
            _context.SaveChanges();
            return new ChatInfoVM
            {
                ChatId = chatInfo.ChatId,
                DateSend = chatInfo.SendDate,
                Message = chatInfo.Message,
                MessageType =chatInfo.MessageType
            };

        }

        void IChatInfoRepository.Delete(string id)
        {
            var chat = _context.ChatInfos.SingleOrDefault(lo => lo.ChatId == Guid.Parse(id));
            if (chat != null)
            {
                _context.Remove(chat);
                _context.SaveChanges(true);
            }
        }

        public List<ChatInfoVM> GetAll(int senderId, int receiverId, int page = 1)
        {
            var allChats = _context.Chats.AsQueryable()
                .Join(_context.ChatInfos,
                    chat => chat.ChatId,
                    chatInfo => chatInfo.ChatId,
                    (chat, chatInfo) => new { Chat = chat, ChatInfo = chatInfo });

            #region Filtering
            allChats = allChats.Where(join => join.Chat.SenderId == senderId && join.Chat.ReceiverId == receiverId);
            #endregion

            #region Sorting
            allChats = allChats.OrderByDescending(join => join.ChatInfo.SendDate);
            #endregion

            #region Paging
            var totalCount = allChats.Count();
            var result = allChats.Skip((page - 1) * PAGE_SIZE)
                         .Take(PAGE_SIZE)
                         .Select(join => new ChatInfoVM
                         {
                             ChatId = join.ChatInfo.ChatId,
                             DateSend = join.ChatInfo.SendDate,
                             Message = join.ChatInfo.Message,
                             MessageType = join.ChatInfo.MessageType
                         })
                         .ToList();
            #endregion

            return result;
        }

        ChatInfoVM IChatInfoRepository.GetById(string id)
        {
            var chat = _context.ChatInfos.SingleOrDefault(chat =>chat.ChatId == Guid.Parse(id));
            if (chat == null) return null;
            return new ChatInfoVM
            {
                ChatId = chat.ChatId,
                DateSend=chat.SendDate,
                Message=chat.Message,
                MessageType=chat.MessageType
            };
        }

        void IChatInfoRepository.Update(ChatInfoVM loai)
        {
            var chat = _context.ChatInfos.SingleOrDefault(lo => lo.ChatId== loai.ChatId);
            if (chat == null) return;
            chat.SendDate = loai.DateSend;
            chat.Message = loai.Message;
            chat.MessageType = loai.MessageType;
            _context.SaveChanges();
        }

        List<FullChatModel> IChatInfoRepository.GetChat(int userRoot, int userClient, int page)
        {
            var query = (from chat in _context.Chats
                        join chatInfo in _context.ChatInfos on chat.ChatId equals chatInfo.ChatId
                        where chat.SenderId == userRoot && chat.ReceiverId == userClient || chat.ReceiverId == userRoot && chat.SenderId == userClient
                        select new FullChatModel
                        {
                            SenderId = chat.SenderId,
                            ReceiverId = chat.ReceiverId,
                            ChatId = chat.ChatId,
                            DateSend = chatInfo.SendDate, 
                            Message=chatInfo.Message, 
                            MessageType = chatInfo.MessageType
                        }).Skip((page-1)*PAGE_SIZE).Take(PAGE_SIZE);
            var result = new List<FullChatModel>();
            foreach (var item in query)
            {
                result.Add(item);   
            }
            return result;
        }
    }
}
