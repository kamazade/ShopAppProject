using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using shopapp.webui.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Extensions;
using Microsoft.AspNetCore.Identity;
using shopapp.webui.Identity;

namespace shopapp.webui.Controllers
{   
    //[Authorize] // Requires only login no need a role
    [Authorize(Roles ="Admin")] // Requires login and Admin Role
    public class AdminController :Controller
    {
        private IProductService _productService;
        private IOrderService _orderService;
        private ICategoryService _categoryService;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        
        
        public AdminController(
            IProductService productService,
            ICategoryService categoryService,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOrderService orderService
        )
        {
            _productService=productService;
            _categoryService = categoryService;
            _userManager = userManager;
            _roleManager = roleManager;
            _orderService = orderService;
        }
       
       
       
        //Users
        public IActionResult UserList(){
            
            return View(_userManager.Users);
        }
        
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user==null)
            {
                return NotFound();
            }
            var selectedRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.Select(i=> i.Name);
            ViewBag.Roles = roles;
            var model = new UserDetailModel(){
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                EmailConfirm = user.EmailConfirmed,
                SelectedRoles = selectedRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserDetailModel model,string[] SelectedRoles)
        {
            if(ModelState.IsValid){
                var user  = await _userManager.FindByIdAsync(model.UserId);
                if(user!= null){
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirm;

                    var result = await _userManager.UpdateAsync(user);
                    if(result.Succeeded){
                        var userRoles = await _userManager.GetRolesAsync(user);
                        SelectedRoles = SelectedRoles?? new string[]{};
                        await _userManager.AddToRolesAsync(user,SelectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(SelectedRoles).ToArray<string>());

                        return RedirectToAction("UserList","Admin");                    
                    }
                }
            }
            var roles = _roleManager.Roles.Select(i=> i.Name);
            ViewBag.Roles = roles;
            return View(model);
        }


        //Roles
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if(ModelState.IsValid){
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList","Admin");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> RoleEdit(string id){
            var role  = await _roleManager.FindByIdAsync(id);
            var members  = new List<User>();
            var nonMembers  = new List<User>();
            foreach (var user in _userManager.Users)
            {
                //     if(await _userManager.IsInRoleAsync(user,role.Name))
                //     {
                //         members.Add(user);
                //     }else{
                //         nonMembers.Add(user);
                //     }
                //
                var list = await _userManager.IsInRoleAsync(user,role.Name)
                        ?members:nonMembers;
                list.Add(user);
            }
            var model = new RoleDetailsModel(){
                Role = role,
                Members = members,
                NonMembers =nonMembers
            };

            
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if(ModelState.IsValid)
            {
                foreach (var id in model.IdsToAdd ?? new string[]{} )
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if(user != null){
                        var result = await _userManager.AddToRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                            foreach (var e in result.Errors)
                            {
                                ModelState.AddModelError("",e.Description);
                            }
                        }
                    }
                }
                foreach (var id in model.IdsToRemove ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if(user != null){
                        var result = await _userManager.RemoveFromRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                            foreach (var e in result.Errors)
                            {
                                ModelState.AddModelError("",e.Description);
                            }
                        }
                    }
                }

            }

            return Redirect("/admin/role/"+model.RoleId);
        }


        //Product
        public IActionResult ProductList() 
        {

            
            ProductListViewModel Model =new ProductListViewModel(){
                Products = _productService.GetAll()
            };
            return View(Model);
        }        
        [HttpGet]
        public IActionResult CreateProduct(){
            var categories = _categoryService.GetAll();
            ViewBag.Categories = categories;
            return View();
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductModel model,int[] categoryIds){

         
                var entity = new Product(){
                    Name = model.Name,
                    Url=model.Url,
                    Desc = model.Desc,
                    ImageUrl = model.ImageUrl,
                    Price= model.Price,

                };
                
                if(_productService.Create(entity,categoryIds)){
                    //Message
                    TempData.Put("message",new AlertMessage(){
                        Title = "Success",
                        Message = $"{entity.Name} isimli ürün eklendi",
                        AlertType = "success"
                    });
                    // CreateMessage($"{entity.Name} isimli ürün eklendi","success");
                    return RedirectToAction("ProductList");
                };
                TempData.Put("message",new AlertMessage(){
                    Title = "Danger",
                    Message = _productService.ErrorMessage,
                    AlertType = "danger"
                });
                // CreateMessage(_productService.ErrorMessage,"danger");

                ViewBag.Categories = _categoryService.GetAll();
                return View(model);

 

        }
    
        [HttpGet]
        public IActionResult EditProduct(int? id)
        {
            
            if(id == null)
            {
                return NotFound();
            }
            // var product = _productService.GetById((int)id);
            var product = _productService.GetByIdWithCategory((int)id);
            if(product == null){
                return NotFound();
            }
            var productModel = new ProductModel(){
                ProductId = product.ProductId,
                Name = product.Name,
                Url  =product.Url,
                Desc = product.Desc,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                IsApproved = product.IsApproved,
                IsHomePage = product.IsHomePage,
                SelectedCategories = product.ProductCategories.Select(pc=> pc.Category).ToList()
            };
            ViewBag.Categories = _categoryService.GetAll();
            return View(productModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel productModel,int[] categoryIds,IFormFile file)
        {
       
                var entity = _productService.GetById(productModel.ProductId);
                if(entity == null)
                {
                    return NotFound();
                } 
                entity.Name = productModel.Name;
                entity.Url = productModel.Url;
                entity.Desc = productModel.Desc;
                entity.Price = productModel.Price;
                entity.IsApproved = productModel.IsApproved;
                entity.IsHomePage = productModel.IsHomePage;
            
                
                
                if(file!=null)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var randomName = string.Format($"{Guid.NewGuid()}{extension}");
                    entity.ImageUrl = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",randomName);
                    using(var stream = new FileStream(path,FileMode.Create)){
                        await file.CopyToAsync(stream);
                    };

                }
                _productService.Update(entity,categoryIds);
                //Message
                TempData.Put("message",new AlertMessage(){
                    Title = "warning",
                    Message = $"ID:{entity.ProductId} nolu ürün güncellendi",
                    AlertType = "warning"
                });
                // CreateMessage($"ID:{entity.ProductId} nolu ürün güncellendi","warning");

                return RedirectToAction("ProductList");
    
       
        }
    
        public IActionResult DeleteProduct(int productId)
        {
            var entity = _productService.GetById(productId);
            if(entity!=null){
                _productService.Delete(entity);
            }

            //Message
            var msg =  new AlertMessage()
            {
                AlertType = "warning",
                Message =  $"ID:{entity.ProductId} nolu ürün silindi"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
            

            return RedirectToAction("ProductList");
        }
        
        
        //Category
        public IActionResult CategoryList(){
            CategoryListModel categoryList = new CategoryListModel(){
                Categories=_categoryService.GetAll()
            };
            
            return View(categoryList);
        }

        [HttpGet]
        public IActionResult CreateCategory(){
            
            return View();
        }
        
        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model){
          
                var entity = new Category(){
                Name = model.Name,
                Url = model.Url
                };
                _categoryService.Create(entity);
                //Message
                var msg =  new AlertMessage()
                {
                    AlertType = "success",
                    Message =  $"{entity.Name} isimli kategori eklendi"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg);
                return RedirectToAction("CategoryList");
       


        }

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var category = _categoryService.GetByIdWithProducts((int)id);
            // var category = _categoryService.GetById((int)id);
            var categoryModel = new CategoryModel(){
                CategoryId=(int)id,
                Name = category.Name,
                Url = category.Url,
                Products = category.ProductCategories.Select(p=>p.Product).ToList()
            };
            return View(categoryModel);
        
        }
        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
          
                var entity = _categoryService.GetById(model.CategoryId);
                if(entity == null)
                {
                    return NotFound();
                } 
                entity.Name = model.Name;
                entity.Url = model.Url;

                _categoryService.Update(entity);
                
                //Message
                var msg =  new AlertMessage()
                {
                    AlertType = "warning",
                    Message =  $"ID:{model.CategoryId} nolu kategori güncellendi"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg);
                
                return RedirectToAction("CategoryList");
  
        }
            
        [HttpPost]
        public IActionResult DeleteFromCategory(int productId,int categoryId) 
        {
            _categoryService.DeleteFromCategory(productId,categoryId);
            return Redirect("/admin/categories/"+categoryId);
        }
        
        public IActionResult DeleteCategory(int categoryId)
        {
            
            var entity = _categoryService.GetById(categoryId);
            if(entity!=null){
                _categoryService.Delete(entity);
            }

            //Message
            var msg =  new AlertMessage()
            {
                AlertType = "danger",
                Message =  $"ID:{entity.CategoryId} nolu kategori silindi"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
            

            return RedirectToAction("CategoryList");
        }

        // OrderList
        public IActionResult OrderList()
        {
            var model = new OrderListModel(){
                Orders = _orderService.GetAll()
            };
            return View(model);
        }


        // private void CreateMessage(string message , string alerttype)
        // {
        //     var msg =  new AlertMessage()
        //     {
        //         AlertType = alerttype,
        //         Message =  message
        //     };
        //     TempData["message"] = JsonConvert.SerializeObject(msg);

        // }

    }


}