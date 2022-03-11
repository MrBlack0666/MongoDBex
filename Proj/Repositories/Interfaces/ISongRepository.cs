using mongoDB.Models;
using MongoDB.Bson;
using System.Collections.Generic;

namespace mongoDB.Repositories
{
    public interface ISongRepository
    {
        void Add(SongVM song);
        void Delete(ObjectId id);
        void Edit(ObjectId id, SongVM song);
        Song GetById(ObjectId id);
        GetSongsVM GetWithFilters(SongFilters songFilters);
        void DeletePerformerIdInSongs(ObjectId performerId);
        bool AddRatingToSong(ObjectId songId, RatingClass rating);
        bool DeleteRatingFromSong(ObjectId songId, int ratingId);
        List<SongForPerformer> GetSongsForPerformer(ObjectId id);
    }
}