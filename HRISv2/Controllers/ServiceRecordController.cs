using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HRISv2.Models;
using PagedList;

namespace HRISv2.Controllers
{
    public class ServiceRecordController : Controller
    {
        private HRISEntities db = new HRISEntities();

        // GET: ServiceRecord
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            const int pageSize = 40;
            int pageNumber = (page ?? 1);

            var vUserProfileWithServices = db.vUserProfileWithServices;

            // search
            if (!string.IsNullOrEmpty(searchString))
            {
                return View(vUserProfileWithServices.Where(u => u.Fullname.StartsWith(searchString)).Distinct().OrderBy(u => u.Fullname).ToPagedList(pageNumber, pageSize));
            }

            // list down all employees with service records of at least one
            return View(vUserProfileWithServices.OrderBy(r=>r.Fullname).ToPagedList(pageNumber, pageSize)); 
        }

        public ActionResult Display(String id)
        {
            if(id==null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // query employee service record
            //var employeeServiceRecord = db.tappServiceRecord.Where(g => g.EIC == id).ToList();
            IEnumerable<fnGetEmployeeServiceRecords_Result> employeeServiceRecord = db.fnGetEmployeeServiceRecords(id).ToList();
            if (!employeeServiceRecord.Any())
            {
                return HttpNotFound();
            }

            ViewBag.Fullname = employeeServiceRecord.First().Fullname;
            ViewBag.employeeServiceRecord = employeeServiceRecord.OrderByDescending(g=>g.dateFrom);

            return View();

        }

        [HttpGet]
        public ActionResult Create()
        {
            var employeeList = db.tappEmployee.OrderBy(g => g.fullnameLast).ToList();

            ViewBag.employeeList = employeeList;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="EIC, dateFrom, dateTo, designation, statusName, " +
                  "officeServiceRec, branch, salaryPayroll, salaryServiceRec, paySchemeCode, paySchemeName, sepCause, officeCode")] tappServiceRecord tappServiceRecord)
        {
            if (ModelState.IsValid)
            {
                db.tappServiceRecord.Add(tappServiceRecord);
                db.SaveChanges();

                return RedirectToAction("Index","ServiceRecord");
            }

            return View();
        }
    }
}
