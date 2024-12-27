using armHospital.Data.Repositories;
using armHospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace armHospital.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Register(User user)
        {
            var existingUser = await _userRepository.FindUserByUsername(user.Username);
            if (existingUser != null)
            {
                return false; // user уже существует
            }

            await _userRepository.AddUser(user);
            return true;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _userRepository.FindUserByUsername(username);
            if (user != null && user.Password == password)
            {
                return user;
            }
            return null;
        }
    }
}
