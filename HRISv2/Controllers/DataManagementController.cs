﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
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

            var vUserProfile = db.vUserProfile;

            // search
            if (!string.IsNullOrEmpty(searchString))
            {
                return View(vUserProfile.Where(u => u.Fullname.StartsWith(searchString)).Distinct().OrderBy(u => u.Fullname).ToPagedList(pageNumber, pageSize));
            }
            return View(vUserProfile.OrderBy(u => u.Fullname).ToPagedList(pageNumber, pageSize));
        }

        // GET: EditUserProfile/5
        public ActionResult EditUserProfile(string id)
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

        // POST: EditUserProfile/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("EditUserProfile")]
        [ValidateAntiForgeryToken]
        public ActionResult EditEditUserProfile(string EIC, string empGroupCode, string AreaID, string SchemeCode)
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

        // GET: DetailsUserProfile/5
        public ActionResult DetailsUserProfile(string id)
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

        // GET: RemoveLoginAccount/5
        public ActionResult RemoveLoginAccount(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vUserProfileWithUsername vUserProfileWithUsername = db.vUserProfileWithUsername.Find(id);
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

            if (!employeeServiceRecord.Any())
            {
                return HttpNotFound();
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
            tappServiceRecord empServiceRecord = db.tappServiceRecord.Find(id);
            db.tappServiceRecord.Remove(empServiceRecord);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: DetailsPlantilla/5
        public ActionResult DetailsPlantilla(string id)
        {
            // query all plantilla preparation records per EIC
            IEnumerable<tappPreparation> tappPreparations = db.tappPreparation.Where(r => r.AIC == id);
            ViewBag.tappPreparations = tappPreparations;

            var empRec = db.vDuplicateEICPlantillaPreparation.SingleOrDefault(r => r.EIC == id);
            var fullname = empRec != null ? empRec.fullnameLast : "";
            ViewBag.Fullname = fullname;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePlatilla(int id)
        {
            tappPreparation tappPreparation = db.tappPreparation.Find(id);
            db.tappPreparation.Remove(tappPreparation);
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