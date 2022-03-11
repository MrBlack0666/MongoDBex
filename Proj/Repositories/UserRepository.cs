using mongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IMongoClient dbClient;
        private IMongoDatabase db;
        private IMongoCollection<BsonDocument> userCollection;

        public UserRepository()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase("songsDB");
            userCollection = db.GetCollection<BsonDocument>("users");
        }

        public UserRepository(string database)
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase(database);
            userCollection = db.GetCollection<BsonDocument>("users");
        }

        public User Add(string username, string email, string password)
        {
            var document = new BsonDocument()
            {
                { "username",  username },
                { "email", email },
                { "password", password },
                { "role", "user" }
            };

            userCollection.InsertOne(document);

            return LogIn(username, password);
        }

        public bool Delete(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id) &
                Builders<BsonDocument>.Filter.Ne(s => s["role"], "admin");

            var result = userCollection.DeleteOne(filter);

            return result.DeletedCount == 1;
        }

        public List<User> GetAll()
        {
            var users = userCollection.Find(new BsonDocument())
                .SortBy(u => u["username"])
                .ToList();

            var userList = BsonSerializer.Deserialize<List<User>>(users.ToJson());

            return userList;
        }

        public User LogIn(string username, string password)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(u => u["username"], username)
                & Builders<BsonDocument>.Filter.Eq(u => u["password"], password);

            var user = userCollection.Find(filter).FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            return BsonSerializer.Deserialize<User>(user.ToJson());
        }

        public bool WasUsernameExist(string username)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(u => u["username"], username);

            var userCount = userCollection.CountDocuments(filter);

            return userCount > 0;
        }

        public void ChangeUserRole(string username, string role)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(u => u["username"], username);
            var updateBuilders = Builders<BsonDocument>.Update.Set("role", role);
            var user = userCollection.UpdateOne(filter, updateBuilders);
        }
    }
}
