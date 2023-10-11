using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierControlBackend.Contexts;
using SupplierControlBackend.Models;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupplierControlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierDashBoardController : ControllerBase
    {
        // GET: api/<SupplierDashBoardController>
        private readonly DbSC_Suppiler_Driver_Lisense _SuppilerDriverLisenseContext;
        private readonly DbSC_SC_Suplier_In_Out _SuppilerDriverInOutContext;

        public SupplierDashBoardController(DbSC_Suppiler_Driver_Lisense suppilerDriverLisenseContext, DbSC_SC_Suplier_In_Out suppilerDriverInOutContext)
        {
            _SuppilerDriverLisenseContext = suppilerDriverLisenseContext;
            _SuppilerDriverInOutContext = suppilerDriverInOutContext;
        }

        //ดึงข้อมูลคนขับรถ
        [HttpGet]
        [Route("getAllSuppilerDashboard")]
        public async Task<IActionResult> getAll()
        {
            DateTime start_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00);
            DateTime end_shift_day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 45, 00, 00); ;


            DateTime start_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 00, 00);
            DateTime end_shift_night = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 08, 00, 00, 00); ;

            // หน้า Attandance ดึงข้ัอมูลมาแสดง
            int currentOfMonth = DateTime.Now.Month;
            int currentOfDay = DateTime.Now.Day;
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
                    deliverlyRemainTime = ((int)(DateTime.Now - attendace.EntryTime).Value.TotalMinutes / 60).ToString() + ":" + ((int)(DateTime.Now - attendace.EntryTime).Value.TotalMinutes % 60).ToString(),
                    deliverlyTotalEntryTime = attendace.LeaveTime - attendace.EntryTime,
                    deliverlyTotalEntryTimess = ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes / 60).ToString() + ":" + ((int)(attendace.LeaveTime - attendace.EntryTime).Value.TotalMinutes % 60).ToString(),
                    deliverlyStatusCal = (attendace.LeaveTime - attendace.EntryTime).Value.Hours < 0 ? false : true,
                    deliverlyRound = attendace.DeliveryRound

                }).Where(x=>x.deliverlyDate.Month == currentOfMonth).ToList();
            #endregion


            #region// supplier ที่อยู่นานที่สุด
            var findMaximumVenderEntryTime = getAttendanceDataTable.GroupBy(x => new { x.deliverlyCompnaycode, x.deliverlyCompany, x.deliverlyShift })
                                            .Select(MaximumVender => new
                                            {
                                                deliverlyMaximumCompany = MaximumVender.FirstOrDefault().deliverlyCompany,
                                                deliverlygetTotalHours = new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)),
                                                deliverlyMaximumEntryTime = ((int)(new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes / 60)).ToString() +
                                                ":" + ((int)(new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes % 60)).ToString()
                                            }).OrderByDescending(x => x.deliverlygetTotalHours).Take(1).ToList();

            #endregion


            #region// supplier ที่อยู่นานที่สุด TOP5
            var findMaximumVenderEntryTimeTop5 = getAttendanceDataTable.GroupBy(x => new { x.deliverlyCompnaycode, x.deliverlyCompany, x.deliverlyShift })
                                            .Select(MaximumVender => new
                                            {
                                                deliverlyMaximumCompany = MaximumVender.FirstOrDefault().deliverlyCompany,
                                                deliverlygetTotalHours = new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)),
                                                deliverlyMaximumEntryTime = ((int)(new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes / 60)).ToString() +
                                                ":" + ((int)(new TimeSpan(MaximumVender.Where(p => p.deliverlyStatusCal == true).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes % 60)).ToString()
                                            }).OrderByDescending(x => x.deliverlygetTotalHours).Take(7).ToList();

            #endregion





            #region// จำนวน supplier ทั้งหมด
            var countSupplier = _SuppilerDriverLisenseContext.ScSupplierDriverLisenses.Count();

            #endregion

            #region// จำนวนชั่วโมง supplier ที่เข้ามาทำงานในวันนี้  
            var countEntryTimeInDay = ((int)(new TimeSpan(getAttendanceDataTable.Where(p => p.deliverlyStatusCal == true && p.deliverlyDate.Day == currentOfDay).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes / 60)).ToString() +
                                                 ":" + ((int)(new TimeSpan(getAttendanceDataTable.Where(p => p.deliverlyStatusCal == true && p.deliverlyDate.Day == currentOfDay).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes % 60)).ToString();
            #endregion



            #region// จำนวนชั่วโมง supplier ที่เข้ามาทำงานในเดือนนี้ 

            var countEntryTimeInMonth = ((int)(new TimeSpan(getAttendanceDataTable.Where(p => p.deliverlyStatusCal == true && p.deliverlyDate.Month == currentOfMonth).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes / 60)).ToString() +
                                                 ":" + ((int)(new TimeSpan(getAttendanceDataTable.Where(p => p.deliverlyStatusCal == true && p.deliverlyDate.Month == currentOfMonth).Sum(p => p.deliverlyTotalEntryTime.Value.Ticks)).TotalMinutes % 60)).ToString();

            #endregion


            #region bar dashboard
            string[] monthNames = new string[12];
            monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            var countVenderEveryMonth = (from vender_dashboard in _SuppilerDriverInOutContext.ScSuplierInOuts
                                        group vender_dashboard by vender_dashboard.DeliveryDate.Month into dhb
                                        select new
                                        {
                                            countMonth = dhb.First().DeliveryDate.Month,
                                            countVender = dhb.Count()
                                        }).ToList();

            int[] countVender = new int[12];
            int i = 1;
            foreach (var vender in countVenderEveryMonth)
            {
               
                countVender[vender.countMonth - 1] = vender.countVender;
           
                i++;
            }

     

            #endregion


            #region donut dashboard
            string[] donutCompany = new string[5];
            decimal[] donutTimme = new decimal[5];
            int j = 0;
            foreach(var company in findMaximumVenderEntryTimeTop5)
            {
                donutCompany[j] = company.deliverlyMaximumCompany;
                donutTimme[j] = Convert.ToDecimal(company.deliverlyMaximumEntryTime.Split(":")[0] + "." + convertFormat(company.deliverlyMaximumEntryTime.Split(":")[1]));
                j++;
            }
            #endregion
            List<Dashboard> dashborad_list = new List<Dashboard>();

            Dashboard dhl = new Dashboard();
            dhl.Month = monthNames;
            dhl.supplierData = countVender;
            dhl.company = donutCompany;
            dhl.totalTime = donutTimme;
            dashborad_list.Add(dhl);






            return Ok(new
            {
                getAttendanceDataTable = getAttendanceDataTable,
                findMaximumVenderEntryTime = findMaximumVenderEntryTime,
                findMaximumVenderEntryTimeTop5 = findMaximumVenderEntryTimeTop5,
                countSupplier = countSupplier,
                countEntryTimeInDay = countEntryTimeInDay,
                countEntryTimeInMonth = countEntryTimeInMonth,
                dashborad_list = dashborad_list

            });
        }

        private string convertFormat(string num)
        {

            if (Convert.ToDecimal(num) < 10)
            {
                num = "0" + num;
            }
            return num;
        }

    }

   
}
