using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public enum SortSongEnum
    {
        [Description("none")]
        NONE,
        [Description("title")]
        TITLE,
        [Description("averageRating")]
        AVERAGERATING,
        [Description("releaseYear")]
        RELEASEYEAR
    }
}
