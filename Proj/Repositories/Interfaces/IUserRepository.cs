using mongoDB.Models;
using MongoDB.Bson;
using System.Collections.Generic;

namespace mongoDB.Repositories
{
    public interface IUserRepository
    {
        User Add(string username, string email, string password);
        void ChangeUserRole(string username, string role);
        bool Delete(ObjectId id);
        List<User> GetAll();
        User LogIn(string username, string password);
        bool WasUsernameExist(string username);
    }
}