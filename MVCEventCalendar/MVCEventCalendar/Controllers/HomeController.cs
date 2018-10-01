using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCEventCalendar.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            using (CalendarDataBaseEntities cd = new CalendarDataBaseEntities())
            {
                var events = cd.Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveEvent(Events e)
        {
            var status = false;
            using (CalendarDataBaseEntities cd = new CalendarDataBaseEntities())
            {
                if (e.EventID > 0)
                {
                    var v = cd.Events.Where (a=>a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        //v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    cd.Events.Add(e);
                }
                cd.SaveChanges();
                status = true;
            }

                return new JsonResult { Data = new { status = status } };
        }
        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;

            using (CalendarDataBaseEntities cd = new CalendarDataBaseEntities())
            {
                var v = cd.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    cd.Events.Remove(v);
                    cd.SaveChanges();
                    status = true;
                }
            }
                return new JsonResult { Data = new { status = status } };
        }
    }
}