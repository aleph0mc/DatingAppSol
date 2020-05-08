using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User Usr, string Password);
        Task<User> Login(string Username, string Password);
        Task<bool> UserExists(string Username);
    }
}
