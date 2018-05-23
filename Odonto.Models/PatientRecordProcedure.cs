using System;
using System.Collections.Generic;
using System.Text;

namespace Odonto.Models
{
    public class PatientRecordProcedure
    {
        public int ID { get; set; }

        public int PatientRecordID { get; set; }

        public int ProcedureID { get; set; }

        public int DentistID { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public decimal Value { get; set; }
    }
}
