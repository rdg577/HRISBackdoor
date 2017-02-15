using System;
using System.Linq;
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
            return Content(id);
        }

        
    }
}
