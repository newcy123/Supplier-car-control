using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierControlBackend.Contexts;
using SupplierControlBackend.Models;

namespace SupplierControlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierCardController : ControllerBase
    {
        private readonly DbSC_Suppiler_Driver_Lisense _SuppilerDriverLisenseContext;
        private readonly DbAL_vender _venderContext;

        public SupplierCardController(DbSC_Suppiler_Driver_Lisense suppilerDriverLisenseContext,DbAL_vender venderContext)
        {
            _SuppilerDriverLisenseContext = suppilerDriverLisenseContext;
            _venderContext = venderContext;
        }

        [HttpGet]
        [Route("getAllSuppilerCard")]
        public async Task<ActionResult<ScSupplierDriverLisense>> getAll()
        {

            var getSupilerCard = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.ToListAsync();


            return Ok(getSupilerCard);
        }


        [HttpGet]
        [Route("getSuppilerCard/{DriverNbr}")]
        public async Task<ActionResult<ScSupplierDriverLisense>> getSuppilerCardByDriverNbr(string DriverNbr)
        {

            var getSupilerCardByNbr = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x=>x.DriverNbr == DriverNbr)
                .FirstOrDefaultAsync();


            return Ok(getSupilerCardByNbr);
        }



        [HttpGet]
        [Route("getVander")]
        public async Task<ActionResult<AlVendor>> GetAllVender()
        {

            var getAllVender = await _venderContext.AlVendors.Select(x=>x.VenderName).ToListAsync();


            return Ok(getAllVender);
        }

        [HttpGet]
        [Route("getVander/{venderCode}")]
        public async Task<ActionResult<AlVendor>> getVenderByCode(string venderCode)
        {

            var getVenderByCode = await _venderContext.AlVendors.Where(x => x.Vender == venderCode).FirstOrDefaultAsync();


            return Ok(getVenderByCode);
        }





    }
}
