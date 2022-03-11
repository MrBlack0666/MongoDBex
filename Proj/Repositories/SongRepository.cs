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
    public class SongRepository : ISongRepository
    {
        private readonly int ELEMENTS_IN_PAGE = 10;

        private IMongoClient dbClient;
        private IMongoDatabase db;
        private IMongoCollection<BsonDocument> songsCollection;

        public SongRepository()
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase("songsDB");

            songsCollection = db.GetCollection<BsonDocument>("songs");
        }

        public SongRepository(string database)
        {
            dbClient = new MongoClient("mongodb://localhost:27017");
            db = dbClient.GetDatabase(database);

            songsCollection = db.GetCollection<BsonDocument>("songs");
        }

        public void Add(SongVM song)
        {
            if (song.Ratings == null)
            {
                song.Ratings = new List<RatingClass>();
            }

            var document = new BsonDocument()
            {
                { "title",  song.Title },
                { "releaseYear", song.ReleaseYear },
                { "averageRating", 0 },
                { "lyrics", song.Lyrics },
                { "albums", new BsonArray(song.Albums) },
                { "ratings", new BsonArray(song.Ratings) },
                { "performers", new BsonArray(song.Performers) },
                { "lastRatingId",  song.Ratings.Count + 1 }
            };

            songsCollection.InsertOne(document);
        }

        public void Edit(ObjectId id, SongVM song)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id);
            var updateBuilders = Builders<BsonDocument>.Update
                .Set("title", song.Title)
                .Set("releaseYear", song.ReleaseYear)
                .Set("lyrics", song.Lyrics)
                .Set("albums", new BsonArray(song.Albums))
                .Set("performers", new BsonArray(song.Performers));

            songsCollection.UpdateOne(filter, updateBuilders);
        }

        public void Delete(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id);

            songsCollection.DeleteOne(filter);
        }

        public Song GetById(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id);

            var songJson = songsCollection.Aggregate().Match(filter)
                .Lookup("performers", "performers", "_id", "perfs")
                .Project(s => new
                {
                    _id = s["_id"],
                    Title = s["title"],
                    ReleaseYear = s["releaseYear"],
                    AverageRating = s["averageRating"],
                    Lyrics = s["lyrics"],
                    Albums = s["albums"],
                    Ratings = s["ratings"],
                    Performers = s["perfs"],
                    LastRatingId = s["lastRatingId"]
                }).FirstOrDefault().ToJson();

            return BsonSerializer.Deserialize<Song>(songJson);
        }

        public GetSongsVM GetWithFilters(SongFilters songFilters)
        {
            var filterBeforeLookup = Builders<BsonDocument>.Filter.Regex(s => s["title"], new BsonRegularExpression(".*" + songFilters.Title + ".*", "i"))
                & Builders<BsonDocument>.Filter.Gte(s => s["releaseYear"], songFilters.YearFrom)
                & Builders<BsonDocument>.Filter.Lte(s => s["releaseYear"], songFilters.YearTo > 0 ? songFilters.YearTo : 10000)
                & Builders<BsonDocument>.Filter.Gte(s => s["averageRating"], Convert.ToDouble(songFilters.AvgFrom <= 10 ? songFilters.AvgFrom : 10))
                & Builders<BsonDocument>.Filter.Lte(s => s["averageRating"], Convert.ToDouble(songFilters.AvgTo >= 0 ? songFilters.AvgTo : 0));

            if (!string.IsNullOrWhiteSpace(songFilters.Album))
            {
                filterBeforeLookup &= Builders<BsonDocument>.Filter.Regex(s => s["albums"], new BsonRegularExpression(".*" + songFilters.Album + ".*", "i"));
            }

            var filterAfterLookup = Builders<BsonDocument>.Filter.Regex(s => s["perfs.nickname"], new BsonRegularExpression(".*" + songFilters.Performer + ".*", "i"))
                | Builders<BsonDocument>.Filter.Regex(s => s["perfs.firstName"], new BsonRegularExpression(".*" + songFilters.Performer + ".*", "i"))
                | Builders<BsonDocument>.Filter.Regex(s => s["perfs.surname"], new BsonRegularExpression(".*" + songFilters.Performer + ".*", "i"));

            var songs = songsCollection.Aggregate().Match(filterBeforeLookup)
                .Lookup("performers", "performers", "_id", "perfs")
                .Match(filterAfterLookup).Project(s => new
                {
                    _id = s["_id"],
                    Title = s["title"],
                    ReleaseYear = s["releaseYear"],
                    AverageRating = s["averageRating"],
                    Lyrics = s["lyrics"],
                    Albums = s["albums"],
                    Ratings = s["ratings"],
                    Performers = s["perfs"],
                    LastRatingId = s["lastRatingId"]
                });

            if (songFilters.SortBy == SortSongEnum.TITLE)
            {
                if (!songFilters.IsDescending)
                {
                    songs = songs.SortBy(s => s.Title);
                }
                else
                {
                    songs = songs.SortByDescending(s => s.Title);
                }
            }
            else if (songFilters.SortBy == SortSongEnum.AVERAGERATING)
            {
                if (!songFilters.IsDescending)
                {
                    songs = songs.SortBy(s => s.AverageRating);
                }
                else
                {
                    songs = songs.SortByDescending(s => s.AverageRating);
                }
            }
            else if (songFilters.SortBy == SortSongEnum.RELEASEYEAR)
            {
                if (!songFilters.IsDescending)
                {
                    songs = songs.SortBy(s => s.ReleaseYear);
                }
                else
                {
                    songs = songs.SortByDescending(s => s.ReleaseYear);
                }
            }

            var allPages = Convert.ToInt32(Math.Ceiling(1.0 * (songs.Count().FirstOrDefault() == null ? 0 : songs.Count().FirstOrDefault().Count) / ELEMENTS_IN_PAGE));
            var songsAfterPaging = songs.Skip(songFilters.Page > 0 ? (songFilters.Page - 1) * ELEMENTS_IN_PAGE : 0).Limit(ELEMENTS_IN_PAGE).ToList();

            var songsList = BsonSerializer.Deserialize<List<Song>>(songsAfterPaging.ToJson());

            return new GetSongsVM()
            {
                AllPages = allPages,
                Songs = songsList
            };
        }

        public void DeletePerformerIdInSongs(ObjectId performerId)
        {
            var filter = Builders<BsonDocument>.Filter.AnyEq("performers", performerId);

            var updateBuilder = Builders<BsonDocument>.Update.Pull("performers", performerId);

            var result = songsCollection.UpdateMany(filter, updateBuilder);
        }

        public bool AddRatingToSong(ObjectId songId, RatingClass rating)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], songId);

            var songsBson = songsCollection.Find(filter).FirstOrDefault();
            var lastRatingId = songsBson.GetValue("lastRatingId").ToInt32();
            var ratingsBson = songsBson.GetValue("ratings").AsBsonArray;
            var ratingCount = ratingsBson.Count;
            int sum = 0;

            foreach (var ratingBson in ratingsBson.ToList())
            {
                sum += ratingBson["rating"].AsInt32;
            }

            ratingCount++;
            sum += rating.Rating;
            var avg = 1.0 * sum / ratingCount;

            var document = new BsonDocument()
            {
                { "rating", rating.Rating },
                { "review", rating.Review },
                { "ratingId", lastRatingId++ }
            };

            var updateBuilders = Builders<BsonDocument>.Update
                .AddToSet("ratings", document)
                .Set("averageRating", avg)
                .Set("lastRatingId", lastRatingId);

            var result = songsCollection.UpdateOne(filter, updateBuilders);

            return result.ModifiedCount == 1;
        }

        public bool DeleteRatingFromSong(ObjectId songId, int ratingId)
        {
            var arrayDocuments = new BsonArray();
            var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], songId);

            var songsBson = songsCollection.Find(filter).FirstOrDefault();

            var ratingsBson = songsBson.GetValue("ratings").AsBsonArray;
            int sum = 0;

            foreach (var ratingBson in ratingsBson.ToList())
            {
                if (ratingBson["ratingId"].AsInt32 != ratingId)
                {
                    arrayDocuments.Add(new BsonDocument()
                    {
                        { "rating", ratingBson["rating"].AsInt32 },
                        { "review", ratingBson["review"].AsString },
                        { "ratingId", ratingBson["ratingId"].AsInt32 }
                    });

                    sum += ratingBson["rating"].AsInt32;
                }
            }

            var avg = 1.0 * sum / arrayDocuments.Count;

            var updateBuilders = Builders<BsonDocument>.Update
                .Set("ratings", arrayDocuments)
                .Set("averageRating", avg);

            var result = songsCollection.UpdateOne(filter, updateBuilders);

            return result.ModifiedCount == 1;
        }

        public List<SongForPerformer> GetSongsForPerformer(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.AnyEq("performers", id);

            var songs = songsCollection.Find(filter).Project(s => new
            {
                Title = s["title"],
                ReleaseYear = s["releaseYear"],
                AverageRating = s["averageRating"]
            }).ToList();

            var songsList = BsonSerializer.Deserialize<List<SongForPerformer>>(songs.ToJson());

            return songsList;
        }
    }
}
