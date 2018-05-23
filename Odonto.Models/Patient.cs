using System;
using System.Collections.Generic;
using System.Text;

namespace Odonto.Models
{
    public class Patient : Person
    {
        public string Profession { get; set; }

        public Person GetBase()
        {
            return (Person)this;
        }
    }
}
