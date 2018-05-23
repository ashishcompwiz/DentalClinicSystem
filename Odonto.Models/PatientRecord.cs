using System;
using System.Collections.Generic;
using System.Text;

namespace Odonto.Models
{
    public class PatientRecord
    {
        public string ID { get; set; }

        public string MainComplaint { get; set; }

        public string Allergies { get; set; }

        public string Medicines { get; set; }

        public string PsychologicalTreatment { get; set; }

        public string Diagnose { get; set; }

        public List<PatientRecordDisease> Diseases { get; set; }

        public List<PatientRecordProcedure> Procedures { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }
    }
}
