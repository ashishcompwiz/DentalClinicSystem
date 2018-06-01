namespace Odonto.Models
{
    public class Procedure
    {
        public int ID { get; set; }

        public int ClinicID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Observation { get; set; }

        public decimal Value { get; set; }
    }
}
