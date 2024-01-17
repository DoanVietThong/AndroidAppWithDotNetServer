using MyWebApiApp.Models;
using System.Collections.Generic;

namespace MyWebApiApp.Services
{
    public interface IChatInfoRepository
    {
        List<ChatInfoVM> GetAll(int SenderId, int ReceiverId, int page);
        ChatInfoVM GetById(string id);
        List<FullChatModel> GetChat(int userRoot, int userClient, int page);
        ChatInfoVM Add(int SenderId, int ReceiverId, ChatInfoModel chatInfo);
        void Update(ChatInfoVM loai);
        void Delete(string id);
    }
}
