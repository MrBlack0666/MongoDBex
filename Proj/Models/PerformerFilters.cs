using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class PerformerFilters
    {
        public int Page { get; set; }
        public string Performer { get; set; }
        public string Country { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public SortPerformersEnum SortBy { get; set; }
        public bool IsDescending { get; set; }
    }
}
