using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBZadaniaPS
{
    class Program
    {
        static MongoClient dbClient;
        static IMongoDatabase db;
        static IMongoCollection<BsonDocument> titleCollection;
        static IMongoCollection<BsonDocument> castCollection;
        static IMongoCollection<BsonDocument> crewCollection;
        static IMongoCollection<BsonDocument> ratingsCollection;
        static IMongoCollection<BsonDocument> nameCollection;

        static void InitializeMongoDB()
        {
            //dbClient = new MongoClient("mongodb://localhost:27017");
            //db = dbClient.GetDatabase("bazafilmow");
            //var collection = db.GetCollection<BsonDocument>("Title");

            //collection.Find(new BsonDocument()).ForEachAsync(x => Console.WriteLine(x));

            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase("imdb");

            titleCollection = db.GetCollection<BsonDocument>("Title");
            castCollection = db.GetCollection<BsonDocument>("Cast");
            crewCollection = db.GetCollection<BsonDocument>("Crew");
            ratingsCollection = db.GetCollection<BsonDocument>("Ratings");
            nameCollection = db.GetCollection<BsonDocument>("Name");
        }

        static void Zadanie2()
        {
            var titleCount = titleCollection.CountDocuments(new BsonDocument());
            var castCount = castCollection.CountDocuments(new BsonDocument());
            var crewCount = crewCollection.CountDocuments(new BsonDocument());
            var ratingCount = ratingsCollection.CountDocuments(new BsonDocument());
            var nameCount = nameCollection.CountDocuments(new BsonDocument());

            var firstTitle = titleCollection.Find(new BsonDocument()).FirstOrDefault()
                .ToJson(new JsonWriterSettings() { Indent = true});
            var firstCast = castCollection.Find(new BsonDocument()).FirstOrDefault()
                .ToJson(new JsonWriterSettings() { Indent = true });
            var firstCrew = crewCollection.Find(new BsonDocument()).FirstOrDefault()
                .ToJson(new JsonWriterSettings() { Indent = true });
            var firstRating = ratingsCollection.Find(new BsonDocument()).FirstOrDefault()
                .ToJson(new JsonWriterSettings() { Indent = true });
            var firstName = nameCollection.Find(new BsonDocument()).FirstOrDefault()
                .ToJson(new JsonWriterSettings() { Indent = true });

            Console.WriteLine("Ilość dokumentów Title:  " + titleCount);
            Console.WriteLine("Ilość dokumentów Cast:   " + castCount);
            Console.WriteLine("Ilość dokumentów Crew:   " + crewCount);
            Console.WriteLine("Ilość dokumentów Rating: " + ratingCount);
            Console.WriteLine("Ilość dokumentów Name:   " + nameCount);
            Console.WriteLine("\n------------------------------------------\n");
            Console.WriteLine("Pierwszy dokument Title:\n" + firstTitle + "\n");
            Console.WriteLine("Pierwszy dokument Cast:\n" + firstCast + "\n");
            Console.WriteLine("Pierwszy dokument Crew:\n" + firstCrew + "\n");
            Console.WriteLine("Pierwszy dokument Rating:\n" + firstRating + "\n");
            Console.WriteLine("Pierwszy dokument Name:\n" + firstCrew + "\n");
        }

        static void Zadanie3()
        {
            var builderFilter = Builders<BsonDocument>.Filter;
            var builderProjection = Builders<BsonDocument>.Projection;

            var filter = builderFilter.Eq(x => x["startYear"], 2005) &
                builderFilter.Regex(x => x["genres"], new BsonRegularExpression(".*Romance.*")) &
                builderFilter.Gt(x => x["runtimeMinutes"], 100) &
                builderFilter.Lte(x => x["runtimeMinutes"], 120);

            var projection = builderProjection.Include(x => x["primaryTitle"])
                .Include(x => x["startYear"]).Include(x => x["genres"])
                .Include(x => x["runtimeMinutes"]).Exclude(x => x["_id"]);

            var resultWithoutLimit = titleCollection.Find(filter).Project(projection)
                .SortBy(x => x["primaryTitle"]);
            var resultCount = resultWithoutLimit.CountDocuments();
            var resultWithLimit = resultWithoutLimit.Limit(5).ToList()
                .ToJson(new JsonWriterSettings() { Indent = true });

            Console.WriteLine("Ilość dokumnetów: " + resultCount);
            Console.WriteLine(resultWithLimit);
        }

        static void Zadanie4()
        {
            var builderFilter = Builders<BsonDocument>.Filter;
            var builderProjection = Builders<BsonDocument>.Projection;

            var filter = builderFilter.Eq(x => x["startYear"], 1930) &
                builderFilter.Regex(x => x["genres"], new BsonRegularExpression(".*Comedy."));

            var projection = builderProjection.Include(x => x["originalTitle"])
                .Include(x => x["runtimeMinutes"]).Include(x => x["genres"])
                .Exclude(x => x["_id"]);

            var result = titleCollection.Find(filter).Project(projection)
                .SortBy(x => x["runtimeMinutes"]);

            var resultCount = result.CountDocuments();
            var resultWithLimit = result.Limit(5).ToList()
                .ToJson(new JsonWriterSettings() { Indent = true });

            Console.WriteLine("Ilość dokumentów: " + resultCount);
            Console.WriteLine(resultWithLimit);
        }

        static void Zadanie5()
        {
            var builderFilter = Builders<BsonDocument>.Filter;
            var builderProjection = Builders<BsonDocument>.Projection;

            var filterTitle = builderFilter.Eq(x => x["primaryTitle"], "Casablanca") &
                builderFilter.Eq(x => x["startYear"], 1942);

            var filterCast = builderFilter.Regex(x => x["cast.category"],
                new BsonRegularExpression(".*director.*"));

            var projection = builderProjection.Include(x => x["primaryName"])
                .Include(x => x["birthDate"]);

            var result = titleCollection.Aggregate().Match(filterTitle)
                .Lookup("Cast", "tconst", "tconst", "cast").Unwind("cast")
                .Lookup("Name", "cast.nconst", "nconst", "name").Unwind("name")
                .Match(filterCast).Project(x => new
                {
                    primaryName = x["name.primaryName"],
                    birthYear = x["name.birthYear"]
                }).FirstOrDefault().ToJson(new JsonWriterSettings() { Indent = true });

            Console.WriteLine(result);
        }

        static void Zadanie6()
        {
            var builderFilter = Builders<BsonDocument>.Filter;

            var filter = builderFilter.Gte(x => x["startYear"], 2007) &
                builderFilter.Lte(x => x["startYear"], 2009);

            var result = titleCollection.Aggregate().Match(filter)
                .Group(x => x["titleType"], y => new
                {
                    titleType = y.Key,
                    howMany = y.Count()
                }).ToList().ToJson(new JsonWriterSettings() { Indent = true });

            Console.WriteLine(result);
        }

        static void Zadanie7()
        {
            var builderFilter = Builders<BsonDocument>.Filter;
            var builderProjection = Builders<BsonDocument>.Projection;

            var filter = builderFilter.Gte(x => x["startYear"], 1994) &
                builderFilter.Lte(x => x["startYear"], 1996);

            titleCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>("{tconst: 1}"));
            ratingsCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>("{tconst: 1}"));

            var result = titleCollection.Aggregate().Match(filter)
                .Lookup("Ratings", "tconst", "tconst", "ratings")
                .SortByDescending(x => x["ratings.averageRating"])
                .Limit(10).Project(x => new
                {
                    primaryTitle = x["primaryTitle"],
                    startYear = x["startYear"],
                    averageRating = x["ratings.averageRating"],
                }).ToList().ToJson(new JsonWriterSettings() { Indent = true });

            Console.WriteLine(result);
        }

        static void Zadanie8()
        {
            var builderFilter = Builders<BsonDocument>.Filter;
            var builderProjection = Builders<BsonDocument>.Projection;

            var filter = builderFilter.Eq(x => x["averageRating"], 10.0);

            var projection = builderProjection.Include(x => x["tconst"])
                .Exclude(x => x["_id"]);

            var titleTconstWithMaxAvgRating = ratingsCollection.Find(filter)
                .Project(projection).ToList();

            var updateFilter = builderFilter.In(x => x["tconst"], 
                titleTconstWithMaxAvgRating.Select(x => x["tconst"]));


            var modifiedCount = titleCollection.UpdateMany(updateFilter, "{$set: {max: 1}}")
                .ModifiedCount;

            Console.WriteLine("Zmodyfikowano: " + modifiedCount);
        }

        static void Zadanie9()
        {
            var builderFilter = Builders<BsonDocument>.Filter;
            var builderProjection = Builders<BsonDocument>.Projection;

            var filter = builderFilter.Gte(x => x["birthYear"], 1950) &
                builderFilter.Lte(x => x["birthYear"], 1980) &
                builderFilter.Regex(x => x["primaryProfession"],
                new BsonRegularExpression("(.*actor.*|.*actress.*|.*director.*)"));

            var projection = builderProjection.Include(x => x["primaryName"])
                .Include(x => x["birthYear"]).Include(x => x["primaryProfession"])
                .Exclude(x => x["_id"]);

            nameCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>
                ("{primaryProfession: \"text\"}"));

            var result = nameCollection.Find(filter);

            var resultCount = result.CountDocuments();
            var resultWithLimit = result.Limit(5).Project(projection).ToList()
                .ToJson(new JsonWriterSettings() { Indent = true });

            Console.WriteLine("Ilość dokumentów: " + resultCount);
            Console.WriteLine(resultWithLimit);
        }

        static void Zadanie10()
        {
            //nameCollection.Indexes.DropOne("primaryProfession_text");

            var builderFilter = Builders<BsonDocument>.Filter;
            var builderProjection = Builders<BsonDocument>.Projection;

            var filter = builderFilter.Regex(x => x["primaryName"],
                new BsonRegularExpression("(.*Fonda.*|.*Coppola.*)"));

            var projection = builderProjection.Include(x => x["primaryName"])
                .Include(x => x["primaryProfession"]).Exclude(x => x["_id"]);

            nameCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>
                ("{primaryName: \"text\"}"));

            var result = nameCollection.Find(filter);
            var resultCount = result.CountDocuments();
            var resultWithLimit = result.Limit(5).Project(projection).ToList()
                .ToJson(new JsonWriterSettings() { Indent = true });
            var indexesCount = nameCollection.Indexes.List().ToList().Count;

            Console.WriteLine("Ilość dokumentów: " + resultCount);
            Console.WriteLine("Ilość indeksów: " + indexesCount);
            Console.WriteLine(resultWithLimit);
        }

        static void Main(string[] args)
        {
            InitializeMongoDB();

            //Zadanie2();
            //Zadanie3();
            //Zadanie4();
            //Zadanie5();
            //Zadanie6();
            //Zadanie7();
            //Zadanie8();
            //Zadanie9();
            Zadanie10();

            Console.ReadKey();
        }
    }
}
