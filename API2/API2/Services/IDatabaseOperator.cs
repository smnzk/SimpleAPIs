using PrzykladKolokwium.Models;

namespace PrzykladKolokwium.Services
{
    public interface IDatabaseOperator
    {
        Task<IList<Prescription>> GetPrescriptions(string nazwisko);
        Task<IList<Prescription>> GetPrescriptions();

        Task<Information> PostMedicaments(IList<Medicament> medicaments, int idPrescription);
    }
}
