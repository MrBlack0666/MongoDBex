using mongoDB.Aspects;
using mongoDB.Exceptions;
using mongoDB.Models;
using mongoDB.Repositories;
using mongoDB.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Services
{
    [PerformersLogger(AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class PerformerService : IPerformerService
    {
        private readonly int ELEMENTS_IN_PAGE = 10;

        //private static SongsDatabase songsDatabase = null;

        //private IMongoClient dbClient;
        //private IMongoDatabase db;
        //private IMongoCollection<BsonDocument> performersCollection;
        
        private ISongService songService;
        private IPerformerRepository _performerRepository;

        public PerformerService(ISongService songService, IPerformerRepository _performerRepository)
        {
            //dbClient = new MongoClient("mongodb://localhost:27017");
            //db = dbClient.GetDatabase("songsDB");
            //performersCollection = db.GetCollection<BsonDocument>("performers");

            this.songService = songService;
            this._performerRepository = _performerRepository;
        }

        public PerformerService(ISongService songService, IPerformerRepository _performerRepository, string database)
        {
            //dbClient = new MongoClient("mongodb://localhost:27017");
            //db = dbClient.GetDatabase(database);

            //performersCollection = db.GetCollection<BsonDocument>("performers");

            this.songService = songService;
            this._performerRepository = _performerRepository;
        }

        public bool AddPerformer(Performer performer)
        {
            ValidatePerformer(performer);

            _performerRepository.Add(performer);

            return true;
        }

        public bool EditPerformer(ObjectId id, Performer performer)
        {
            ValidatePerformer(performer);

            _performerRepository.Edit(id, performer);

            return true;
        }

        public bool DeletePerformer(ObjectId id)
        {
            var result = _performerRepository.Delete(id);

            if (result)
            {
                songService.DeletePerformerIdInSongs(id);
            }

            return result;
        }

        public List<PerformerVM> GetAllPerformers()
        {
            return _performerRepository.GetAll();
        }

        public GetPerformersVM GetPerformersWithFilter(PerformerFilters performerFilters)
        {
            return _performerRepository.GetWithFilters(performerFilters);
        }

        public List<SongForPerformer> GetSongsForPerformer(ObjectId id)
        {
            return songService.GetSongsForPerformer(id);
        }

        private void ValidatePerformer(Performer performer)
        {
            var exceptionTitle = "Nieprawidłowe dane wykonawcy";
            var exceptionMessage = "Wykryto błędy:";
            bool isInvalid = false;

            if (string.IsNullOrWhiteSpace(performer.Nickname))
            {
                isInvalid = true;
                exceptionMessage += "\nPole pseudonimu wykonawcy nie może być puste";
            }

            if (string.IsNullOrWhiteSpace(performer.FirstName))
            {
                isInvalid = true;
                exceptionMessage += "\nPole imienia wykonawcy nie może być puste";
            }

            if (string.IsNullOrWhiteSpace(performer.Surname))
            {
                isInvalid = true;
                exceptionMessage += "\nPole nazwiska wykonawcy nie może być puste";
            }

            if (string.IsNullOrWhiteSpace(performer.OriginCountry))
            {
                isInvalid = true;
                exceptionMessage += "\nPole kraju pochodzenia wykonawcy nie może być puste";
            }

            if (performer.BirthDate > DateTime.Now)
            {
                isInvalid = true;
                exceptionMessage += "\nData urodzenia wykonawcy musi być mniejsza od daty bierzącej";
            }

            if (isInvalid)
                throw new ValidationException(exceptionTitle, exceptionMessage);
        }

    }
}
