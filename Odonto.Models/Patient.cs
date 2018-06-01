namespace Odonto.Models
{
    public class Patient : Person
    {
        public string Profession { get; set; }

        public PatientRecord Record { get; set; }

        public string Email { get; set; }

        public Person GetBase()
        {
            return (Person)this;
        }
    }
}
