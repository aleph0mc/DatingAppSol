using DatingApp.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class Seed
    {

        private static void CreatePasswordHash(string password, out byte[] pwdHash, out byte[] pwdSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pwdSalt = hmac.Key;
                pwdHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                foreach (var user in users)
                {
                    byte[] pwdHash, pwdSalt;
                    CreatePasswordHash("password", out pwdHash, out pwdSalt);
                    user.PasswordHash = pwdHash;
                    user.PasswordSalt = pwdSalt;
                    user.Username = user.Username.ToLower();

                    context.Users.Add(user);
                }

                context.SaveChanges();
            }
        }
    }
}
