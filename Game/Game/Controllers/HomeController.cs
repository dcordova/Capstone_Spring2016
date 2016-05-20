using ReviewsApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace ReviewsApp.Controllers
{
    public class HomeController : Controller
    {
        public  readonly MyDbContext db;
        private  readonly UserManager<ApplicationUser> manager;

        public HomeController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new MyDbContext()));
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Game()
        {
            
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var scores = db.Scores.Where(x => x.UserId == currentUser.MyUserInfo.Id);
            
            var rawScores = new List<int>(new int[100]);
            int i = 0;

            scores.ForEach(x => rawScores[i++] = x.Points);
            return View(rawScores.OrderByDescending(x => x));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateScore(int newScore)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = currentUser.UserName, };
              
                var score = new Score { Date = DateTime.Now, Points = newScore, UserId = currentUser.MyUserInfo.Id};
                
                try
                {
                    db.Scores.Add(score);
                    await db.SaveChangesAsync();
                    
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return RedirectToAction("Game", "Home");
            }


            return null;
        }

        // Only Authenticated users can access thier profile
        [Authorize]
        public ActionResult Profile()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId()); 
            
            
            // Recover the profile information about the logged in user
            ViewBag.HomeTown = currentUser.HomeTown;
            ViewBag.FirstName = currentUser.MyUserInfo.FirstName;

            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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