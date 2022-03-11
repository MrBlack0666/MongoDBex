using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Models
{
    public enum SortPerformersEnum
    {
        [Description("none")]
        NONE,
        [Description("nickname")]
        NICKNAME,
        [Description("firstName")]
        FIRSTNAME,
        [Description("surname")]
        SURNAME,
        [Description("birthDate")]
        BIRTHDATE,
        [Description("originCountry")]
        ORIGINCOUNTRY
    }
}
