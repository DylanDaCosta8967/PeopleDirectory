using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PeopleDirectory.Context;

namespace PeopleDirectory.Controllers
{
    public class HomeController : Controller
    {
        PeopleDirectoryContext db = new PeopleDirectoryContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(PeopleDirectory.Models.Users usermodel)
        {
            var userdetails = db.Users.Where(v => v.UserName == usermodel.UserName && v.Password == usermodel.Password).FirstOrDefault();
            if (userdetails == null)
            {
                return View("Login", usermodel);
            }
            else
            {
                Session["UserId"] = userdetails.UserId;
                return RedirectToAction("Index", "PeopleDirectory");
            }
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "PeopleDirectory");
        }
    }
}