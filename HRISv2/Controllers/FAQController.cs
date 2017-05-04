using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRIS.Models;

namespace HRIS.Controllers
{
    public class FAQController : Controller
    {
        HRISMonitoringEntities db = new HRISMonitoringEntities();

        // GET: FAQ
        public ActionResult Index()
        {
            var faqs = db.tappFAQs.OrderByDescending(r => r.timestamp).ToList();
            ViewBag.faqs = faqs;

            return View();
        }

        [HttpPost]
        public ActionResult Create(string officeCode, int questionCode)
        {
            var petsa = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            var faq = new tappFAQ {officeCode = officeCode, questionCode = questionCode, timestamp = Convert.ToDateTime(petsa)};

            db.tappFAQs.Add(faq);
            db.SaveChanges();

            return Content(0.ToString());
        }

        [HttpPost]
        public ActionResult CreateQuestion(string question)
        {
            var q = new trefQuestion {detail = question.ToUpperInvariant()};
            db.trefQuestions.Add(q);
            db.SaveChanges();

            return Content(0.ToString());
        }
        
        public JsonResult GetOffices()
        {
            var list = db.tappOffices.OrderBy(r => r.officeName).Select(r => new { r.officeCode, r.officeName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuestions()
        {
            var list = db.trefQuestions.Select(r => new { r.Id, r.detail }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActionDetails()
        {
            var list = db.tappActionDetails.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRemarks()
        {
            var list = db.trefRemarks.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}