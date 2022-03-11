using mongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Repositories
{
    public class PerformerRepository : IPerformerRepository
    {
        private readonly int ELEMENTS_IN_PAGE = 10;

        private IMongoClient dbClient;
        private IMongoDatabase db;
        private IMongoCollection<BsonDocument> performersCollection;

        public PerformerRepository()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase("songsDB");

            performersCollection = db.GetCollection<BsonDocument>("performers");
        }

        public PerformerRepository(string database)
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase(database);

            performersCollection = db.GetCollection<BsonDocument>("performers");
        }

        public void Add(Performer performer)
        {
            var document = new BsonDocument()
            {
                { "nickname",  performer.Nickname },
                { "firstName", performer.FirstName },
                { "surname", performer.Surname },
                { "originCountry", performer.OriginCountry },
                { "birthDate", performer.BirthDate }
            };

            performersCollection.InsertOne(document);
        }

        public void Edit(ObjectId id, Performer performer)
        {
            var document = new BsonDocument()
            {
                { "nickname",  performer.Nickname },
                { "firstName", performer.FirstName },
                { "surname", performer.Surname },
                { "originCountry", performer.OriginCountry },
                { "birthDate", performer.BirthDate }
            };

            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id);

            performersCollection.ReplaceOne(filter, document);
        }

        public bool Delete(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id);

            var result = performersCollection.DeleteOne(filter);

            return result.DeletedCount == 1;
        }

        public Performer GetById(ObjectId id)
        {
            return new Performer();
        }

        public List<PerformerVM> GetAll()
        {
            var performers = performersCollection.Find(new BsonDocument())
                .SortBy(p => p["nickname"])
                .Project(p => new
                {
                    _id = p["_id"],
                    nickname = p["nickname"]
                }).ToList();

            var performersList = BsonSerializer.Deserialize<List<PerformerVM>>(performers.ToJson());

            return performersList;
        }

        public GetPerformersVM GetWithFilters(PerformerFilters performerFilters)
        {
            var filter = (Builders<BsonDocument>.Filter.Regex(s => s["nickname"], new BsonRegularExpression(".*" + performerFilters.Performer + ".*", "i"))
                | Builders<BsonDocument>.Filter.Regex(s => s["firstName"], new BsonRegularExpression(".*" + performerFilters.Performer + ".*", "i"))
                | Builders<BsonDocument>.Filter.Regex(s => s["surname"], new BsonRegularExpression(".*" + performerFilters.Performer + ".*", "i")))
                & Builders<BsonDocument>.Filter.Regex(s => s["originCountry"], new BsonRegularExpression(".*" + performerFilters.Country + ".*", "i"))
                & Builders<BsonDocument>.Filter.Gte(s => s["birthDate"], new DateTime(performerFilters.YearFrom > 0 ? performerFilters.YearFrom : 1, 1, 1))
                & Builders<BsonDocument>.Filter.Lt(s => s["birthDate"], new DateTime(performerFilters.YearTo > 0 ? performerFilters.YearTo + 1 : 9999, 1, 1));

            var performers = performersCollection.Find(filter)
                .Project(p => new
                {
                    _id = p["_id"],
                    nickname = p["nickname"],
                    firstName = p["firstName"],
                    surname = p["surname"],
                    originCountry = p["originCountry"],
                    birthDate = p["birthDate"]
                });

            if (performerFilters.SortBy != SortPerformersEnum.NONE && !performerFilters.IsDescending)
            {
                var sortBy = GetStringFromEnum(performerFilters.SortBy);
                performers = performers.SortBy(p => p[sortBy]);
            }
            else if (performerFilters.SortBy != SortPerformersEnum.NONE && performerFilters.IsDescending)
            {
                var sortBy = GetStringFromEnum(performerFilters.SortBy);
                performers = performers.SortByDescending(p => p[sortBy]);
            }

            var allPages = Convert.ToInt32(Math.Ceiling(1.0 * performers.CountDocuments() / ELEMENTS_IN_PAGE));

            performers = performers.Skip(performerFilters.Page > 0 ? (performerFilters.Page - 1) * ELEMENTS_IN_PAGE : 0).Limit(ELEMENTS_IN_PAGE);

            var performersList = BsonSerializer.Deserialize<List<Performer>>(performers.ToList().ToJson());

            return new GetPerformersVM()
            {
                AllPages = allPages,
                Performers = performersList
            };
        }

        private string GetStringFromEnum(object performerEnum)
        {
            var fi = performerEnum.GetType().GetField(performerEnum.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : performerEnum.ToString();
        }
    }
}
