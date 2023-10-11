using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using SupplierControlBackend.Contexts;
using SupplierControlBackend.Models;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace SupplierControlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierCardScanController : Controller
    {
        private readonly DbSC_Suppiler_Driver_Lisense _SuppilerDriverLisenseContext;
        private readonly DbSC_SC_Suplier_In_Out _SuppilerDriverInOutContext;

        public SupplierCardScanController(DbSC_Suppiler_Driver_Lisense suppilerDriverLisenseContext, DbSC_SC_Suplier_In_Out suppilerDriverInOutContext)
        {
            _SuppilerDriverLisenseContext = suppilerDriverLisenseContext;
            _SuppilerDriverInOutContext = suppilerDriverInOutContext;
        }
        //ดึงข้อมูลคนขับรถ
        [HttpGet]
        [Route("getAllSuppilerScanCard")]
        public async Task<IActionResult> getAll()
        {
            DateTime start_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00);
            DateTime end_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 45, 00, 00); ;


            DateTime start_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 00, 00);
            DateTime end_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00); ;


            int currentDay = DateTime.Now.Day;

            // หน้า Attandance ดึงข้ัอมูลมาแสดง
            try { 
            #region// Attendance Table 
            var getAttendance = await _SuppilerDriverInOutContext.ScSuplierInOuts.ToListAsync();
            var getAttendanceDataTable = (

                from attendace in getAttendance
                join company in _SuppilerDriverLisenseContext.ScSupplierDriverLisenses
                on attendace.VenderCard equals company.DriverNbr
                select new
                {   
                    deliverlyImage = company.DriverPicture,
                    deliverlyCompany = company.VenderName,
                    deliverlyRound = attendace.DeliveryRound,
                    deliverlyName = company.Fname + " " + company.Surn,
                    deliverlyNbr = company.DriverNbr,
                    deliverlyExpire = company.DriverLicenceExpire,
                    deliverlyDate = attendace.DeliveryDate,
                    deliverlyShift = attendace.DeliveryShift,
                    deliverlyEntryTime = attendace.EntryTime,
                    deliverlyLeaveTime = attendace.LeaveTime,
                    //deliverlyTotalEntryTime = (attendace.LeaveTime - attendace.EntryTime)
                    deliverlyTotalEntryTime = ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes / 60).ToString() + ":" + ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes % 60).ToString(),
                    deliverlyStatusCal = (attendace.LeaveTime - attendace.EntryTime).Value.Hours < 0 ? false : true


                }).Where(x=>x.deliverlyDate.Day == currentDay ).ToList();
                return Ok(new
                {
                    getAttendanceDataTable = getAttendanceDataTable,

                });
                #endregion
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }


          
        }
        [HttpPost]
        [Route("SaveSupplierScanCard")]
        public async Task<IActionResult> addSupplierInOut(ScanCard sc)
        {
            try
            {
                string vendercode = sc.Drivercard_Nbr.Split('_')[1].ToString();
                int countCard = 0;
                var getAttendance = await _SuppilerDriverInOutContext.ScSuplierInOuts.ToListAsync();
                
                // เช็คว่าบริษัทนี้ดขเามาหรือยัง
                var checkVendercode = (

                    from attendace in getAttendance
                    join company in _SuppilerDriverLisenseContext.ScSupplierDriverLisenses
                    on attendace.VenderCard equals company.DriverNbr
                    select new
                    {
                        deliverlyVenderCode = company.VenderCode,
                        deliverlyDate = attendace.DeliveryDate,
                        deliverlyLeaveTime = attendace.LeaveTime,
                        deliverlyRound = attendace.DeliveryRound
                       

                    }).Where(x => x.deliverlyVenderCode == vendercode && x.deliverlyDate.Day == DateTime.Now.Day).ToList();



                // ถ้าเคยมีบริษัทนั้นเข้ามาแล้ว
                if (checkVendercode.Count > 0)
                {
                    //เช็คบัตรและวันที่ปัจจุบัน
                    var checkgetAttendance = await _SuppilerDriverInOutContext.ScSuplierInOuts.Where(x => x.VenderCard == sc.Drivercard_Nbr && x.DeliveryDate.Day == DateTime.Now.Day).ToListAsync();
                    // เช็ควันที่ว่าบริษัทนี้มีการออกไปหรือยัง
                    var supplierInOutTop1 = checkgetAttendance.Where(x => Convert.ToDateTime(x.LeaveTime).ToString("HH:mm:ss") == "00:00:00").OrderByDescending(x => x.DeliveryRound).Take(1).ToList();
                    countCard = checkVendercode.Count();
                    
                    // ถ้าออกไปแล้ว ให้ update เวลาออก และ คนที่ยิงสแกน
                    if (supplierInOutTop1.Count > 0)
                    {
                        supplierInOutTop1.FirstOrDefault().LeaveTime = DateTime.Now;
                        supplierInOutTop1.FirstOrDefault().UpdateDate = DateTime.Now;
                        supplierInOutTop1.FirstOrDefault().UpdateBy = sc.Empcode;

                        await _SuppilerDriverInOutContext.SaveChangesAsync();
                        return Ok(new { status = true });
                    }
                    // ถ้ายังไม่ออกให้สร้างบัตรใบนั้นใหม่ เพราะถือว่า เป็นคนของบริษัทเข้ามา
                    else
                    {
                        var checkVenderCard = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x => x.DriverNbr == sc.Drivercard_Nbr && x.DriverLicenceExpire.Value.Date >= DateTime.Now.Date).ToListAsync();
                        if (checkVenderCard.Count > 0)
                        {
                            ScSuplierInOut sc_inout = new ScSuplierInOut();
                            sc_inout.DeliveryDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                            sc_inout.DeliveryRound = checkVendercode.Count + 1;
                            sc_inout.DeliveryShift = "D";
                            sc_inout.VenderCard = sc.Drivercard_Nbr;
                            sc_inout.EntryTime = DateTime.Now;
                            sc_inout.LeaveTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                            sc_inout.CreateBy = sc.Empcode;
                            sc_inout.CreateDate = DateTime.Now;
                            sc_inout.UpdateDate = DateTime.Now;
                            sc_inout.UpdateBy = sc.Empcode;

                            _SuppilerDriverInOutContext.ScSuplierInOuts.Add(sc_inout);
                            await _SuppilerDriverInOutContext.SaveChangesAsync();
                            return Ok(new { status = true });
                        }
                        else
                        {
                            return Ok(new { status = false });

                        }

                    }
             
                }
                //บริษัทเข้ามาครั้งแรก
                else
                {
                    var checkVenderCard = await _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Where(x => x.DriverNbr == sc.Drivercard_Nbr && x.DriverLicenceExpire.Value.Date >= DateTime.Now.Date).ToListAsync();
                    if(checkVenderCard.Count > 0) {
                        ScSuplierInOut sc_inout = new ScSuplierInOut();
                        sc_inout.DeliveryDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        sc_inout.DeliveryRound = countCard + 1;
                        sc_inout.DeliveryShift = "D";
                        sc_inout.VenderCard = sc.Drivercard_Nbr;
                        sc_inout.EntryTime = DateTime.Now;
                        sc_inout.LeaveTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                        sc_inout.CreateBy = sc.Empcode;
                        sc_inout.CreateDate = DateTime.Now;
                        sc_inout.UpdateDate = DateTime.Now;
                        sc_inout.UpdateBy = sc.Empcode;

                        _SuppilerDriverInOutContext.ScSuplierInOuts.Add(sc_inout);
                        await _SuppilerDriverInOutContext.SaveChangesAsync();
                        return Ok(new { status = true });
                    }
                    else
                    {
                        return Ok(new { status = false });

                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }





        }

    }
}
