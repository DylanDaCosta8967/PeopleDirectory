using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PeopleDirectory.Context;
using PeopleDirectory.Models;
using System.Data.Entity.Infrastructure;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace PeopleDirectory.Controllers
{
    public class PeopleDirectoryController : Controller
    {
       PeopleDirectoryContext db = new PeopleDirectoryContext();

        [HttpGet]
        public ActionResult Index()
        {
            List<Country> allCountry = new List<Country>();
            List<City> allCity = new List<City>();
            List<PersonDirectoryModel> allClients = new List<PersonDirectoryModel>();

            allCountry = db.Countries.OrderBy(a => a.CountryName).ToList();
            allCity = db.Cities.OrderBy(a => a.CityName).ToList();
            allClients = db.PersonDirectoryModel.OrderBy(a => a.Name).ToList();

            ViewBag.clients = new SelectList(allClients, "ClientId", "Name");
            ViewBag.CountryId = new SelectList(allCountry, "CountryId", "CountryName");
            ViewBag.CityId = new SelectList(allCity, "CityId", "CityName");
            return View(db.PersonDirectoryModel.ToList());
        }

        // Post: PeopleDirectory

        //public JsonResult IndexJ( String term)
        //{
        //    List<String> Names;
        //    Names = db.PersonDirectoryModel.Where(a => a.Name.ToLower().StartsWith(term.ToLower())).Select(y=>y.Name).ToList();
        //    Names.AddRange(db.PersonDirectoryModel.Where(a => a.SurName.ToLower().StartsWith(term.ToLower())).Select(y => y.SurName).ToList());
        //    return Json(Names, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult IndexJ(String term)
        {
             List <String> Names = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:64919/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Values?term="+term);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<string>>();
                    readTask.Wait();

                    Names = readTask.Result;
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, "Server API error. Please contact administrator.");
                }
            }
            return Json(Names, JsonRequestBehavior.AllowGet);
        }




        // Post: PeopleDirectory
        [HttpPost]
        public ActionResult Index(int? CountrySelect, int? CitySelect, String SearchNameTerm, int? ClientId)
        {

            List<Country> allCountry = new List<Country>();
            List<City> allCity = new List<City>();

            allCountry = db.Countries.OrderBy(a => a.CountryName).ToList();
            allCity = db.Cities.OrderBy(a => a.CityName).ToList();

            ViewBag.CountryId = new SelectList(allCountry, "CountryId", "CountryName");
            ViewBag.CityId = new SelectList(allCity, "CityId", "CityName");
            
            if (CountrySelect!= null && CountrySelect > 0 || CitySelect != null && CitySelect > 0)
            {
                var ClientCity = db.PersonDirectoryModel.ToList()
                .Where(p => (p.ClientCityId == CitySelect) || (p.ClientCountryId == CountrySelect)
                && (p.Name.ToLower().Contains(SearchNameTerm.ToLower())
                || p.SurName.ToLower().Contains(SearchNameTerm.ToLower())));
                return View(ClientCity);
            }
            else
            {

                var Client = db.PersonDirectoryModel.ToList()
                .Where(p => p.Name.ToLower().Contains(SearchNameTerm.ToLower())
                || p.SurName.ToLower().Contains(SearchNameTerm.ToLower()));
                return View(Client);
            }

          
        }
        //public JsonResult GetCityList(int ClientCountyId)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    List<City> CityList = db.Cities.Where(x => x.CountryId == ClientCountyId).ToList();
        //    return Json(CityList, JsonRequestBehavior.AllowGet);
        //}

        // GET: PeopleDirectory/Details/
        public ActionResult Details(int id)
        {
            var Client = db.PersonDirectoryModel.Where(p => p.ClientId == id).FirstOrDefault();
            City ct = db.Cities.Where(a => a.CityId == Client.ClientCityId).FirstOrDefault();
            Country co =  db.Countries.Where(a => a.CountryId == Client.ClientCountryId).FirstOrDefault();
            ViewBag.CityName = ct.CityName;
            ViewBag.CountryName = co.CountryName;
            return View(Client);
        }

        // GET: PeopleDirectory/Create
        public ActionResult Create()

        {
            List<Country> allCountry = new List<Country>();
            List<City> allCity = new List<City>();

            allCountry = db.Countries.OrderBy(a => a.CountryName).ToList();
            allCity = db.Cities.OrderBy(a => a.CityName).ToList();

            ViewBag.CountryId = new SelectList(allCountry, "CountryId", "CountryName");
            ViewBag.CityId = new SelectList(allCity, "CityId", "CityName");
           
            return View();
        }

        // POST: PeopleDirectory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonDirectoryModel model, HttpPostedFileBase Image1)
        {

            List<Country> allCountry = new List<Country>();
            List<City> allCity = new List<City>();

            allCountry = db.Countries.OrderBy(a => a.CountryName).ToList();

            if (model != null && model.ClientCountryId != 0)
            {
                allCity = db.Cities.Where(a => a.CountryId == model.ClientCountryId).OrderBy(a => a.CityName).ToList();
            }

            ViewBag.CountryId = new SelectList(allCountry, "CountryId", "CountryName", model.ClientCountryId);
            ViewBag.CityId = new SelectList(allCity, "CityId", "CityName", model.ClientCityId);

            try
            {
                if(Image1!=null)
                {
                    model.ProfilePic = new byte[Image1.ContentLength];
                    Image1.InputStream.Read(model.ProfilePic, 0, Image1.ContentLength);
                }
                db.PersonDirectoryModel.Add(model);
                db.SaveChanges();
                ModelState.Clear();
                model = null;
                ViewBag.Message = "Successfully Submited";
                return View(model);
            }
            catch
            {
                ViewBag.Message = "UnSuccessful";
                return View();
            }
        }

        [HttpGet]
        public JsonResult GetCities(String ClientCountryID = "")
        {
            List<City> allCity = new List<City>();
            int ID = 0;
            if (int.TryParse(ClientCountryID, out ID))
            {
                allCity = db.Cities.Where(a => a.CountryId.Equals(ID)).OrderBy(a => a.CityName).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return new JsonResult
                {
                    Data = allCity,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }else
            {
                return new JsonResult
                {
                    Data = "Not Valid Request",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }


        public ActionResult AddImage(PersonDirectoryModel model)
        {
            PersonDirectoryModel Pd = new PersonDirectoryModel();
            return View(Pd);
        }

        // GET: PeopleDirectory/Edit/
        public ActionResult Edit(int? id)
       {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<Country> allCountry = new List<Country>();
            List<City> allCity = new List<City>();

            allCountry = db.Countries.OrderBy(a => a.CountryName).ToList();
            allCity = db.Cities.OrderBy(a => a.CityName).ToList();

            ViewBag.CountryId = new SelectList(allCountry, "CountryId", "CountryName");
            ViewBag.CityId = new SelectList(allCity, "CityId", "CityName");

            PersonDirectoryModel people = db.PersonDirectoryModel.Find(id);
            if (people == null)
            {
                return HttpNotFound();
            }
            return View(people);
        }


        // POST: PeopleDirectory/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> Edit(int? id, PersonDirectoryModel model, HttpPostedFileBase Image1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonDirectoryModel people = db.PersonDirectoryModel.Find(id);

            List<Country> allCountry = new List<Country>();
            List<City> allCity = new List<City>();

            allCountry = db.Countries.OrderBy(a => a.CountryName).ToList();
            allCity = db.Cities.OrderBy(a => a.CityName).ToList();

            ViewBag.CountryId = new SelectList(allCountry, "CountryId", "CountryName");
            ViewBag.CityId = new SelectList(allCity, "CityId", "CityName");


            if (ModelState.IsValid) 
            {
                try
                {
                    

                    City Citydata = db.Cities.Find(people.ClientCityId);
                    Country Countrydata = db.Countries.Find(people.ClientCountryId);
                    City CitydataModel = db.Cities.Find(model.ClientCityId);
                    Country CountrydataModel = db.Countries.Find(model.ClientCountryId);


                    var body = "<p>The Below Person Was Editied: </p>" +
                        "<p><b>Old Person:</b> <br>" +
                        "Name: {0} <br>" +
                        "SurName: {1} <br>" +
                        "Mobile Number: {2} <br>" +
                        "Gender: {3} <br>" +
                        "Email Address: {4} <br>" +
                        "Country: {5} <br>" +
                        "City: {6} <br></p>" +

                        "<p><b>New Person:</b>  <br>" +
                        "Name: {7} <br>" +
                        "SurName: {8} <br>" +
                        "Mobile Number: {9} <br>" +
                        "Gender: {10} <br>" +
                        "Email Address: {11} <br>" +
                        "Country: {12} <br>" +
                        "City: {13} </p>"
                        ;

                    var message = new MailMessage();
                    message.To.Add(new MailAddress("mark@bluegrassdigital.com"));  // replace with valid value mark@bluegrassdigital.com
                    message.From = new MailAddress("dylan.dacosta8967@gmail.com");
                    message.Subject = "People Directory Update";
                    message.Body = string.Format(body, people.Name, people.SurName, people.MobileNo, people.Gender, people.EmailAddress, Countrydata.CountryName, Citydata.CityName, model.Name, model.SurName, model.MobileNo, model.Gender, model.EmailAddress, CountrydataModel.CountryName, CitydataModel.CityName);
                    message.IsBodyHtml = true;

                    if (people.ProfilePic != null)
                    {
                        model.ProfilePic = people.ProfilePic;
                        string Filename = "";
                        if (Image1 != null)
                        { Filename = "Old "; }
                        message.Attachments.Add(new Attachment(new MemoryStream(people.ProfilePic), Filename + "Profile Picture.jpg"));
                    }

                    if (Image1 != null)
                    {
                        if (people.ProfilePic != null)
                        {
                            message.Attachments.Add(new Attachment(new MemoryStream(people.ProfilePic), "Old Profile Picture.jpg"));
                        }
                        people.ProfilePic = new byte[Image1.ContentLength];
                        Image1.InputStream.Read(people.ProfilePic, 0, Image1.ContentLength);
                        message.Attachments.Add(new Attachment(new MemoryStream(people.ProfilePic), "New Profile Picture" + Image1.FileName));
                    }
                    else
                    {
                        model.ProfilePic = people.ProfilePic;
                    }

                    people.Name = model.Name;
                    people.SurName = model.SurName;
                    people.MobileNo = model.MobileNo;
                    people.Gender = model.Gender;
                    people.EmailAddress = model.EmailAddress;

                    db.SaveChanges();
                    ModelState.Clear();
                    model = null;

                    using (var smtp = new SmtpClient())
                    {

                        smtp.UseDefaultCredentials = false;

                        smtp.Credentials = new System.Net.NetworkCredential("dylan.dacosta8967@gmail.com", "666666n@lyD");
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);

                    }

                    ViewBag.Message = "Successfully Submited";
                    return RedirectToAction("Index", "PeopleDirectory");
                }
                catch (Exception e)
                {
                    model.ProfilePic = people.ProfilePic;
                    var error = e.Message;
                    ViewBag.Message = "UnSuccessful";
                    return View();
                }
            }
            else
            {
                model.ProfilePic = people.ProfilePic;
                return View(model);
            }
            
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new System.IO.FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        // GET: PeopleDirectory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            PersonDirectoryModel people = db.PersonDirectoryModel.Find(id);
            if (people == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.PersonDirectoryModel.Remove(people);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "PeopleDirectory");
        }

    }
}
