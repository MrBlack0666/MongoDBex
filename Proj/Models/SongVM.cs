using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class SongVM
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public double AverageRating { get; set; }
        public string Lyrics { get; set; }
        public List<string> Albums { get; set; }
        public List<RatingClass> Ratings { get; set; }
        public List<ObjectId> Performers { get; set; }
        public long LastRatingId { get; set; }
    }
}
