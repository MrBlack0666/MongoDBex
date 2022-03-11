using mongoDB.Models;
using MongoDB.Bson;
using System.Collections.Generic;

namespace mongoDB.Repositories
{
    public interface IPerformerRepository
    {
        void Add(Performer performer);
        bool Delete(ObjectId id);
        void Edit(ObjectId id, Performer performer);
        List<PerformerVM> GetAll();
        Performer GetById(ObjectId id);
        GetPerformersVM GetWithFilters(PerformerFilters performerFilters);
    }
}