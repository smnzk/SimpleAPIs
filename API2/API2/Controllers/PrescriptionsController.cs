using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrzykladKolokwium.Models;
using PrzykladKolokwium.Services;

namespace PrzykladKolokwium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {

        private readonly IDatabaseOperator _databaseOperator;

        public PrescriptionsController(IDatabaseOperator databaseOperator)
        {
            _databaseOperator = databaseOperator;
        }

        [HttpPost("IdPresc")] 
        public async Task<Information> PostPresctiptions(IList<Medicament> medicaments, int IdPresc)
        {
            Information information = await _databaseOperator.PostMedicaments(medicaments, IdPresc);
            return information;
        }

        [HttpGet]
        public async Task<IList<Prescription>> GetPrescriptions(string? nazwisko)
        {
            IList<Prescription> prescrptions = new List<Prescription>();
            if (nazwisko == null)
            {
                prescrptions = await _databaseOperator.GetPrescriptions();
            } else
            {
                prescrptions = await _databaseOperator.GetPrescriptions(nazwisko);
            }
            return prescrptions;
        } 

    }
}
