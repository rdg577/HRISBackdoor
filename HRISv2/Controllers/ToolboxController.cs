using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using HRISv2.Models;
using Kendo.Mvc.Infrastructure.Implementation;

namespace HRISv2.Controllers
{
    public class ToolboxController : Controller
    {
        HRISEntities db = new HRISEntities();

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

            var applicationMenus = new List<ApplicationMenu>();
            var menu1 = new ApplicationMenu(1, "PASS SLIP", "", passSlipApp);
            var menu2 = new ApplicationMenu(2, "PTLOS", "", ptlosApp);
            var menu3 = new ApplicationMenu(3, "JUSTIFICATION", "", justificationApp);
            applicationMenus.Add(menu1);
            applicationMenus.Add(menu2);
            applicationMenus.Add(menu3);

            return Json(applicationMenus, JsonRequestBehavior.AllowGet);
        }

        // ****** BEGIN --- Justification ******
        public JsonResult JustificationPending(String approvingEIC)
        {
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
        public JsonResult JustificationApproval(
            String EIC, 
            int month, 
            int year, 
            String month_year, 
            String approvingEIC, 
            int statusID, 
            int period, 
            String remarks)
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
        // ****** END --- Justification ******

        // ****** BEGIN --- PTLOS ******
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
                ptlos.Tag = tag;      // 3 - Recommend, 4 - Disapprove, 5 - approve, 9 - Cancel

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            dynamic wrap = new {ptlos_approval = new {tag}};
            return Json(wrap,JsonRequestBehavior.AllowGet);
        }
        // ****** END --- PTLOS ******

        // ****** BEGIN --- PASS SLIPS ******
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
        // ****** END --- PASS SLIPS ******
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
}