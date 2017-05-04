using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HRIS.Models;
using PagedList;

namespace HRIS.Controllers
{
    public class DuplicatePlantillaController : Controller
    {
        HRISEntities _db = new HRISEntities();

        // GET: DuplicatePlantilla
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

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            var vDuplicateEICPlantillaPreparation = _db.vDuplicateEICPlantillaPreparations;

            // search
            if (!string.IsNullOrEmpty(searchString))
            {
                return View(vDuplicateEICPlantillaPreparation.Where(u => u.fullnameLast.StartsWith(searchString)).Distinct().OrderBy(u => u.fullnameLast).ToPagedList(pageNumber, pageSize));
            }
            return View(vDuplicateEICPlantillaPreparation.OrderBy(u => u.fullnameLast).ToPagedList(pageNumber, pageSize));

            /*return View(_db.vDuplicateEICPlantillaPreparation.OrderBy(r=>r.fullnameLast).ToList());*/
        }

        public ActionResult Details(string id)
        {
            // query all plantilla preparation records per EIC
            IEnumerable<tappPreparation> tappPreparations = _db.tappPreparations.Where(r=>r.AIC==id);
            ViewBag.tappPreparations = tappPreparations;

            var empRec = _db.vDuplicateEICPlantillaPreparations.SingleOrDefault(r => r.EIC == id);
            var fullname = empRec != null ? empRec.fullnameLast : "";
            ViewBag.Fullname = fullname;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            tappPreparation tappPreparation = _db.tappPreparations.Find(id);
            _db.tappPreparations.Remove(tappPreparation);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}