﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mélodie.Models;

namespace Mélodie.Controllers
{
    // Add login authorization
    [Authorize]
    public class CoursesController : Controller
    {
        private MélodieContext db = new MélodieContext();

        // GET: Courses
        public async Task<ActionResult> Index()
        {

            return View(await db.Course.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Courses courses = await db.Course.FindAsync(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            // Auth function
            if (Request.Cookies["Role"] != null && !Server.HtmlEncode(Request.Cookies["Role"].Value).Equals("Instructor"))
            {
                return RedirectToAction("Index");
            }
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in db.Users.Where(a => a.role_id.Equals("Instructor")))
            {
                SelectListItem s = new SelectListItem
                {
                    Value = item.ID,
                    Text = item.username
                };

                items.Add(s);

            }
            ViewBag.user_id = items;
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,title,user_id,description")] Courses courses)
        {
            // Auth function
            if (Request.Cookies["Role"] != null && !Server.HtmlEncode(Request.Cookies["Role"].Value).Equals("Instructor"))
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                db.Course.Add(courses);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(courses);
        }

        // GET: Courses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            // Auth function
            if (Request.Cookies["Role"] != null && !Server.HtmlEncode(Request.Cookies["Role"].Value).Equals("Instructor"))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SelectListItem> items = new List<SelectListItem>(); 
            foreach (var item in db.Users.Where(a => a.role_id.Equals("Instructor")))
            {
               SelectListItem s = new SelectListItem{
                    Value = item.ID,
                    Text = item.username
                };

               items.Add(s);
                
            }
            ViewBag.user_id = items;
            Courses courses = await db.Course.FindAsync(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,title,user_id,description")] Courses courses)
        {
            // Auth function
            if (Request.Cookies["Role"] != null && !Server.HtmlEncode(Request.Cookies["Role"].Value).Equals("Instructor"))
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                db.Entry(courses).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(courses);
        }

        // GET: Courses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            // Auth function
            if (Request.Cookies["Role"] != null && !Server.HtmlEncode(Request.Cookies["Role"].Value).Equals("Instructor"))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Courses courses = await db.Course.FindAsync(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            // Auth function
            if (Request.Cookies["Role"] != null && !Server.HtmlEncode(Request.Cookies["Role"].Value).Equals("Instructor"))
            {
                return RedirectToAction("Index");
            }
            Courses courses = await db.Course.FindAsync(id);
            db.Course.Remove(courses);
            await db.SaveChangesAsync();
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
