namespace Odonto.Models
{
    public class Administrator : Person
    {
        public string Position { get; set; }

        public Person GetBase()
        {
            return (Person)this;
        }
    }
}
