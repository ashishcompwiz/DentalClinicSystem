using System;
using System.Collections.Generic;
using System.Text;

namespace Odonto.Models
{
    public class User
    {
        public User()
        {
        }

        public int ID { get; set; }

        public bool Active { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Type { get; set; }
    }
}
