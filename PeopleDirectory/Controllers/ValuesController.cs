using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PeopleDirectory.Context;

namespace PeopleDirectory.Controllers.webapi
{
    public class ValuesController : ApiController
    {
        PeopleDirectoryContext db = new PeopleDirectoryContext();

        public HttpResponseMessage Get(String term = "")
        {
            List<String> Names;
            Names = db.PersonDirectoryModel.Where(a => a.Name.ToLower().StartsWith(term.ToLower())).Select(y => y.Name).ToList();
            Names.AddRange(db.PersonDirectoryModel.Where(a => a.SurName.ToLower().StartsWith(term.ToLower())).Select(y => y.SurName).ToList());

            return Request.CreateResponse(HttpStatusCode.OK, Names);
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get(String term)
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5

        // POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}