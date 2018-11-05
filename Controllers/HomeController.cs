using MasstransitRBMQPublishRate.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasstransitRBMQPublishRate.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            try
            {
                string jsonRequestData = "{ \"contacts\": [ { \"id\": 7113, \"name\": \"James Norris\", \"birthDate\": \"1977-05-13T00:00:00\", \"phone\": \"488-555-1212\", \"address\": { \"postalCode\": \"92115\", \"street\": \"4627 Sunset Ave\", \"city\": \"San Diego\", \"state\": \"CA\" } }, { \"id\": 7114, \"name\": \"Mary Lamb\", \"birthDate\": \"1974-10-21T00:00:00\", \"phone\": \"337-555-1212\", \"address\": { \"postalCode\": \"49245\", \"street\": \"1111 Industrial Way\", \"city\": \"Dallas\", \"state\": \"TX\" } }, { \"id\": 7115, \"name\": \"Robert Shoemaker\", \"birthDate\": \"1968-02-08T00:00:00\", \"phone\": \"643-555-1212\", \"address\": null } ] }";
                var requestEntity = JsonConvert.DeserializeObject<Contact>(jsonRequestData);
                var starttime = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");                
                for (int i = 0; i < 5; i++)
                {
                    QueueConfig.busControl.Publish<IContact>(requestEntity);
                }

                var endtime = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");

                Response.Write((DateTime.Parse(endtime) - DateTime.Parse(starttime)).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                throw;
            }


            return View();
        }
    }
}
