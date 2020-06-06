using DatingApp.API.Helpers;
using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T Entity) where T : class;
        void Delete<T>(T Entity) where T : class;
        Task<bool> SaveAll();
        //Replaced
        //Task<IEnumerable<User>> GetUsers();
        //with the following to allow paging
        Task<PagedList<User>> GetUsers(UserParams UsrParams);
        Task<User> GetUser(int Id);
        Task<Photo> GetPhoto(int Id);
        Task<Photo> GetMainPhotoFoUser(int UserId);
        Task<Like> GetLike(int UserId, int RecipientId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int userid, int recipientid);
    }
}
