using System;
using System.Collections.Generic;
using System.Text;

namespace Odonto.Models
{
    public class PatientRecord
    {
        public int PatientID { get; set; }

        public string MainComplaint { get; set; }

        public string Alterations { get; set; }

        public string Medicines { get; set; }

        public bool PsychologicalTreatment { get; set; }

        public string Diagnose { get; set; }

        public List<PatientRecordDisease> Diseases { get; set; } = new List<PatientRecordDisease>();

        public List<PatientRecordProcedure> Procedures { get; set; } = new List<PatientRecordProcedure>();

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }
    }
}
