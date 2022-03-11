using MongoDB.Bson;
using mongoDB.Models;
using System.Collections.Generic;

namespace mongoDB.Services
{
    public interface ISongService
    {
        //bool AddPerformer(Performer performer);
        bool AddRatingToSong(ObjectId songId, RatingClass rating);
        bool AddSong(SongVM song);
        //bool DeletePerformer(ObjectId id);
        bool DeleteRatingFromSong(ObjectId songId, int ratingId);
        bool DeleteSong(ObjectId id);
        //bool EditPerformer(ObjectId id, Performer performer);
        bool EditSong(ObjectId id, SongVM song);
        //List<PerformerVM> GetAllPerformers();
        //GetPerformersVM GetPerformersWithFilter(PerformerFilters performerFilters);
        Song GetSong(ObjectId id);
        List<SongForPerformer> GetSongsForPerformer(ObjectId id);
        GetSongsVM GetSongsWithFilter(SongFilters songFilters);
        void DeletePerformerIdInSongs(ObjectId performerId);
    }
}