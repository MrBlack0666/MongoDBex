using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class GetPerformersVM
    {
        public int AllPages { get; set; }
        public List<Performer> Performers { get; set; }
    }
}
