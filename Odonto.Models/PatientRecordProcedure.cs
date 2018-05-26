using System;
using System.ComponentModel.DataAnnotations;

namespace Odonto.Models
{
    public class PatientRecordProcedure
    {
        public int ID { get; set; }

        public int PatientRecordID { get; set; }

        [Required(ErrorMessage = "Infome o procedimento realizado")]
        public int ProcedureID { get; set; }

        [Required(ErrorMessage = "Informe o Dentista que realizou o procedimento")]
        public int DentistID { get; set; }

        [Required(ErrorMessage = "Informe a data da realização do procedimento")]
        public DateTime Date { get; set; } = DateTime.Now;

        public string Description { get; set; }

        [Required(ErrorMessage = "Informe o valor")]
        public decimal Value { get; set; }

        /* Helpers */
        public string ProcedureLabel { get; set; }

        public string DentistName { get; set; }
    }
}
