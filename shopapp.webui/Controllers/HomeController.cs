using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;

namespace shopapp.webui.Controllers
{
    
    public class HomeController : Controller
    {
        private IProductService _productservice;

        public HomeController(IProductService productService)
        {
            _productservice = productService;
        }
       
        public IActionResult Index()
        { 
            int time =DateTime.Now.Hour;
            string message = time<5?"Goodnight":time<12?"Good Morning": time<19? "Good Afternoon":"Good Evening";
            TempData["Greeting"] = message;
            TempData["Username"] = User.Identity.Name??"Stranger";
            TempData["Info"] = "Shopapp created by Alper KAMA for self practice.Register,confirm your email and enjoy your shopping.Also if you want to go deeper you can login as a admin.Remember username and password are 'admin'.";

            var products = _productservice.GetHomePageProducts();
    


            return View(products);
        }

        public IActionResult About()
        { 
            return View();
        }
         public IActionResult Contact()
        { 
            return View("MyView");
        }
        
    }
}