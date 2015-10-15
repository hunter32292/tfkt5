using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mélodie.Models;
using System.Collections;
using System.Text.RegularExpressions;
using Excel;

namespace Mélodie.Controllers
{
    public class UsersController : Controller
    {
        private MélodieContext db = new MélodieContext();

        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }

        // GET: /Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = await db.Users.FindAsync(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: /Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // Dylan added this
        // "Good work" - John
        public async Task<ActionResult> Import()
        {
         
					  ArrayList newUsers = new ArrayList();
						// Filter Regex
            string regex = "^[a-z,A-Z,0-9]*@uwec.edu$";
            Regex r = new Regex(regex);
						// Parse excel document
						

						// Base controller code and view code off of:
						// https://cmatskas.com/upload-files-in-asp-net-mvc-with-javascript-and-c/
            foreach (var worksheet in Workbook.Worksheets(@"H:\login.xlsx"))
            {
                foreach (var row in worksheet.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (cell != null)
                        {
                            Match m = r.Match(cell.Text);
                            if (m.Success)
                            {
                                list.Add(cell.Text);
                            }
                        }

                    }
                }
            }



            for (int x = 0; x < list.Count; x++)
            {
                Users user = new Users();
                user.email = (String)list[x];
                user.role_id = 1;
                db.Users.Add(user);

            }

            //stuff to make the computer stop yelling at me
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // POST: /Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,username,email,password_salt,password_hash,role_id")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(users);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: /Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = await db.Users.FindAsync(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: /Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,username,email,password_salt,password_hash,role_id")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = await db.Users.FindAsync(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Users users = await db.Users.FindAsync(id);
            db.Users.Remove(users);
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
