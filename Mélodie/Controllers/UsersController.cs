using System;
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
        [HttpPost]
        public ActionResult Import()
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

                    var path = Path.Combine("H:\\tfkt5\\Mélodie\\excel\\", fileName);//H:\tfkt5\Mélodie\excel\
                    file.SaveAs(path);
                    foreach (var worksheet in Workbook.Worksheets("H:\\tfkt5\\Mélodie\\excel\\" + fileName))
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
                user.role_id = 1;
                db.Users.Add(user);
                db.SaveChanges();

            }

            //stuff to make the computer stop yelling at me
            
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


        private void sendMailToRecipient(Users user)
        {
            // create the message that is to be mailed
            MailMessage mail = new MailMessage();

            // TODO - add proper address for email here
            mail.From = new MailAddress("sender email address here");
            mail.To.Add(new MailAddress(user.email));

            mail.Subject = "Thank you for your registration to Mèlodie Maker!";

            // set to true for now 
            Boolean isGenerated = true;
            // generate the body of the email
            string body = GenerateEmailBody(isGenerated, user);

            mail.Body = body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new System.Net.NetworkCredential("sender email address here", "sender email password here");
            smtpClient.Send(mail);

        }

        private string GenerateEmailBody(Boolean isGenerated, Users user)
        {
            string message = "";
            if (isGenerated)
            {
                message =
                    "Hi, " + user.username + "!" + Environment.NewLine +
                    Environment.NewLine +
                    "You have been registered for Mèlodie Maker!  Please use the following to login:" + Environment.NewLine +
                    "Your new username is " + user.username + "." + Environment.NewLine +
                    "Your new password is " + GenerateRandomPassword(8) + "." + Environment.NewLine +
                    "You can change your password in Settings." + Environment.NewLine +
                    Environment.NewLine +
                    "This is a no-reply email.  For general inquiries, please send an email to " + "our email here" + ".";

            }
            else
            {
                message =
                    "Hi, " + user.username + "!" + Environment.NewLine +
                    Environment.NewLine +
                    "Your registration for Mèlodie Maker has been succesful.  Thank you!" + Environment.NewLine +
                    Environment.NewLine +
                    "This is a no-reply email.  For general inquiries, please send an email to " + "our email here" + ".";
            }
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
        }​


    }
}
