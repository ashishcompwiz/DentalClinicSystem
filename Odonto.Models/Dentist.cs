using System;
using System.Collections.Generic;
using System.Text;

namespace Odonto.Models
{
    public class Dentist : Person
    {
        public string Specialty { get; set; }

        public string CRO { get; set; }

        public Person GetBase()
        {
            return (Person)this;
        }
    }
}
