using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HRISv2.Models;
using Kendo.Mvc.Infrastructure.Implementation;

namespace HRISv2.Controllers
{
    
    public class ToolboxController : Controller
    {
        HRISEntities db = new HRISEntities();
        
        public JsonResult GetAllApplications(String approvingEIC)
        {
            var passSlipApp = (from r in db.vpassSlipApps
                               where r.statusID == 0
                               where r.apprvEIC == approvingEIC
                               select r).Count();

            var ptlosApp = (from g in db.vPtlosApps
                            where g.recommendStatus == 1
                            where g.Tag == 3
                            where g.approveEIC == approvingEIC
                            select g).Count();

            var justificationApp = (from g in db.tjustifyApps
                                    where g.statusID == null
                                    where g.approveEIC == approvingEIC
                                    select g).Count();

            var revertJustificationApp = (from g in db.tjustifyApps
                                          where g.statusID == 1               // 1 - is approved
                                          where g.approveEIC == approvingEIC
                                          where g.returnTag == 1
                                          select g).Count();

            var returnDtrApp = (from g in db.tAttDTRs
                                where g.approveEIC == approvingEIC
                                where g.returnTag == 1
                                select g).Count();

            var applicationMenus = new List<ApplicationMenu>();
            var menu1 = new ApplicationMenu(1, "PASS SLIP", "", passSlipApp);
            var menu2 = new ApplicationMenu(2, "PTLOS", "", ptlosApp);
            var menu3 = new ApplicationMenu(3, "JUSTIFICATION", "", justificationApp);
            var menu4 = new ApplicationMenu(4, "REVERT - JUSTIFICATION", "", revertJustificationApp);
            var menu5 = new ApplicationMenu(5, "RETURN - DTR", "", returnDtrApp);
            applicationMenus.Add(menu1);
            applicationMenus.Add(menu2);
            applicationMenus.Add(menu3);
            applicationMenus.Add(menu4);
            applicationMenus.Add(menu5);
            applicationMenus.Reverse();

            return Json(applicationMenus, JsonRequestBehavior.AllowGet);
        }
        
        /*
         * DTR
         */
        public JsonResult DTRAction(String DtrId, String strPeriod, int intPeriod, int action, String approvingEIC, String remarks)
        {
            Boolean has_error;
            try
            {
                // invoke the store procedure for DTR
                db.DtrAction(DtrId, strPeriod, intPeriod, action, approvingEIC, remarks);
                db.SaveChanges();

                has_error = false;
            }
            catch (Exception ex)
            {
                has_error = true;
            }

            dynamic wrap = new { dtr_action = new { has_error } };
            return Json(wrap, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DTRReturnRequest(String approvingEIC)
        {
            /**
             * Returns a list of Employee that requests return of their DTRs
             */
            var list = (from r in db.tAttDTRs
                        group r by new
                        {
                            r.EIC,
                            r.approveEIC,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC == r.EIC select s.fullnameFirst).FirstOrDefault().ToString().Trim(),
                            r.returnTag,
                            total = (from y in db.tAttDTRs
                                     where y.EIC == r.EIC
                                     where y.approveEIC == r.approveEIC
                                     where y.returnTag == 1
                                     select y).Count()
                        } into g
                        orderby g.Key.fullnameFirst
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.returnTag == 1
                        select new
                        {
                            g.Key.EIC,
                            g.Key.approveEIC,
                            g.Key.fullnameFirst,
                            g.Key.total,
                            g.Key.returnTag
                        });

            dynamic wrap = new { dtrs = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DTRReturnRequestPerEmployee(String EIC, String approvingEIC)
        {
            /**
             * Returns a list of DTRs requested by employee
             */
            var list = (from r in db.tAttDTRs
                        group r by new
                        {
                            r.DtrID,
                            r.Year,
                            r.Month,
                            r.Period,
                            r.EIC,
                            r.approveEIC,
                            r.Remarks,
                            r.returnTag,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC == r.EIC select s.fullnameFirst).FirstOrDefault().ToString().Trim()
                        } into g
                        orderby new { g.Key.Year, g.Key.Month, g.Key.Period } descending
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.EIC == EIC
                        where g.Key.returnTag == 1
                        select new
                        {
                            g.Key.DtrID,
                            g.Key.Year,
                            g.Key.Month,
                            g.Key.Period,
                            g.Key.EIC,
                            g.Key.approveEIC,
                            g.Key.fullnameFirst,
                            g.Key.Remarks,
                            g.Key.returnTag
                        });

            dynamic wrap = new { dtrs = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DTRDetail(String DtrId)
        {
            /**
             * Returns DTR detail requested by employee for return
             */
            var list = (from r in db.tAttDTRs
                        where r.DtrID == DtrId
                        where r.returnTag == 1  
                        select new
                        {
                            r.DtrID,
                            r.EIC,
                            r.approveEIC,
                            r.Remarks,
                            r.returnTag,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC == r.EIC select s.fullnameFirst).FirstOrDefault().ToString().Trim()
                        });

            dynamic wrap = new { dtr = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }

        /*
         * ******** Begin --- WEB BUNDY *********
         */
        public ActionResult FormBundy(String EIC)
        {
            // var EIC = Session["EIC"].ToString();

            if (IsFlexi(EIC))
            {
                var logNow = DateTime.Now;
                var logDate = logNow.Date;
                var logTime = logNow.TimeOfDay;

                var attDailyLog = db.tAttDailyLogs.Where(r => r.EIC == EIC).SingleOrDefault(r => r.LogDate == logDate);
                var employee = db.vUserProfiles.SingleOrDefault(r => r.EIC == EIC);

                String in1 = null, out1 = null, in2 = null, out2 = null;
                if (attDailyLog != null)
                {
                    if (attDailyLog.In1 != null) in1 = attDailyLog.In1.Value.ToShortTimeString();
                    if (attDailyLog.Out1 != null) out1 = attDailyLog.Out1.Value.ToShortTimeString();
                    if (attDailyLog.In2 != null) in2 = attDailyLog.In2.Value.ToShortTimeString();
                    if (attDailyLog.Out2 != null) out2 = attDailyLog.Out2.Value.ToShortTimeString();
                }

                // determine next log
                String nextLog = null;
                if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 6, 0, 0).TimeOfDay &&
                    logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 12, 0, 0).TimeOfDay &&
                    in1 == null)
                {
                    nextLog = "IN1";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 8, 0, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 13, 0, 0).TimeOfDay &&
                         out1 == null)
                {
                    nextLog = "OUT1";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 12, 30, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 17, 0, 0).TimeOfDay &&
                         in2 == null)
                {
                    nextLog = "IN2";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 13, 0, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 23, 59, 59).TimeOfDay &&
                         out2 == null)
                {
                    nextLog = "OUT2";
                }

                ViewBag.EIC = EIC;
                ViewBag.schemeCode = employee != null ? employee.SchemeCode : null;
                ViewBag.logDate = logDate;
                ViewBag.nextLog = nextLog;
                ViewBag.in1 = in1;
                ViewBag.in2 = in2;
                ViewBag.out1 = out1;
                ViewBag.out2 = out2;
                ViewBag.isFlexi = true;
            }
            else
            {
                ViewBag.isFlexi = false;
                ViewBag.logDate = DateTime.Today;
            }

            return View();
        }
        [HttpPost]
        public ActionResult LogBundyWeb(String time_period, String EIC, String schemeCode)
        {
            var logNow = DateTime.Now;
            var logDate = logNow.Date;
            var logTime = logNow.TimeOfDay;

            // check for existing record entry
            var rec = db.tAttDailyLogs.Where(r => r.EIC == EIC).SingleOrDefault(r => r.LogDate == logDate);
            if (rec == null)
            {
                // create an entry if no existing record
                var n = new tAttDailyLog
                {
                    EIC = EIC,
                    LogDate = logDate,
                    SchemeCode = schemeCode,
                    nonRegDay = 0
                };

                db.tAttDailyLogs.Add(n);
                db.SaveChanges();
            }

            // update the record
            var l = db.tAttDailyLogs.Where(r => r.EIC == EIC).Single(r => r.LogDate == logDate);
            if (time_period.Equals("IN1"))
            {
                l.In1 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("OUT1"))
            {
                l.Out1 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("IN2"))
            {
                l.In2 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("OUT2"))
            {
                l.Out2 = logNow;
                l.LastLog = time_period;
            }
            // save all changes
            db.SaveChanges();

            //BundyTransaction(EIC, time_period);

            var log = db.tAttDailyLogs.Where(r => r.EIC == EIC).SingleOrDefault(r => r.LogDate == logDate);
            ViewBag.log = log;
            ViewBag.EIC = EIC;

            return Content("1");
        }
        

        /*
         * ******** Begin --- App BUNDY *********
         */
        public JsonResult CheckBundy(String EIC)
        {
            if (IsFlexi(EIC))
            {

                var logNow = DateTime.Now;
                var logDate = logNow.Date;
                var logTime = logNow.TimeOfDay;

                var attDailyLog = db.tAttDailyLogs.Where(r => r.EIC == EIC).SingleOrDefault(r => r.LogDate == logDate);

                String in1 = null, out1 = null, in2 = null, out2 = null;

                if (attDailyLog != null)
                {
                    if (attDailyLog.In1 != null) in1 = attDailyLog.In1.Value.ToShortTimeString();
                    if (attDailyLog.Out1 != null) out1 = attDailyLog.Out1.Value.ToShortTimeString();
                    if (attDailyLog.In2 != null) in2 = attDailyLog.In2.Value.ToShortTimeString();
                    if (attDailyLog.Out2 != null) out2 = attDailyLog.Out2.Value.ToShortTimeString();
                }

                // determine next log
                String nextLog = null;
                if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 6, 0, 0).TimeOfDay &&
                    logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 12, 0, 0).TimeOfDay &&
                    in1 == null)
                {
                    nextLog = "IN1";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 8, 0, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 13, 0, 0).TimeOfDay &&
                         out1 == null)
                {
                    nextLog = "OUT1";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 12, 30, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 17, 0, 0).TimeOfDay &&
                         in2 == null)
                {
                    nextLog = "IN2";
                }
                else if (logTime >= new DateTime(logDate.Year, logDate.Month, logDate.Day, 13, 0, 0).TimeOfDay &&
                         logTime < new DateTime(logDate.Year, logDate.Month, logDate.Day, 23, 59, 59).TimeOfDay &&
                         out2 == null)
                {
                    nextLog = "OUT2";
                }

                return
                    Json(
                        new
                        {
                            status =
                                new { logDate = logDate.ToShortDateString(), nextLog, in1, out1, in2, out2, attDailyLog }
                        },
                        JsonRequestBehavior.AllowGet);
            }
            else
            {
                return
                    Json(
                        new
                        {
                            status =
                                new { logDate = "Invalid Flexi User" }
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult LogBundy(String time_period, String EIC, String schemeCode)
        {
            var logNow = DateTime.Now;
            var logDate = logNow.Date;
            var logTime = logNow.TimeOfDay;

            // check for existing record entry
            var rec = db.tAttDailyLogs.Where(r => r.EIC == EIC).SingleOrDefault(r => r.LogDate == logDate);
            if (rec == null)
            {
                // create an entry if no existing record
                var n = new tAttDailyLog
                {
                    EIC = EIC,
                    LogDate = logDate,
                    SchemeCode = schemeCode
                };

                db.tAttDailyLogs.Add(n);
                db.SaveChanges();
            }

            // update the record
            var l = db.tAttDailyLogs.Where(r => r.EIC == EIC).Single(r => r.LogDate == logDate);
            if (time_period.Equals("IN1"))
            {
                l.In1 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("OUT1"))
            {
                l.Out1 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("IN2"))
            {
                l.In2 = logNow;
                l.LastLog = time_period;
            }
            else if (time_period.Equals("OUT2"))
            {
                l.Out2 = logNow;
                l.LastLog = time_period;
            }
            // save all changes
            db.SaveChanges();

            // BundyTransaction(EIC, time_period);

            var log = db.tAttDailyLogs.Where(r => r.EIC == EIC).Where(r => r.LogDate == logDate);

            return Json(new { log }, JsonRequestBehavior.AllowGet);
        }
        public bool IsFlexi(String EIC)
        {
            var b = db.tappDFlexibles.SingleOrDefault(r => r.EIC == EIC);
            return b != null;
        }
        public void BundyTransaction(String EIC, String logStatus)
        {
            // log every bundy transaction
            var bt = new tappDFlexiblesLog()
            {
                EIC = EIC,
                loginStatus = logStatus,
                timeStamp = DateTime.Now
            };
            db.tappDFlexiblesLogs.Add(bt);
            db.SaveChanges();
        }

        /*
         * ****** BEGIN --- Justification ******
         */
        public JsonResult JustificationRevertDetail(String EIC, String approvingEIC, int month, int year, int period)
        {
            var list = (from r in db.vJustifyApps
                        where r.EIC == EIC
                        where r.approveEIC == approvingEIC
                        where r.month == month
                        where r.year == year
                        where r.period == period
                        where r.returnTag == 1
                        select new
                        {
                            r.fullnameFirst,
                            r.logType,
                            r.logTitle,
                            r.time,
                            date = r.date.ToString(),
                            r.reason
                        }).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationRevertPerMonth(String EIC, String approvingEIC)
        {
            /**
             * Returns a list of Justifications per Month for revert
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            month_year = r.monthyear,
                            r.EIC,
                            r.approveEIC,
                            r.month,
                            r.year,
                            r.period,
                            r.statusID,
                            r.returnTag,
                            total = (from y in db.vJustifyApps
                                     where y.EIC == r.EIC
                                     where y.monthyear == r.monthyear
                                     where y.period == r.period
                                     select y).Count()
                        } into g
                        orderby g.Key.year, g.Key.month
                        where g.Key.EIC == EIC
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.statusID == 1
                        where g.Key.returnTag == 1
                        select g.Key).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationRevert(String approvingEIC)
        {
            /**
             * Returns a list of Justifications for revert
             */
            var list = (from r in db.tjustifyApps
                        group r by new
                        {
                            r.EIC,
                            r.approveEIC,
                            r.statusID,
                            fullnameFirst = (from s in db.tappEmployees where s.EIC==r.EIC select s.fullnameFirst).FirstOrDefault().ToString(),
                            r.returnTag,
                            total = (from y in db.tjustifyApps
                                     where y.EIC == r.EIC
                                     where y.approveEIC == r.approveEIC
                                     where y.returnTag == 1
                                     select y).Count()
                        } into g
                        orderby g.Key.fullnameFirst
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.returnTag == 1
                        select new
                        {
                            g.Key.EIC,
                            g.Key.approveEIC,
                            g.Key.statusID,
                            g.Key.fullnameFirst,
                            g.Key.returnTag,
                            g.Key.total
                        });

            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationPending(String approvingEIC)
        {
            /**
             * Returns a list of Justification Totals for Approval
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            r.EIC, 
                            r.approveEIC, 
                            r.statusID, 
                            r.fullnameFirst, 
                            total = (from y in db.vJustifyApps 
                                     where y.EIC == r.EIC 
                                     where y.approveEIC == r.approveEIC
                                     where y.statusID != 1
                                     select y).Count()
                        } into g
                        orderby g.Key.fullnameFirst
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.statusID != 1
                        select new
                        {
                            g.Key
                        });

            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationPerMonth(String EIC, String approvingEIC)
        {
            /**
             * Returns a list of Justifications per Month for Approval
             */
            var list = (from r in db.vJustifyApps
                        group r by new
                        {
                            month_year = r.monthyear,
                            r.EIC,
                            r.approveEIC,
                            r.month,
                            r.year,
                            r.period,
                            r.statusID,
                            total = (from y in db.vJustifyApps 
                                     where y.EIC == r.EIC 
                                     where y.monthyear == r.monthyear
                                     where y.period == r.period
                                     select y).Count()
                        }  into g
                        orderby g.Key.year, g.Key.month
                        where g.Key.EIC == EIC
                        where g.Key.approveEIC == approvingEIC
                        where g.Key.statusID != 1
                        select g.Key).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationDetail(String EIC, String approvingEIC, int month, int year, int period)
        {
            var list = (from r in db.vJustifyApps
                        where r.EIC == EIC
                        where r.approveEIC == approvingEIC
                        where r.month == month
                        where r.year == year
                        where r.period==period
                        where r.statusID != 1
                        select new
                        {
                            r.fullnameFirst,
                            r.logType,
                            r.logTitle,
                            r.time,
                            date = r.date.ToString(),
                            r.reason
                        }).ToList();
            dynamic wrap = new { justifications = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JustificationApproval(String EIC, int month, int year, String month_year, String approvingEIC, int statusID, int period, String remarks)
        {
            Boolean has_error;
            try
            {
                DateTime startDate = new DateTime(), endDate = new DateTime();
                String CtrlNo = EIC.Substring(0, 8) + year.ToString() + month_year.Substring(0, 3);

                // determine startDate and endDate
                if (period == 0)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else if (period == 1)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = new DateTime(year, month, 15);
                }
                else if (period == 2)
                {
                    startDate = new DateTime(year, month, 16);
                    endDate = startDate.AddDays(DateTime.DaysInMonth(year, month) - 16);
                }


                db.JustifyAction(EIC, DateTime.Parse(startDate.ToShortDateString()), DateTime.Parse(endDate.ToShortDateString()), statusID, approvingEIC, CtrlNo, remarks, period);
                db.SaveChanges();

                has_error = false;
            }
            catch (Exception ex)
            {
                has_error = true;
            }

            dynamic wrap = new {justification_approval = new {has_error}};
            return Json(wrap, JsonRequestBehavior.AllowGet);

        }

        /*
         * ****** BEGIN --- PTLOS ******
         */
        public JsonResult GetPTLOSApplications(String approvingEIC)
        {
            var list = (from r in db.tptlosApps
                where r.approveEIC == approvingEIC
                where r.recommendStatus == 1
                where r.Tag == 3
                select new
                {
                    r.recNo,
                    name = r.nameText,
                    destination = r.proceedTo.Trim(),
                    date_applied = r.dateApplied.ToString(),
                    r.purpose
                }).ToList();
            dynamic wrap = new {ptlos_applications = list};
            return Json(wrap,JsonRequestBehavior.AllowGet);
        }
        public JsonResult PTLOSDetail(int id)
        {
            var list = (from r in db.tptlosApps
                        where r.recNo == id
                        select new
                        {
                            r.recNo,
                            name = r.nameText,
                            destination = r.proceedTo.Trim(),
                            purpose = r.purpose.Trim(),
                            date_applied = r.dateApplied.ToString(),
                            departure = r.departure.ToString(),
                            arrival = r.arrival.ToString(),
                            official_return = r.returnOfficial.ToString()
                        }).ToList();
            dynamic wrap = new { ptlos_detail = list };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PTLOSApproval(int id, int tag)
        {
            try
            {
                var ptlos = db.tptlosApps.Single(r => r.recNo == id);
                ptlos.Tag = tag;      // 3 - Recommend, 4 - Disapprove, 5 - approve, 9 - Cancel, 0 - Returned
                // update to code
                if (tag == 5)   // if approve
                {
                    ptlos.approvedDate = DateTime.Now;
                    ptlos.approveStatus = 1;    // 0 - default, 1 - approve, 3 - cancel
                    ptlos.remarks = "Approved";
                }

                if (tag == 0)   // if returned
                {
                    ptlos.recommendStatus = 0;
                    ptlos.approveStatus = 0;    // 0 - default, 1 - approve, 3 - cancel
                    ptlos.remarks = "Returned";
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            dynamic wrap = new {ptlos_approval = new {tag}};
            return Json(wrap,JsonRequestBehavior.AllowGet);
        }

        /*
         * ****** BEGIN --- PASS SLIPS ******
         */
        public JsonResult PassSlipApproval(int id, int statusId, int isOfficial)
        {
            
            try
            {
                var passSlip = db.tpassSlipApps.Single(r => r.recNo == id);
                passSlip.statusID = statusId;
                passSlip.isOfficial = isOfficial;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            dynamic wrap = new { pass_slip_approval = new { statusId } };
            return Json(wrap, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipDetail(int id)
        {
            var list = (from r in db.vpassSlipApps
                        where r.recNo == id
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            fullnameFirst = r.fullnameFirst,
                            timeOut = r.timeOut.ToString(),
                            timeIn = r.timeIn.ToString(),
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                        }).ToList();
            dynamic listWrapper = new {pass_slip_detail = list};
            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsPending(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                orderby r.timeOut 
                where r.apprvEIC == approvingEIC
                where r.statusID == 0   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                select new
                {
                    recNo = r.recNo,
                    controlNo = r.controlNo,
                    EIC = r.EIC,
                    timeOut = r.timeOut.ToString(),
                    timeIn = r.timeIn.ToString(),
                    destination = r.destination,
                    purpose = r.purpose,
                    isOfficial = r.isOfficial,
                    statusID = r.statusID,
                    apprvEIC = r.apprvEIC,
                    fullnameFirst = r.fullnameFirst,
                    empGroupCode = r.empGroupCode
                }).ToList();

            dynamic listWrapper = new {pass_slips = list};

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsApproved(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                        orderby r.timeOut descending 
                        where r.apprvEIC == approvingEIC
                        where r.statusID == 1   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            timeOut = r.timeOut,
                            timeIn = r.timeIn,
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                            fullnameFirst = r.fullnameFirst,
                            empGroupCode = r.empGroupCode
                        }).ToList();
            dynamic listWrapper = new { pass_slips = list };

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsDisapproved(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                        orderby r.timeOut descending 
                        where r.apprvEIC == approvingEIC
                        where r.statusID == 2   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            timeOut = r.timeOut,
                            timeIn = r.timeIn,
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                            fullnameFirst = r.fullnameFirst,
                            empGroupCode = r.empGroupCode
                        }).ToList();
            dynamic listWrapper = new { pass_slips = list };

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PassSlipsCancelled(String approvingEIC)
        {
            // select all pass slip application under a certain 
            // approving officer using EIC
            var list = (from r in db.vpassSlipApps
                        orderby r.timeOut descending 
                        where r.apprvEIC == approvingEIC
                        where r.statusID == 3   // 0 (pending), 1 (approved), 2 (disapproved), 3 (cancelled)
                        select new
                        {
                            recNo = r.recNo,
                            controlNo = r.controlNo,
                            EIC = r.EIC,
                            timeOut = r.timeOut,
                            timeIn = r.timeIn,
                            destination = r.destination,
                            purpose = r.purpose,
                            isOfficial = r.isOfficial,
                            statusID = r.statusID,
                            apprvEIC = r.apprvEIC,
                            fullnameFirst = r.fullnameFirst,
                            empGroupCode = r.empGroupCode
                        }).ToList();
            dynamic listWrapper = new { pass_slips = list };

            return Json(listWrapper, JsonRequestBehavior.AllowGet);
        }

        /*
         * Different Listings
         */
        public JsonResult EmployeeList()
        {
            //var list = db.tappEmployee.OrderBy(g => g.fullnameLast).ToList();
            var list = (from r in db.tappEmployees
                        orderby r.fullnameLast
                        select new
                        {
                            EIC = r.EIC, 
                            FullnameWithEIC = (r.EIC + " - " + r.fullnameLast), 
                            Fullname = r.fullnameLast
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeDetail(String EIC)
        {
            //var list = db.tappEmployee.OrderBy(g => g.fullnameLast).ToList();
            var list = (from r in db.tappEmployees
                        orderby r.fullnameLast
                        where r.EIC == EIC
                        select new
                        {
                            EIC = r.EIC,
                            FullnameWithEIC = (r.EIC + " - " + r.fullnameLast),
                            Fullname = r.fullnameLast
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PositionList()
        {
            var list = (from r in db.tappPositions
                orderby r.fullDescription
                select r).ToArray();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Position(string id)
        {
            var list = (from r in db.tappPositions
                        orderby r.fullDescription
                        where r.positionCode==id
                        select r).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubPositionList()
        {
            var list = (from r in db.tappPositionSubs
                        orderby r.subPositionName
                        select r).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult WorkStatusList()
        {
            var list = (from r in db.tappEmpStatus
                        orderby r.statusName
                        where !r.statusName.Equals("ADMIN")
                        select r).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OfficeList()
        {
            var list = (from r in db.tappOffices
                        orderby r.officeServiceRec
                        select new
                        {
                            recNo = r.recNo,
                            officeCode = r.officeCode,
                            officeName = r.officeName,
                            accronym = r.accronym,
                            officeServiceRec = r.officeServiceRec,
                            officeServiceRecWithCode = r.officeServiceRec + " - " + r.officeCode,
                            branch = r.branch,
                            sortTag = r.sortTag,
                            tagRemarks = r.tagRemarks
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalarySchemeList()
        {
            var list = (from r in db.tappSalarySchems
                        orderby r.paySchemeName
                        select new
                        {
                            paySchemeCode = r.paySchemeCode,
                            paySchemeNameWithCode = r.paySchemeCode + " - " + r.paySchemeName,
                            paySchemeName = r.paySchemeName,
                            paySchemeAcronym = r.paySchemeAcronym
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalarySchedule(int sg, int step)
        {
            var salSched = (from r in db.tappSalaryScheds
                            where r.sg == sg
                            where r.step == step
                            where r.isActive == 1
                            select r).ToList();
            return Json(salSched, JsonRequestBehavior.AllowGet);
        }
    }

    class ApplicationMenu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public int TotalApplications { get; set; }

        public ApplicationMenu(int _Id, string title, string iconUrl, int totalApplications)
        {
            Id = _Id;
            Title = title;
            IconUrl = iconUrl;
            TotalApplications = totalApplications;
        }
    }
}