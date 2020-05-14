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

        public async Task<User> GetUser(int Id)
        {
            return await _Context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => Id == u.Id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _Context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}
