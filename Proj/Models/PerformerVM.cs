using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class PerformerVM
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("nickname")]
        public string Nickname { get; set; }
    }
}
