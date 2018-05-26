using System;
using System.ComponentModel.DataAnnotations;

namespace Odonto.Models
{
    public class Person
    {
        public Person()
        {
        }

        public int ID { get; set; }

        public int ClinicID { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public char Sex { get; set; }

        public string CEP { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string City { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string State { get; set; } = "RS";

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Phone { get; set; }

        public string Phone2 { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime BirthDate { get; set; } = DateTime.Now;

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        /* Methods */
        public string SexString
        {
            get 
            {
                if (Sex.ToString().ToUpper() == "F")
                    return "Feminino";
                else if (Sex.ToString().ToUpper() == "M")
                    return "Masculino";
                else return string.Empty;
            }
        }

        public string AddressString
        {
            get {
                return Address + ", " + Number + " - " + City + "/" + State;
            }
        }
    }
}