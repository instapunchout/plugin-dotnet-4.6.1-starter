using System.Web.Mvc;


namespace InstaPunchout.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }



    }

    public class Customer
    {
       // TODO : should be replaced with native ecommerce customer object
    }



    public class Cart
    {
        // TODO : should be replaced with native ecommerce cart object
    }

}