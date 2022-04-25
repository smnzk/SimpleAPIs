namespace PrzykladKolokwium.Models
{
    public class Prescription
    {
        public int IdPrescrption { get; set; }
        public DateTime Date { get; set; } 
        public DateTime DueDate { get; set; } 
        public int IdPatient { get; set; }
        public int IdDoctor { get; set; }   
    }
}
