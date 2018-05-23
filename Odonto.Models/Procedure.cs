using System;
using System.Collections.Generic;
using System.Text;

namespace Odonto.Models
{
    public class Procedure
    {
        public int ClinicID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Observation { get; set; }

        public decimal Value { get; set; }
    }
}
