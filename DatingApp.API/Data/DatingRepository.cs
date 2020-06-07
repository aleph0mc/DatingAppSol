using DatingApp.API.Controllers;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _Context;

        public DatingRepository(DataContext Context)
        {
            _Context = Context;
        }

        public void Add<T>(T Entity) where T : class
        {
            _Context.Add(Entity);
        }

        public void Delete<T>(T Entity) where T : class
        {
            _Context.Remove(Entity);
        }

        public async Task<Like> GetLike(int UserId, int RecipientId)
        {
            return await _Context.Likes.FirstOrDefaultAsync(u => u.LikerId == UserId && u.LikeeId == RecipientId);
        }

        public async Task<Photo> GetMainPhotoFoUser(int UserId)
        {
            return await _Context.Photos.FirstOrDefaultAsync(p => (UserId == p.User.Id) && p.IsMain);
        }

        public async Task<Photo> GetPhoto(int Id)
        {
            return await _Context.Photos.FirstOrDefaultAsync(p => p.Id == Id);
        }

        public async Task<User> GetUser(int Id)
        {
            return await _Context.Users.FirstOrDefaultAsync(u => Id == u.Id);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _Context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
                return user.Likers.Where(u => u.LikeeId == id).Select(s => s.LikerId);
            else
                return user.Likees.Where(u => u.LikerId == id).Select(s => s.LikeeId);

        }

        public async Task<PagedList<User>> GetUsers(UserParams UsrParams)
        {
            var users = _Context.Users.OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => (UsrParams.UserId != u.Id) && (UsrParams.Gender == u.Gender));

            if (UsrParams.Likers)
            {
                var userLikers = await GetUserLikes(UsrParams.UserId, UsrParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (UsrParams.Likees)
            {
                var userLikees = await GetUserLikes(UsrParams.UserId, false);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if ((18 != UsrParams.MinAge) || (99 != UsrParams.MaxAge))
            {
                var minDob = DateTime.Today.AddYears(-UsrParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-UsrParams.MinAge);

                users = users.Where(u => (u.DateOfBirth >= minDob) && (u.DateOfBirth <= maxDob));
            }

            if (!string.IsNullOrEmpty(UsrParams.OrderBy))
                switch (UsrParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }

            return await PagedList<User>.CreateAsync(users, UsrParams.PageNumber, UsrParams.PageSize);
        }

        //public async Task<IEnumerable<User>> GetUsers()
        //{
        //    return await _Context.Users.Include(p => p.Photos).ToListAsync();
        //}

        public async Task<bool> SaveAll()
        {
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _Context.Messages.FirstOrDefaultAsync(m => id == m.Id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _Context.Messages.AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId && !m.SenderDeleted);
                    break;
                default:
                    messages = messages.Where(m => (m.RecipientId == messageParams.UserId) && !m.IsRead && !m.RecipientDeleted);
                    break;
            }

            messages = messages.OrderByDescending(m => m.MessageSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userid, int recipientid)
        {
            var messages = await _Context.Messages
             .Where(m => (m.RecipientId == userid && !m.RecipientDeleted && m.SenderId == recipientid)
             || (m.RecipientId == recipientid && !m.SenderDeleted && m.SenderId == userid))
             .OrderByDescending(m => m.MessageSent).ToListAsync();

            return messages;
        }
    }
}
