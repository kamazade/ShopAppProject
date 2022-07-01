using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopapp.entity
{
    public class Adress
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        
        public string City { get; set; }
        public string AdressDetail { get; set; }
        
    }
}