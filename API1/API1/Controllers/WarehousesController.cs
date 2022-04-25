using APBD5.Models;
using APBD5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {

        private readonly IDatabaseOperator _databaseOperator;

        public WarehousesController(IDatabaseOperator databaseOperator)
        {
            _databaseOperator = databaseOperator;   
        }

        [HttpPost]
        public async Task<IActionResult> PostEntryAsync(Entry entry)
        {
            Information information = await _databaseOperator.Post(entry);
            if (information.done)
            {
                return Ok(information.info);
            }
            else
            {
                return NotFound(information.info);
            }
           
        }


    }
}
