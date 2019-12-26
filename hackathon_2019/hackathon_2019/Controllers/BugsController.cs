namespace hackathon_2019.Controllers
{
    using System.Web.Mvc;

    public class BugsController : Controller
    {
        public ActionResult CreateBug()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PostBug()
        {
            return new JsonResult { Data = true };
        }

        [HttpPost]
        public ActionResult UpdateBug()
        {
            return View();
        }

        public ActionResult ListBugs()
        {
            return View();
        }

        public ActionResult ViewBug()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidateBug()
        {
            return View();
        }
    }
}