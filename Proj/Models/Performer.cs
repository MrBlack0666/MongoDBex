using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class Performer
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("nickname")]
        public string Nickname { get; set; }
        [BsonElement("firstName")]
        public string FirstName { get; set; }
        [BsonElement("surname")]
        public string Surname { get; set; }
        [BsonElement("originCountry")]
        public string OriginCountry { get; set; }
        [BsonElement("birthDate")]
        public DateTime BirthDate { get; set; }
    }
}
