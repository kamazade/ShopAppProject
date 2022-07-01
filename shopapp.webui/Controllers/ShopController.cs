using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.webui.Extensions;
using shopapp.webui.Models;


namespace shopapp.webui.Controllers
{
    public class ShopController:Controller
    {
        private IProductService _productService; 
        public ShopController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult List(string category,int page=1) 
        {
            var pageSize = 3;
            var productViewListModel = new ProductListViewModel(){
                PageInfo = new PageInfo(){
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurentCategory=category
                },
                Products=_productService.GetProductsByCategory(category,page,pageSize)
            };
            if(String.IsNullOrEmpty(category)){
                TempData.Put("sliderInfo",new SliderModel(){
                    Urls = new List<string>(){
                        "slider1.png",
                        "slider2.png",
                        "slider3.png"
                    }
                }); 
            }
            else{
                TempData.Put("sliderInfo",new SliderModel(){
                    Urls = new List<string>(){
                        "slideiphone1.jpg",
                        "slideiphone2.jpg",
                        "slideiphone3.jpg"
                    }
                }); 
            }
            return View(productViewListModel);
        }
        public IActionResult Details(string url) 
        {   
            
            if(url!=null)
            {
                var productDetailModel = new ProductDetailModel(){
                    Product = _productService.GetProductDetails(url),
                    Categories = _productService.GetProductDetails(url)
                            .ProductCategories
                            .Select(i=> i.Category)
                            .ToList()
                };

                return View(productDetailModel);
            }
            return NotFound();
            
        }

        public IActionResult Search(string q)
        {
            var productViewListModel = new ProductListViewModel(){
            
                Products=_productService.GetSearchResult(q)
            };
            return View(productViewListModel);
        }
    }
}