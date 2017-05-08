using System.Linq;
using System.Net;
using System.Web.Mvc;
using HRISv2.Models;
using PagedList;

namespace HRISv2.Controllers
{
    public class UserLogController : Controller
    {
        private HRISEntities db = new HRISEntities();
        private GENERALEntities dbGeneral = new GENERALEntities();

        // GET: UserLog
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

            const int pageSize = 20;
            int pageNumber = (page ?? 1);

            var userProfileWithUsername = db.vUserProfileWithUsernames;

            // search
            if (!string.IsNullOrEmpty(searchString))
            {
                return View(userProfileWithUsername.Where(u => u.Fullname.StartsWith(searchString)).Distinct().OrderBy(u => u.Fullname).ToPagedList(pageNumber, pageSize));
            }
            return View(userProfileWithUsername.OrderBy(u => u.Fullname).ToPagedList(pageNumber, pageSize));
            
        }

        // GET: UserLog/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vUserProfileWithUsername vUserProfileWithUsername = db.vUserProfileWithUsernames.Find(id);
            if (vUserProfileWithUsername == null)
            {
                return HttpNotFound();
            }
            return View(vUserProfileWithUsername);
        }

        // GET: UserLog/Reset/5
        public ActionResult Reset(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vUserProfileWithUsername vUserProfileWithUsername = db.vUserProfileWithUsernames.Find(id);
            if (vUserProfileWithUsername == null)
            {
                return HttpNotFound();
            }
            return View(vUserProfileWithUsername);
        }

        // POST: UserLog/Reset/5
        [HttpPost, ActionName("Reset")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetConfirmed(string id)
        {
            tlogUser logUser = dbGeneral.tlogUser.Single(u => u.EIC == id);
            dbGeneral.tlogUser.Remove(logUser);
            dbGeneral.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
