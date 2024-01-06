using MyWebApiApp.Models;
using System.Collections.Generic;

namespace MyWebApiApp.Services
{
    public interface IChatInfoRepository
    {
        List<ChatInfoVM> GetAll(int SenderId, int ReceiverId, int page);
        ChatInfoVM GetById(string id);
        ChatInfoVM Add(int SenderId, int ReceiverId, ChatInfoModel chatInfo);
        void Update(ChatInfoVM loai);
        void Delete(string id);
    }
}
