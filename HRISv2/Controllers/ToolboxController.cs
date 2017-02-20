using System;
using System.Collections.Generic;
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

        public JsonResult EmployeeList()
        {
            //var list = db.tappEmployee.OrderBy(g => g.fullnameLast).ToList();
            var list = (from r in db.tappEmployee
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
            var list = (from r in db.tappPosition
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
            var list = (from r in db.tappOffice
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
            var list = (from r in db.tappSalarySchem
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
            var salSched = (from r in db.tappSalarySched
                            where r.sg == sg
                            where r.step == step
                            where r.isActive == 1
                            select r).ToList();
            return Json(salSched, JsonRequestBehavior.AllowGet);
        }
    }
}