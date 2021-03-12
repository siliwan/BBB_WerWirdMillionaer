using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context)
            : base(context)
        { }

        public void CreateUser(string username, string password)
        {
            string hashedPassword = HashString(password);
            _context.Set<User>().Add(new User
            {
                Username = username,
                PasswordCrypt = hashedPassword
            });
        }

        public bool Login(string username, string password)
        {
            return _context.Set<User>().Any(user => user.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                                                    && user.PasswordCrypt.Equals(HashString(password)));
        }

        private static string HashString(string input)
        {
            using SHA1Managed sha1 = new();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
