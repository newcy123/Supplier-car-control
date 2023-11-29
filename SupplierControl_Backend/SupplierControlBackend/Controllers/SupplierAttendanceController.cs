using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierControlBackend.Contexts;
using SupplierControlBackend.Models;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SupplierControlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierAttendanceController : ControllerBase
    {
        private readonly DbSC_Suppiler_Driver_Lisense _SuppilerDriverLisenseContext;
        private readonly DbSC_SC_Suplier_In_Out _SuppilerDriverInOutContext;

        public SupplierAttendanceController(DbSC_Suppiler_Driver_Lisense suppilerDriverLisenseContext, DbSC_SC_Suplier_In_Out suppilerDriverInOutContext)
        {
            _SuppilerDriverLisenseContext = suppilerDriverLisenseContext;
            _SuppilerDriverInOutContext = suppilerDriverInOutContext;
        }


        //ดึงข้อมูลคนขับรถ
        [HttpGet]
        [Route("getAllSuppilerInOut")]
        public async Task<IActionResult> getAll()
        {
            DateTime start_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00);
            DateTime end_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 45, 00, 00); ;


            DateTime start_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 00, 00);
            DateTime end_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00); ;

            // หน้า Attandance ดึงข้ัอมูลมาแสดง

            #region// Attendance Table 
            var getAttendance = await _SuppilerDriverInOutContext.ScSuplierInOuts.ToListAsync();
            var getAttendanceDataTable = (

                from attendace in getAttendance
                join company in _SuppilerDriverLisenseContext.ScSupplierDriverLisenses
                on attendace.VenderCard equals company.DriverNbr
                select new
                {
                    deliverlyCompany = company.VenderName,
                    deliverlyName = company.Fname + " " + company.Surn,
                    deliverlyNbr = company.DriverNbr,
                    deliverlyRound = attendace.DeliveryRound,
                    deliverlyDate = attendace.DeliveryDate,
                    deliverlyShift = attendace.DeliveryShift,
                    deliverlyEntryTime = attendace.EntryTime,
                    deliverlyLeaveTime = attendace.LeaveTime,
                    //deliverlyTotalEntryTime = (attendace.LeaveTime - attendace.EntryTime)
                    deliverlyTotalEntryTime = ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes / 60).ToString() + ":" + ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes % 60).ToString(),
                    deliverlyStatusCal = (attendace.LeaveTime - attendace.EntryTime).Value.Hours < 0 ? false : true

                }).OrderByDescending(x => Convert.ToDateTime(x.deliverlyLeaveTime).ToString("hh:mm:ss")).ToList();
            #endregion


            //#region// supplier ที่อยู่นานที่สุด
            //var findMaximumVenderEntryTime = getAttendanceDataTable.GroupBy(x => new { x.deliverlyNbr, x.deliverlyCompany, x.deliverlyShift })
            //                                .Select(g => new
            //                                {
            //                                    deliverlyMaximumCompanyTotal = g.FirstOrDefault().deliverlyCompany,
            //                                    deliverlyMaximumTotalEntryTime = g.Sum(x => (x.deliverlyTotalEntryTime.Value.TotalHours))
            //                                }).OrderByDescending(x=>x.deliverlyMaximumTotalEntryTime).Take(1).FirstOrDefault();

            //#endregion



            //#region// จำนวน supplier ที่เข้ามาทำงานในวันนี้
            //var countSupplier = getAttendanceDataTable.Where(x => x.deliverlyEntryTime >= start_shift_day && x.deliverlyLeaveTime <= end_shift_day).Count();

            //#endregion

            //#region// จำนวนชั่วโมง supplier ที่เข้ามาทำงานในวันนี้  
            //var countEntryTime = getAttendanceDataTable.Where(x => x.deliverlyEntryTime >= start_shift_day && x.deliverlyLeaveTime <= end_shift_day).Sum(x=>x.deliverlyTotalEntryTime.Value.TotalHours);
            //#endregion


            //#region// จำนวน supplier ที่เข้ามาทำงานในวันนี้
            ////var countSupplier = getAttendanceDataTable.Where(x => x.deliverlyDate.Month == currentOfMonth).Count();

            //var countSupplier = getAttendanceDataTable.Where(x => x.deliverlyDate.Day == start_shift_day.Day).Count();


            //#endregion

            //#region// จำนวนชั่วโมง supplier ที่เข้ามาทำงานในเดือนนี้ 

            ////var countEntryTimeInMonth = Convert.ToInt32(getAttendanceDataTable.Where(x => x.deliverlyDate.Month == currentOfMonth).Sum(x => x.deliverlyTotalEntryTime.Value.TotalMinutes / 60)).ToString() + ":" +
            ////                     Convert.ToInt32(getAttendanceDataTable.Where(x => x.deliverlyDate.Month == currentOfMonth).Sum(x => x.deliverlyTotalEntryTime.Value.TotalMinutes % 60)).ToString();
            //var countEntryTimeInMonth = new TimeSpan(getAttendanceDataTable.Where(x => x.deliverlyDate.Month == currentOfMonth).Sum(x => x.deliverlyTotalEntryTime.Value.Ticks));

            //#endregion


            //#region// จำนวนชั่วโมง supplier ที่เข้ามาทำงานในวันนี้
            //var t = getAttendanceDataTable.Where(x => x.deliverlyDate.Day == currentOfDay).Sum(x => x.deliverlyTotalEntryTime.Value.Minutes).ToString();
            //var countEntryTimeInDay = getAttendanceDataTable.Where(x => x.deliverlyDate.Day == currentOfDay).Sum(x => x.deliverlyTotalEntryTime.Value.Hours).ToString() + ":" +
            //                     getAttendanceDataTable.Where(x => x.deliverlyDate.Day == currentOfDay).Sum(x => (x.deliverlyTotalEntryTime.Value.TotalHours % 100) * 60).ToString();

            //#endregion


            return Ok(new { getAttendanceDataTable = getAttendanceDataTable, 
                //findMaximumVenderEntryTime = findMaximumVenderEntryTime,
                //countSupplier = countSupplier,
                //countEntryTime = countEntryTime,
                       
         });
        }

    }
}
