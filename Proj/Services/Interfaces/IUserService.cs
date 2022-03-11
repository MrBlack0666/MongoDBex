using mongoDB.Models;
using MongoDB.Bson;
using System.Collections.Generic;

namespace mongoDB.Services
{
    public interface IUserService
    {
        void ChangeUserRole(string username, string currentRole);
        List<User> GetAllUsers();
        User LogIn(string username, string password);
        User Register(string username, string email, string password);
        bool WasUsernameExist(string username);
        bool DeleteUser(ObjectId id);
    }
}