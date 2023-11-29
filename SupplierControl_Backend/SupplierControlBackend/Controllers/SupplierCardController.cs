using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SupplierControlBackend.Contexts;
using SupplierControlBackend.Models;
using System.IO;

namespace SupplierControlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierCardController : ControllerBase
    {
        private readonly DbSC_Suppiler_Driver_Lisense _SuppilerDriverLisenseContext;
        private readonly DbAL_vender _venderContext;

        public SupplierCardController(DbSC_Suppiler_Driver_Lisense suppilerDriverLisenseContext, DbAL_vender venderContext)
        {
            _SuppilerDriverLisenseContext = suppilerDriverLisenseContext;
            _venderContext = venderContext;
        }

        //ดึงข้อมูลคนขับรถ
        [HttpGet]
        [Route("getAllSuppilerCard")]
        public async Task<ActionResult<ScSupplierDriverLisense>> getAll()
        {

            var getSupilerCard = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses
                .Select(x => new { x.TcTel, x.Fname, x.Surn, x.DriverLicenceExpire, x.DriverNbr, x.VenderName, x.DriverStatus , x.DriverPicture,x.CreateDate })
                .Where(x => x.DriverStatus == "1")
                .ToListAsync();


            return Ok(getSupilerCard);
        }

        // ค้นหาข้อมูลบัตรคนขัขรถจาก เลขบัตร
        [HttpGet]
        [Route("getSuppilerCard/{DriverNbr}")]
        public async Task<ActionResult<ScSupplierDriverLisense>> getSuppilerCardByDriverNbr(string DriverNbr)
        {

            var getSupilerCardByNbr = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x => x.DriverNbr == DriverNbr)
                .FirstOrDefaultAsync();


            return Ok(getSupilerCardByNbr);
        }


        // ดึงรายชื่อบริษัท vender
        [HttpGet]
        [Route("getVander")]
        public async Task<ActionResult<AlVendor>> GetAllVender()
        {

            var getAllVender = await _venderContext.AlVendors.Select(x => new { x.Vender, x.VenderName }).ToListAsync();



            return Ok(getAllVender);
        }

        // ค้นหารายชื่อ vender จาก vendercode
        [HttpGet]
        [Route("getVander/{venderCode}")]
        public async Task<ActionResult<AlVendor>> getVenderByCode(string venderCode)
        {

            var getVenderByCode = await _venderContext.AlVendors.Where(x => x.Vender == venderCode).FirstOrDefaultAsync();



            return Ok(getVenderByCode);
        }


        // ดึงข้อมูล dataCardByNbrnumber ปริ้นบัตรคนชับรถ
        [HttpPost]
        [Route("getdataCardByNbr")]
        public async Task<ActionResult> getdataCardByNbr(string[] nbr)
        {
            List<ScSupplierDriverLisense> cardNames = new List<ScSupplierDriverLisense>();
            foreach(var card in nbr)
            {
                var getdataNbrByCode = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x => x.DriverNbr == card).FirstOrDefaultAsync();
                cardNames.Add(getdataNbrByCode);
            }




            return Ok(cardNames);
        }


        [HttpGet]
        [Route("getGenerateDriverCard/{vendercode}")]
        public async Task<ActionResult> getGenerateDriverCard(string vendercode)
        {

            string drvCard = "";

            int countDriverCard = _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x=>x.VenderCode == vendercode).Count();
            
            if(countDriverCard > 0)
            {
                string getGenerateDriverCard = _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x => x.VenderCode.Contains(vendercode)).OrderByDescending(x => x.DriverNbr).FirstOrDefault().DriverNbr;
                drvCard = "DRV" + "_" + vendercode.ToUpper() + "_" + (Convert.ToInt32(getGenerateDriverCard.Split('_')[2]) + 1).ToString("D5");

            }
            else
            {
                drvCard = "DRV" + "_" + vendercode.ToUpper() + "_" + "00001";


            }


            return Ok(drvCard);
        }





        [HttpPut]
        [Route("EditSupplierCard/{nbr}")]
        public async Task<ActionResult> EditSupplierCard(string nbr,[FromForm] ScSupplierDriverLisense sc)
        {


            if (sc.DriverNbr != nbr)
            {
                return BadRequest();
            }

            var getSupilerCardByNbr = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x => x.DriverNbr == nbr).FirstOrDefaultAsync();

            try
            {
                if (getSupilerCardByNbr != null)
                {


                    getSupilerCardByNbr.Fname = sc.Fname;
                    getSupilerCardByNbr.Surn = sc.Surn;
                    getSupilerCardByNbr.TcTel = sc.TcTel;
                    getSupilerCardByNbr.DriverLicence = sc.DriverLicence;
                    getSupilerCardByNbr.DriverPicture = sc.FormFile != null ? await UploadImage(sc.RawImage,sc.DriverPicture, sc.FormFile,"UPDATE") : sc.DriverPicture;
                    //getSupilerCardByNbr.DriverPicture = sc.DriverPicture;

                    getSupilerCardByNbr.DriverLicenceExpire = sc.DriverLicenceExpire;
                    getSupilerCardByNbr.VehicleNbr = sc.VehicleNbr;
                    getSupilerCardByNbr.UpdateDate = DateTime.Now;




                    if (sc == null)
                        return BadRequest();

                    await _SuppilerDriverLisenseContext.SaveChangesAsync();
                    return Ok(new { status = true });
                }
                else
                {
                    return NotFound($"Employee with Id = {sc.DriverNbr} not found");

                }

            }



            catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error not save supplier record");
            }




        }



        [HttpPost]
        [Route("SaveSupplierCard")]
        public async Task<ActionResult> SaveSupplierCard([FromForm] ScSupplierDriverLisense sc )
        {   
                 
            try
            {
                if (sc == null)
                    return BadRequest();
                sc.DriverPicture = await UploadImage(sc.RawImage,sc.DriverPicture, sc.FormFile,"INSERT");
                //sc.DriverPicture = "test";
                sc.CreateDate = DateTime.Now;
                sc.UpdateDate = DateTime.Now;
                sc.IssueDate = DateTime.Now;
                sc.DriverStatus = "1";

               
                _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Add(sc);
                await _SuppilerDriverLisenseContext.SaveChangesAsync();

                return Ok(new {status = true});
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error not save supplier record");
            }
        }


        //[HttpPost]
        //[Route("removeSupplierCard/{nbr}")]
        //public async Task<ActionResult> removeSupplierCard(string nbr)
        //{
        //    try
        //    {

        //        var supplier_status = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(s => s.DriverNbr == nbr).FirstOrDefaultAsync();


        //        if (supplier_status != null)
        //        {
        //            supplier_status.DriverStatus = "2";
        //            await _SuppilerDriverLisenseContext.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            return NotFound($"Employee with Id = {nbr} not found");

        //        }


        //        return Ok(new { status = true });
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error updating data");
        //    }
        //}

        [HttpDelete]
        [Route("removeSupplierCard/{nbr}")]
        public async Task<ActionResult> removeSupplierCard2(string nbr)
        {
            try
            {
                var findCard = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x => x.DriverNbr == nbr).FirstOrDefaultAsync();
                if(findCard != null)
                {
                    _SuppilerDriverLisenseContext.Remove(findCard);
                    _SuppilerDriverLisenseContext.SaveChangesAsync();
                    
                   string oldPathFileDelete = Path.Combine(@"\\192.168.226.8\g$\www\SupplierControlWeb\build\assets\img\supplier", findCard.DriverPicture);
                   //string oldPathFileDelete = Path.Combine(@"D:\Project\2023\SupplierControl\SupplierControl_Frontend\suppiler-frontend\public\assets\img\supplier", findCard.DriverPicture);

                    if (System.IO.File.Exists(oldPathFileDelete))
                    {
                        System.IO.File.Delete(oldPathFileDelete);
                    }

              
                    return Ok(new { status = true });

                }

                return NotFound($"Employee with Id = {nbr} not found");

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error updating data");
            }
        
        }

            //[HttpPost]
            //[Route("UploadImage")]
            //public async Task<ActionResult>SaveImage([FromForm] FileUpload fileUpload)
            //{


            //    try
            //    {   
            //        //file = "supplier_" + "" +"." + fileUpload;
            //        string path = Path.Combine(@"D:\Project\2023\SupplierControl\SupplierControl_Frontend\suppiler-frontend\public\assets\img\supplier", fileUpload.FileName);
            //        string fileName = Path.GetFileName(path);
            //        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            //        //if (!Directory.Exists(path))
            //        //{
            //        //    Directory.CreateDirectory(path);
            //        //}


            //        using (Stream stream = new FileStream(path, FileMode.Create))
            //        {
            //             fileUpload.FormFile.CopyToAsync(stream);
            //        }
            //        return Ok(new { sts = true });


            //    }
            //    catch (Exception ex)
            //    {
            //        return StatusCode(StatusCodes.Status500InternalServerError,
            //          "Error not save File record");

            //    }


            //}




        [NonAction]
        public string generateFileName(string fileNames)
        {
            return ("DRV_LINCE_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + "_" + randomNumberInRange(1, 100) + "." +
                fileNames.Split(".")[1]
           );
        }

        [NonAction]
        public int randomNumberInRange(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max) * (max - min) + min;
        }
        [NonAction]
        public async Task<string> UploadImage(string rawImage, string fileName, IFormFile imageFiles,string statusAction)
        {
            string imageNameFormat = "";
            try
            {
                imageNameFormat = generateFileName(fileName);
                //string newPathFile = Path.Combine(@"D:\Project\2023\SupplierControl\SupplierControl_Frontend\suppiler-frontend\public\assets\img\supplier", imageNameFormat);
                string newPathFile = Path.Combine(@"\\192.168.226.8\g$\www\SupplierControlWeb\build\assets\img\supplier", imageNameFormat);

                switch (statusAction){

                    case "INSERT":

                        using (Stream stream = new FileStream(newPathFile, FileMode.Create))
                        {
                             await imageFiles.CopyToAsync(stream);

                        }
                        break;
                    
                    case "UPDATE":
                        //string oldPathFileUpdate = Path.Combine(@"D:\Project\2023\SupplierControl\SupplierControl_Frontend\suppiler-frontend\public\assets\img\supplier", rawImage);
                        string oldPathFileUpdate = Path.Combine(@"\\192.168.226.8\g$\www\SupplierControlWeb\build\assets\img\supplier", rawImage);

                        if (System.IO.File.Exists(oldPathFileUpdate))
                        {
                            System.IO.File.Delete(oldPathFileUpdate);
                        }
                        using (Stream stream = new FileStream(newPathFile, FileMode.Create))
                        {
                           await imageFiles.CopyToAsync(stream);

                        }
                        break;

                

                }

               
            }
            catch (Exception ex)
            {


            }

            return imageNameFormat;
        }





    }
}
