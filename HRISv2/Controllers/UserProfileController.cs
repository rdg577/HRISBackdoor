using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HRISv2.Models;
using PagedList;

namespace HRISv2.Controllers
{
    public class UserProfileController : Controller
    {
        private HRISEntities db = new HRISEntities();

        // GET: UserProfile
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

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            var vUserProfile = db.vUserProfile;

            // search
            if (!string.IsNullOrEmpty(searchString))
            {
                return View(vUserProfile.Where(u => u.Fullname.StartsWith(searchString)).Distinct().OrderBy(u => u.Fullname).ToPagedList(pageNumber, pageSize));
            }
            return View(vUserProfile.OrderBy(u => u.Fullname).ToPagedList(pageNumber, pageSize));
        }

        // GET: UserProfile/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vUserProfile vUserProfile = db.vUserProfile.FirstOrDefault(g => g.EIC == id);
            if (vUserProfile == null)
            {
                return HttpNotFound();
            }
            return View(vUserProfile);
        }

        //// GET: UserProfile/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: UserProfile/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "idNo,EIC,fullnameLast,birthdate,birthplace,Position,subPositionName,officeName,statusName,BATS_Area,WarmBody,Attnd_Scheme")] vUserProfile vUserProfile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.vUserProfile.Add(vUserProfile);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(vUserProfile);
        //}

        // GET: UserProfile/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vUserProfile vUserProfile = db.vUserProfile.FirstOrDefault(g => g.EIC == id);
            if (vUserProfile == null)
            {
                return HttpNotFound();
            }

            var listEmpGroup = db.trefEmpGroup.ToList();
            ViewBag.listEmpGroup = listEmpGroup;

            var listStationArea = db.tAttStationArea.ToList();
            ViewBag.listStationArea = listStationArea;

            var listAttendanceScheme = db.tAttScheme.ToList();
            ViewBag.listAttendanceScheme = listAttendanceScheme;

            return View(vUserProfile);
        }

        // POST: UserProfile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string EIC, string empGroupCode, string AreaID, string SchemeCode)
        {
            try
            {
                // fetch record
                if (db.tappEmpGroup.SingleOrDefault(r => r.EIC == EIC) == null)
                {
                    tappEmpGroup empGroup = new tappEmpGroup();
                    empGroup.EIC = EIC;
                    empGroup.empGroupCode = empGroupCode;
                    db.tappEmpGroup.Add(empGroup);
                }
                else
                {
                    // update group code/name
                    tappEmpGroup empGroup = db.tappEmpGroup.Single(r => r.EIC == EIC);
                    empGroup.empGroupCode = empGroupCode;
                }

                if (db.tAttEmpScheme.SingleOrDefault(r => r.EIC == EIC) == null)
                {
                    tAttEmpScheme attEmpScheme = new tAttEmpScheme();
                    attEmpScheme.EIC = EIC;
                    attEmpScheme.SchemeCode = SchemeCode;
                    db.tAttEmpScheme.Add(attEmpScheme);
                }
                else
                {
                    // update attendance scheme
                    tAttEmpScheme attndScheme = db.tAttEmpScheme.Single(r => r.EIC == EIC);
                    attndScheme.SchemeCode = SchemeCode;
                }

                if (db.tAttEmpArea.SingleOrDefault(r => r.EIC == EIC) == null)
                {
                    tAttEmpArea areaBATS = new tAttEmpArea();
                    areaBATS.EIC = EIC;
                    areaBATS.AreaID = AreaID;
                    db.tAttEmpArea.Add(areaBATS);

                }
                else
                {
                    // update attendance scheme
                    tAttEmpArea areaBATS = db.tAttEmpArea.SingleOrDefault(r => r.EIC == EIC);
                    areaBATS.AreaID = AreaID;
                }

                // save all changes
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                vUserProfile vUserProfile = db.vUserProfile.Single(r => r.EIC == EIC);
                return View(vUserProfile);
            }
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
