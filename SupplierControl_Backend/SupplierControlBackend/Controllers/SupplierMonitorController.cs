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
    public class SupplierMonitorController : Controller
    {
        private readonly DbSC_Suppiler_Driver_Lisense _SuppilerDriverLisenseContext;
        private readonly DbSC_SC_Suplier_In_Out _SuppilerDriverInOutContext;

        public SupplierMonitorController(DbSC_Suppiler_Driver_Lisense suppilerDriverLisenseContext, DbSC_SC_Suplier_In_Out suppilerDriverInOutContext)
        {
            _SuppilerDriverLisenseContext = suppilerDriverLisenseContext;
            _SuppilerDriverInOutContext = suppilerDriverInOutContext;
        }


        //ดึงข้อมูลคนขับรถ
        [HttpGet]
        [Route("getSupplierMonitor")]
        public async Task<IActionResult> getAll()
        {

            //  กะเช้า
            DateTime start_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00);
            DateTime end_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 45, 00, 00); ;

            // กะกลางคืน
            DateTime start_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 00, 00);
            DateTime end_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00);

            // วันแรกของเดือน กับ วันสุดท้ายของเดือน
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            // เดือนปัจจุบัน
            int currentOfMonth = DateTime.Now.Month;
            int currentOfDay= DateTime.Now.Day;


            // หน้า Attandance ดึงข้ัอมูลมาแสดง

            #region// Attendance Table 
            var getAttendance = await _SuppilerDriverInOutContext.ScSuplierInOuts.ToListAsync();
            var getAttendanceDataTable = (

                from attendace in getAttendance
                join company in _SuppilerDriverLisenseContext.ScSupplierDriverLisenses
                on attendace.VenderCard equals company.DriverNbr
                select new
                {   
                    deliverlyCompnaycode = company.VenderCode,
                    deliverlyCompany = company.VenderName,
                    deliverlyNbr = company.DriverNbr,
                    deliverlyDate = attendace.DeliveryDate,
                    deliverlyShift = attendace.DeliveryShift,
                    deliverlyEntryTime = attendace.EntryTime,
                    deliverlyLeaveTime = attendace.LeaveTime,
                    //deliverlyRemainTime = (DateTime.Now - attendace.EntryTime).Value.Hours.ToString() + ":" + (DateTime.Now - attendace.EntryTime).Value.Minutes.ToString(),
                    deliverlyRemainTime = ((int)(DateTime.Now - attendace.EntryTime).Value.TotalMinutes/60).ToString() + ":" + ((int)(DateTime.Now - attendace.EntryTime).Value.TotalMinutes %60).ToString(),
                    deliverlyTotalEntryTime = attendace.LeaveTime - attendace.EntryTime,
                    deliverlyTotalEntryTimess = ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes/60).ToString() + ":" + ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes % 60).ToString(),
                    deliverlyStatusCal = (attendace.LeaveTime - attendace.EntryTime).Value.Hours < 0 ? false : true,
                    deliverlyRound = attendace.DeliveryRound

                }).Where(x => x.deliverlyDate.Month == currentOfMonth).ToList();
            #endregion


            #region// supplier ที่อยู่นานที่สุด
            var findMaximumVenderEntryTime = getAttendanceDataTable.GroupBy(x => new { x.deliverlyCompnaycode, x.deliverlyCompany, x.deliverlyShift })
                                            .Select(MaximumVender => new
                                            {
                                                deliverlyMaximumCompany = MaximumVender.FirstOrDefault().deliverlyCompany,
                                                deliverlygetTotalHours = new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)),
                                                //deliverlyMaximumEntryTime = new TimeSpan(MaximumVender.Where(p=>p.deliverlyStatusCal == true).Sum(p=>p.deliverlyTotalEntryTime.Value.Ticks))
                                                deliverlyMaximumEntryTime = ((int)(new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes / 60)).ToString() +
                                                ":" + ((int)(new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes % 60)).ToString()
                                            }).OrderByDescending(x => x.deliverlygetTotalHours).Take(7).ToList();

            #endregion



          
            return Ok(new
            {
                getAttendanceDataTable = getAttendanceDataTable,
                findMaximumVenderEntryTime = findMaximumVenderEntryTime,
             

            });
        }


        //public string convertFormat(DateTime? startTimeEntry)
        //{
        //    string newFormat = "";
        //    int number = (DateTime.Now - startTimeEntry).Value.Minutes;
        //    if(number < 0) {
        //       newFormat =  "0"+(DateTime.Now - startTimeEntry).Value.Minutes.ToString();

        //    }
        //    else
        //    {
        //        newFormat = (DateTime.Now - startTimeEntry).Value.Minutes.ToString();
        //    }
        //    return newFormat;
        //}

    }
}
