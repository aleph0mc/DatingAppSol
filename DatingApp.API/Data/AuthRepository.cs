using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private DataContext _Context;
        public AuthRepository(DataContext Context)
        {
            _Context = Context;
        }

        public DataContext Context { get; }

        public async Task<User> Login(string Username, string Password)
        {
            var user = await _Context.Users.FirstOrDefaultAsync(x => Username == x.Username);

            if (null == user)
                return null;

            if (!VerifyPasswordHash(Password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }

        public async Task<User> Register(User Usr, string Password)
        {
            byte[] pwdHash, pwdSalt;
            CreatePasswordHash(Password, out pwdHash, out pwdSalt);

            Usr.PasswordHash = pwdHash;
            Usr.PasswordSalt = pwdSalt;

            await _Context.Users.AddAsync(Usr);
            await _Context.SaveChangesAsync();

            return Usr;
        }

        private void CreatePasswordHash(string password, out byte[] pwdHash, out byte[] pwdSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pwdSalt = hmac.Key;
                pwdHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string Username)
        {
            return await _Context.Users.AnyAsync(x => Username == x.Username);
        }
    }
}
