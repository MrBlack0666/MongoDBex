using mongoDB.Models;
using mongoDB.Aspects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Extensibility;
using mongoDB.Repositories;

namespace mongoDB.Services
{
    [UsersLogger(AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class UserService : IUserService
    {
        private IMongoClient dbClient;
        private IMongoDatabase db;
        private IMongoCollection<BsonDocument> userCollection;
        
        private IMessageService _messageService;
        private IUserRepository _userRepository;


        public UserService(IMessageService _messageService, IUserRepository _userRepository)
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase("songsDB");
            userCollection = db.GetCollection<BsonDocument>("users");
            this._messageService = _messageService;
            this._userRepository = _userRepository;
        }

        public UserService(IMessageService _messageService, IUserRepository _userRepository, string database)
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase(database);
            userCollection = db.GetCollection<BsonDocument>("users");

            this._messageService = _messageService;
            this._userRepository = _userRepository;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        //[UsersLogger]
        public User LogIn(string username, string password)
        {
            return _userRepository.LogIn(username, password);
        }

        public bool WasUsernameExist(string username)
        {
            return _userRepository.WasUsernameExist(username);
        }

        //[UsersLogger]
        public User Register(string username, string email, string password)
        {
            if (WasUsernameExist(username))
                return null;

            var user = _userRepository.Add(username, email, password); 

            _messageService.SendEmail(email, $"Witaj, {username}!\n ...............");

            return user;
        }

        public void ChangeUserRole(string username, string currentRole)
        {
            var role = "";
            if (currentRole == "admin")
            {
                role = "user";
            }
            else
            {
                role = "admin";
            }

            _userRepository.ChangeUserRole(username, role);
        }

        public bool DeleteUser(ObjectId id)
        {
            return _userRepository.Delete(id);
        }
    }
}
