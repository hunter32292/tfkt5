﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Mélodie.Models;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Excel;
using System.IO;
using Microsoft.AspNet.Identity;

namespace Mélodie.Controllers
{
    // Add login authorization
    [Authorize]
    public class UsersController : Controller
    {
        private MélodieContext db = new MélodieContext();

        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View(await UserToListAsync());
        }

        public async Task<List<Users>> UserToListAsync()
        {
            MélodieContext mc = new MélodieContext();
            var query = from u in mc.Users
                        orderby u.username ascending
                        select u;
            return await query.ToListAsync();
        }
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
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
        [HttpPost]
        public async Task<ActionResult> Import()
        {

            ArrayList newUsers = new ArrayList();
            //newUsers.Add("hello");
            // Filter Regex
            string regex = "^[a-z,A-Z,0-9]*@uwec.edu$";
            Regex r = new Regex(regex);
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.GetFullPath(Server.MapPath("~"));
                    path = Path.Combine(path , fileName); //H:\tfkt5\Mélodie\excel\
                    file.SaveAs(path);
                    foreach (var worksheet in Workbook.Worksheets(path))
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
                                        newUsers.Add(cell.Text);
                                    }
                                }

                            }
                        }
                    }

                }
            }

            for (int x = 0; x < newUsers.Count; x++)
            {
                Users user = new Users();

                user.email = (String)newUsers[x];
                user.role_id = "Student";
                // generate a random password for the user
                AccountController ac = new AccountController();
                string password = GenerateRandomPassword(8);
                RegisterViewModel model = new RegisterViewModel();
                model.UserName = user.email.Substring(0, user.email.IndexOf('@'));
                model.Email = user.email;
                model.role_id = user.role_id;
                // notify the user that an account has been created in their name
                await ac.Register(model);
            }

            //stuff to make the computer stop yelling at me

            return RedirectToAction("Index");

        }


        // POST: /Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,username,email,role_id")] Users users, RegisterViewModel model)
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
        public async Task<ActionResult> Edit(string id)
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
        public async Task<ActionResult> Edit([Bind(Include = "ID,username,email,role_id")] Users users)
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
        public async Task<ActionResult> Delete(string id)
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
        public async Task<ActionResult> DeleteConfirmed(string id)
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

        // Sender email is hard coded in for now
        //  address:    tfkt5melodiemaker@gmail.com
        //  password:   pieceofcake
        public void sendMailToRecipient(ApplicationUser user, string password)
        {
            // create the message that is to be mailed
            MailMessage mail = new MailMessage();

            // TODO - add proper address for email here
            mail.From = new MailAddress("tfkt5melodiemaker@gmail.com");
            mail.To.Add(new MailAddress(user.Email));

            mail.Subject = "You have been registered for Mèlodie Maker!";

            // generate the body of the email
            string body = GenerateEmailBody(password, user);

            mail.Body = body;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("tfkt5melodiemaker@gmail.com", "pieceofcake")
            };
            smtp.Send(mail);

        }

        private string GenerateEmailBody(string password, ApplicationUser user)
        {
            string message = "";
            message =
                    "Hi, " + user.UserName + "!" + Environment.NewLine +
                    Environment.NewLine +
                    "You have been registered for Mèlodie Maker!  Please use the following to login:" + Environment.NewLine +
                    "Your new username is " + user.UserName + "." + Environment.NewLine +
                    "Your new password is " + password + "." + Environment.NewLine +
                    "You can change your password in Settings." + Environment.NewLine +
                    Environment.NewLine +
                    "This is a no-reply email.  For general inquiries, please send an email to musicandtheatre@uwec.edu.";
            return message;
        }

        private string GenerateRandomPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder password = new StringBuilder();
            Random rnd = new Random();

            while (0 < length)
            {
                password.Append(valid[rnd.Next(valid.Length)]);
                length--;
            }

            return password.ToString();
        }

    }
}
