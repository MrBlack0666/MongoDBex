using mongoDB.Aspects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using mongoDB.Exceptions;
using mongoDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PostSharp.Extensibility;
using mongoDB.Repositories;

namespace mongoDB.Services
{
    [SongsLogger(AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class SongService : ISongService
    {
        private readonly int ELEMENTS_IN_PAGE = 10;

        //private static SongsDatabase songsDatabase = null;

        //private IMongoClient dbClient;
        //private IMongoDatabase db;
        //private IMongoCollection<BsonDocument> songsCollection;

        private ISongRepository _songRepository;

        public SongService(ISongRepository _songRepository)
        {
            //dbClient = new MongoClient("mongodb://localhost:27017");
            //db = dbClient.GetDatabase("songsDB");

            //songsCollection = db.GetCollection<BsonDocument>("songs");

            this._songRepository = _songRepository;
        }

        public SongService(ISongRepository _songRepository, string database)
        {
            //dbClient = new MongoClient("mongodb://localhost:27017");
            //db = dbClient.GetDatabase(database);

            //songsCollection = db.GetCollection<BsonDocument>("songs");

            this._songRepository = _songRepository;

            //try
            //{
            //    AddPerformer(new Performer() { BirthDate = DateTime.Now.AddDays(-1), FirstName="Jan", Nickname="Kowalski", OriginCountry="Poland", Surname="Kowalski" });
            //}
            //catch(ValidationException e)
            //{
            //    var xd = e.Title + e.Message;
            //}

            //EditPerformer(new ObjectId("5edb991e64d1111d641d1d53"), new Performer() { BirthDate = DateTime.Now, FirstName = "Kacper", Surname = "Kowal", Nickname = "KK", OriginCountry = "Poland" });
            //DeletePerformer(new ObjectId("5edb9cf564d1110b2c3dc07b"));
            //AddSong(new SongVM() { Albums = new List<string> { "Pierwszy", "Drugi" }, AverageRating = 0, Lyrics = "ABC", ReleaseYear = 2000, Title = "Nowy", Ratings = new List<RatingClass>(), Performers= new List<ObjectId> { new ObjectId("5edb9cf564d1110b2c3dc07b"), new ObjectId("5eda399864d1112bc475a8b7") } });
            //EditSong(new ObjectId("5edbd12864d11125dc00ffd8"), new SongVM() );
            //DeleteSong(new ObjectId("5edb9e1d64d11129d437da88"));
            //AddRatingToSong(new ObjectId("5ed906501c258620d8319c10"), new RatingClass() { Rating = 10} );
            //DeleteRatingFromSong(new ObjectId("5ed906501c258620d8319c10"), 1);
            //GetPerformersWithFilter(new PerformerFilters() {  });
            //GetSongsWithFilter(new SongFilters() { Page = 1, AvgFrom = 0, AvgTo = 10, SortBy = SortSongEnum.TITLE, IsDescending = false });
            //GetAllPerformers();
            //GetSongsForPerformer(new ObjectId("5ed8f1341c258620d8319bf2"));

        }

        //        public static SongsDatabase GetInstance()
        //        {
        //            if(songsDatabase == null)
        //            {
        //                songsDatabase = new SongsDatabase();
        //            }
        //
        //            return songsDatabase;
        //        }

        //[PerformersLogger]


        //public bool AddPerformer(Performer performer)
        //{
        //    ValidatePerformer(performer);

        //    var document = new BsonDocument()
        //    {
        //        { "nickname",  performer.Nickname },
        //        { "firstName", performer.FirstName },
        //        { "surname", performer.Surname },
        //        { "originCountry", performer.OriginCountry },
        //        { "birthDate", performer.BirthDate }
        //    };

        //    performersCollection.InsertOne(document);

        //    return true;
        //}

        //[PerformersLogger]
        //public bool EditPerformer(ObjectId id, Performer performer)
        //{
        //    ValidatePerformer(performer);

        //    var document = new BsonDocument()
        //    {
        //        { "nickname",  performer.Nickname },
        //        { "firstName", performer.FirstName },
        //        { "surname", performer.Surname },
        //        { "originCountry", performer.OriginCountry },
        //        { "birthDate", performer.BirthDate }
        //    };

        //    var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id);

        //    var result = performersCollection.ReplaceOne(filter, document);

        //    return result.ModifiedCount == 1;
        //}

        //[PerformersLogger]
        //public bool DeletePerformer(ObjectId id)
        //{
        //    var filter = Builders<BsonDocument>.Filter.Eq(s => s["_id"], id);

        //    var result = performersCollection.DeleteOne(filter);

        //    if (result.DeletedCount == 1)
        //    {
        //        DeletePerformerIdInSongs(id);
        //    }

        //    return result.DeletedCount == 1;
        //}

        public bool AddSong(SongVM song)
        {
            ValidateSongVM(song);

            _songRepository.Add(song);

            return true;
        }

        public bool EditSong(ObjectId id, SongVM song)
        {
            ValidateSongVM(song);

            _songRepository.Edit(id, song);

            return true;
        }

        public bool DeleteSong(ObjectId id)
        {
            _songRepository.Delete(id);

            return true;
        }

        public bool AddRatingToSong(ObjectId songId, RatingClass rating)
        {
            ValidateRating(rating);

            return _songRepository.AddRatingToSong(songId, rating);
        }

        //[SongsLogger]
        public bool DeleteRatingFromSong(ObjectId songId, int ratingId)
        {
            return _songRepository.DeleteRatingFromSong(songId, ratingId);
        }

        //public List<PerformerVM> GetAllPerformers()
        //{
        //    var performers = performersCollection.Find(new BsonDocument())
        //        .SortBy(p => p["nickname"])
        //        .Project(p => new
        //        {
        //            _id = p["_id"],
        //            nickname = p["nickname"]
        //        }).ToList();

        //    var performersList = BsonSerializer.Deserialize<List<PerformerVM>>(performers.ToJson());

        //    return performersList;
        //}

        //public GetPerformersVM GetPerformersWithFilter(PerformerFilters performerFilters)
        //{
        //    var filter = (Builders<BsonDocument>.Filter.Regex(s => s["nickname"], new BsonRegularExpression(".*" + performerFilters.Performer + ".*", "i"))
        //        | Builders<BsonDocument>.Filter.Regex(s => s["firstName"], new BsonRegularExpression(".*" + performerFilters.Performer + ".*", "i"))
        //        | Builders<BsonDocument>.Filter.Regex(s => s["surname"], new BsonRegularExpression(".*" + performerFilters.Performer + ".*", "i")))
        //        & Builders<BsonDocument>.Filter.Regex(s => s["originCountry"], new BsonRegularExpression(".*" + performerFilters.Country + ".*", "i"))
        //        & Builders<BsonDocument>.Filter.Gte(s => s["birthDate"], new DateTime(performerFilters.YearFrom > 0 ? performerFilters.YearFrom : 1, 1, 1))
        //        & Builders<BsonDocument>.Filter.Lt(s => s["birthDate"], new DateTime(performerFilters.YearTo > 0 ? performerFilters.YearTo + 1 : 9999, 1, 1));

        //    var performers = performersCollection.Find(filter)
        //        .Project(p => new
        //        {
        //            _id = p["_id"],
        //            nickname = p["nickname"],
        //            firstName = p["firstName"],
        //            surname = p["surname"],
        //            originCountry = p["originCountry"],
        //            birthDate = p["birthDate"]
        //        });

        //    if (performerFilters.SortBy != SortPerformersEnum.NONE && !performerFilters.IsDescending)
        //    {
        //        var sortBy = GetStringFromEnum(performerFilters.SortBy);
        //        performers = performers.SortBy(p => p[sortBy]);
        //    }
        //    else if (performerFilters.SortBy != SortPerformersEnum.NONE && performerFilters.IsDescending)
        //    {
        //        var sortBy = GetStringFromEnum(performerFilters.SortBy);
        //        performers = performers.SortByDescending(p => p[sortBy]);
        //    }

        //    var allPages = Convert.ToInt32(Math.Ceiling(1.0 * performers.CountDocuments() / ELEMENTS_IN_PAGE));

        //    performers = performers.Skip(performerFilters.Page > 0 ? (performerFilters.Page - 1) * ELEMENTS_IN_PAGE : 0).Limit(ELEMENTS_IN_PAGE);

        //    var performersList = BsonSerializer.Deserialize<List<Performer>>(performers.ToList().ToJson());

        //    return new GetPerformersVM()
        //    {
        //        AllPages = allPages,
        //        Performers = performersList
        //    };
        //}

        public GetSongsVM GetSongsWithFilter(SongFilters songFilters)
        {
            return _songRepository.GetWithFilters(songFilters);
        }

        public Song GetSong(ObjectId id)
        {
            return _songRepository.GetById(id);
        }

        public List<SongForPerformer> GetSongsForPerformer(ObjectId id)
        {
            return _songRepository.GetSongsForPerformer(id);
        }

        //private string GetStringFromEnum(object performerEnum)
        //{
        //    var fi = performerEnum.GetType().GetField(performerEnum.ToString());
        //    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //    return (attributes.Length > 0) ? attributes[0].Description : performerEnum.ToString();
        //}

        public void DeletePerformerIdInSongs(ObjectId performerId)
        {
            _songRepository.DeletePerformerIdInSongs(performerId);
        }

        //private void ValidatePerformer(Performer performer)
        //{
        //    var exceptionTitle = "Nieprawidłowe dane wykonawcy";
        //    var exceptionMessage = "Wykryto błędy:";
        //    bool isInvalid = false;

        //    if (string.IsNullOrWhiteSpace(performer.Nickname))
        //    {
        //        isInvalid = true;
        //        exceptionMessage += "\nPole pseudonimu wykonawcy nie może być puste";
        //    }

        //    if (string.IsNullOrWhiteSpace(performer.FirstName))
        //    {
        //        isInvalid = true;
        //        exceptionMessage += "\nPole imienia wykonawcy nie może być puste";
        //    }

        //    if (string.IsNullOrWhiteSpace(performer.Surname))
        //    {
        //        isInvalid = true;
        //        exceptionMessage += "\nPole nazwiska wykonawcy nie może być puste";
        //    }

        //    if (string.IsNullOrWhiteSpace(performer.OriginCountry))
        //    {
        //        isInvalid = true;
        //        exceptionMessage += "\nPole kraju pochodzenia wykonawcy nie może być puste";
        //    }

        //    if (performer.BirthDate > DateTime.Now)
        //    {
        //        isInvalid = true;
        //        exceptionMessage += "\nData urodzenia wykonawcy musi być mniejsza od daty bierzącej";
        //    }

        //    if (isInvalid)
        //        throw new ValidationException(exceptionTitle, exceptionMessage);
        //}

        private void ValidateSongVM(SongVM song)
        {
            var exceptionTitle = "Nieprawidłowe dane utworu";
            var exceptionMessage = "Wykryto błędy:";
            bool isInvalid = false;

            if (string.IsNullOrWhiteSpace(song.Title))
            {
                isInvalid = true;
                exceptionMessage += "\nPole tytułu utworu nie może być puste";
            }

            if (song.ReleaseYear <= 0)
            {
                isInvalid = true;
                exceptionMessage += "\nPole roku wydania utworu nie może być minejsze niż 1";
            }

            var albums = new List<string>();

            if (song.Albums != null)
            {
                foreach (var album in song.Albums)
                {
                    albums.Add(album.ToLower());
                }

                if (!albums.All(new HashSet<string>().Add))
                {
                    isInvalid = true;
                    exceptionMessage += "\nAlbumy na którym znajduje się utwór nie mogą być takie same";
                }
            }

            if (song.Performers == null || song.Performers.Count == 0)
            {
                isInvalid = true;
                exceptionMessage += "\nNie dodano, żadnego wykonawcy do utworu";
            }

            if (isInvalid)
                throw new ValidationException(exceptionTitle, exceptionMessage);
        }

        private void ValidateRating(RatingClass rating)
        {
            var exceptionTitle = "Nieprawidłowe dane oceny";
            var exceptionMessage = "Wykryto błędy:";
            bool isInvalid = false;

            if (rating.Rating <= 0)
            {
                isInvalid = true;
                exceptionMessage += "\nPole oceny nie może być mniejsze lub równe 0";
            }
            else if (rating.Rating > 10)
            {
                isInvalid = true;
                exceptionMessage += "\nPole oceny nie może być większe od 10";
            }

            if (isInvalid)
                throw new ValidationException(exceptionTitle, exceptionMessage);
        }
    }
}
