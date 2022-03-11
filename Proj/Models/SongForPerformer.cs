using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class SongForPerformer
    {
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public double AverageRating { get; set; }

        public string AvgRating
        {
            get
            {
                var temp = Math.Round(AverageRating, 2).ToString();

                if(temp.IndexOf(',') < 0)
                {
                    temp += ",00";
                }
                else if(temp.IndexOf(',') == temp.Length - 2)
                {
                    temp += "0";
                }

                return temp;
            }
        }
    }
}
