namespace Odonto.Models
{
    public class PatientRecordDisease
    {
        public PatientRecordDisease(int patientRecordID, int diseaseID, string description)
        {
            PatientRecordID = patientRecordID;
            DiseaseID = diseaseID;
            Description = description;
        }

        public PatientRecordDisease()
        {}

        public int ID;

        public int PatientRecordID;

        public int DiseaseID;

        public string Description;

        /* Helper */
        public string DiseaseLabel { get; set; }
    }
}
