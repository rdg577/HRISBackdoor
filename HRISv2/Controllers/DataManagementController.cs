using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HRISv2.Models;
using PagedList;

namespace HRISv2.Controllers
{
    public class DataManagementController : Controller
    {
        private HRISEntities db = new HRISEntities();
        private GENERALEntities dbGeneral = new GENERALEntities();

        // GET: DataManagement
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

            var vUserProfile = db.vUserProfiles;
            var tappEmployee = db.tappEmployees;

            ViewBag.vUserProfile = vUserProfile;

            // search
            if (!string.IsNullOrEmpty(searchString))
            {
                return View(tappEmployee.Where(u => u.fullnameLast.StartsWith(searchString)).Distinct().OrderBy(u => u.fullnameLast).ToPagedList(pageNumber, pageSize));
            }
            return View(tappEmployee.OrderBy(u => u.fullnameLast).ToPagedList(pageNumber, pageSize));
        }


        // GET: EditUserProfile/5
        public ActionResult EditUserProfile(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vUserProfile vUserProfile = db.vUserProfiles.FirstOrDefault(g => g.EIC == id);
            if (vUserProfile == null)
            {
                ViewBag.ErrorMessage = "Record cannot be found.";
                return RedirectToAction("Index");
                return HttpNotFound("Record cannot be found.");
            }

            var listEmpGroup = db.trefEmpGroups.ToList();
            ViewBag.listEmpGroup = listEmpGroup;

            var listStationArea = db.tAttStationAreas.ToList();
            ViewBag.listStationArea = listStationArea;

            var listAttendanceScheme = db.tAttSchemes.ToList();
            ViewBag.listAttendanceScheme = listAttendanceScheme;

            return View(vUserProfile);
        }

        // POST: EditUserProfile/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("EditUserProfile")]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserProfile(string EIC, string empGroupCode, string AreaID, string SchemeCode)
        {
            try
            {
                // fetch record
                if (db.tappEmpGroups.SingleOrDefault(r => r.EIC == EIC) == null)
                {
                    tappEmpGroup empGroup = new tappEmpGroup();
                    empGroup.EIC = EIC;
                    empGroup.empGroupCode = empGroupCode;
                    db.tappEmpGroups.Add(empGroup);
                }
                else
                {
                    // update group code/name
                    tappEmpGroup empGroup = db.tappEmpGroups.Single(r => r.EIC == EIC);
                    empGroup.empGroupCode = empGroupCode;
                }

                if (db.tAttEmpSchemes.SingleOrDefault(r => r.EIC == EIC) == null)
                {
                    tAttEmpScheme attEmpScheme = new tAttEmpScheme();
                    attEmpScheme.EIC = EIC;
                    attEmpScheme.SchemeCode = SchemeCode;
                    db.tAttEmpSchemes.Add(attEmpScheme);
                }
                else
                {
                    // update attendance scheme
                    tAttEmpScheme attndScheme = db.tAttEmpSchemes.Single(r => r.EIC == EIC);
                    attndScheme.SchemeCode = SchemeCode;
                }

                if (db.tAttEmpAreas.SingleOrDefault(r => r.EIC == EIC) == null)
                {
                    tAttEmpArea areaBATS = new tAttEmpArea();
                    areaBATS.EIC = EIC;
                    areaBATS.AreaID = AreaID;
                    db.tAttEmpAreas.Add(areaBATS);

                }
                else
                {
                    // update attendance scheme
                    tAttEmpArea areaBATS = db.tAttEmpAreas.SingleOrDefault(r => r.EIC == EIC);
                    areaBATS.AreaID = AreaID;
                }

                // save all changes
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                vUserProfile vUserProfile = db.vUserProfiles.Single(r => r.EIC == EIC);
                return View(vUserProfile);
            }
        }

        // GET: DetailsUserProfile/5
        public ActionResult DetailsUserProfile(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vUserProfile vUserProfile = db.vUserProfiles.FirstOrDefault(g => g.EIC == id);
            if (vUserProfile == null)
            {
                return HttpNotFound();
            }
            return View(vUserProfile);
        }

        // GET: RemoveLoginAccount/5
        public ActionResult RemoveLoginAccount(string id)
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

        // POST: RemoveLoginAccount/5
        [HttpPost, ActionName("RemoveLoginAccount")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAccount(string id)
        {
            tlogUser logUser = dbGeneral.tlogUser.Single(u => u.EIC == id);
            dbGeneral.tlogUser.Remove(logUser);
            dbGeneral.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: DisplayServiceRecord/5
        public ActionResult DisplayServiceRecord(String id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // query employee service record
            IEnumerable<fnGetEmployeeServiceRecords_Result> employeeServiceRecord = db.fnGetEmployeeServiceRecords(id).ToList();

            // if there is no service record, redirect to
            // create service record form
            if (!employeeServiceRecord.Any())
            {
                var employee = db.tappEmployees.SingleOrDefault(r => r.EIC == id);
                ViewBag.employee = employee;
                return View("CreateServiceRecord");
            }

            ViewBag.Fullname = employeeServiceRecord.First().Fullname;

            // all employee services
            ViewBag.employeeServiceRecord = employeeServiceRecord.OrderByDescending(g => g.dateFrom);

            return View();

        }

        // POST: DisplayServiceRecord/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteServiceRecord(int id)
        {
            tappServiceRecord empServiceRecord = db.tappServiceRecords.Find(id);
            db.tappServiceRecords.Remove(empServiceRecord);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: DetailsPlantilla/5
        public ActionResult DetailsPlantilla(string id)
        {
            // query all plantilla preparation records per EIC
            IEnumerable<tappPreparation> tappPreparations = db.tappPreparations.Where(r => r.AIC == id);
            ViewBag.tappPreparations = tappPreparations;

            var empRec = db.vDuplicateEICPlantillaPreparations.SingleOrDefault(r => r.EIC == id);
            var fullname = empRec != null ? empRec.fullnameLast : "";
            ViewBag.Fullname = fullname;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePlatilla(int id)
        {
            tappPreparation tappPreparation = db.tappPreparations.Find(id);
            db.tappPreparations.Remove(tappPreparation);
            db.SaveChanges();

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