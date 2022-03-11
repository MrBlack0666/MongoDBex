using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class Song
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public double AverageRating { get; set; }
        public string Lyrics { get; set; }
        public List<string> Albums { get; set; }
        public List<RatingClass> Ratings { get; set; }
        public List<Performer> Performers { get; set; }
        public long LastRatingId { get; set; }

        public string AvgRating
        {
            get
            {
                var temp = Math.Round(AverageRating, 2).ToString();

                if (temp.IndexOf(',') < 0)
                {
                    temp += ",00";
                }
                else if (temp.IndexOf(',') == temp.Length - 2)
                {
                    temp += "0";
                }

                return temp;
            }
        }

        public string PerformersString
        {
            get
            {
                var temp = "";

                if(Performers != null)
                {
                    foreach(var performer in Performers)
                    {
                        temp += performer.Nickname;

                        if(performer != Performers.Last())
                        {
                            temp += ", ";
                        }
                    }
                }

                return temp;
            }
        }
    }
}
