using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public class GetSongsVM
    {
        public int AllPages { get; set; }
        public List<Song> Songs { get; set; }
    }
}
