using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class SongFilters
    {
        public int Page { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Performer { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public int AvgFrom { get; set; }
        public int AvgTo { get; set; }
        public SortSongEnum SortBy { get; set; }
        public bool IsDescending { get; set; }
    }
}
