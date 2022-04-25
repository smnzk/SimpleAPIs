using APBD5.Models;
using APBD5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private readonly IDatabaseOperator _databaseOperator;

        public Warehouses2Controller(IDatabaseOperator databaseOperator)
        {
            _databaseOperator = databaseOperator;
        }


        [HttpPost]
        public async Task<IActionResult> Post(Entry entry)
        {

        }
    }
}
