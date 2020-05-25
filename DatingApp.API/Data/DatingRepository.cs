using DatingApp.API.Controllers;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await _Context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => Id == u.Id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams UsrParams)
        {
            var users = _Context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => (UsrParams.UserId != u.Id) && (UsrParams.Gender == u.Gender));

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
    }
}
