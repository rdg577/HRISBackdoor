using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                where g.Tag == 3
                where g.approveEIC == approvingEIC
                select g).Count();

            var applicationMenus = new List<ApplicationMenu>();
            var menu1 = new ApplicationMenu(1, "PASS SLIP", "", passSlipApp);
            var menu2 = new ApplicationMenu(1, "PTLOS", "", ptlosApp);
            applicationMenus.Add(menu1);
            applicationMenus.Add(menu2);
            
            return Json(applicationMenus, JsonRequestBehavior.AllowGet);
        }

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