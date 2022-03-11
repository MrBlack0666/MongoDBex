using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class RatingClass
    {
        [BsonElement("rating")]
        public int Rating { get; set; }
        [BsonElement("review")]
        public string Review { get; set; }

        [BsonElement("ratingId")]
        public int RatingId { get; set; }
    }
}
