using Backend.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        void CreateUser(string username, string password);
        bool Login(string username, string password);
    }
}
