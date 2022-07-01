using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.webui.EmailServices;
using shopapp.webui.Extensions;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [AutoValidateAntiforgeryToken] // all controller actions validate against csrf attacks 
    public class AccountController:Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;

        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,IEmailSender emailSender ,ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cartService = cartService;
        } 

        // Login
        public IActionResult Login(string ReturnUrl=null)
        {

            return View(new LoginModel{
                ReturnUrl =ReturnUrl
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken] // to prevent csrf attacks 
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);    
            if(user==null)
            {
                ModelState.AddModelError("","Böyle Bir Kullanıcı Adı Yok");
                return View(model);
            }
            if(!await _userManager.IsEmailConfirmedAsync(user)){

                ModelState.AddModelError("","Account not confirmed , Please Confirm Your Account");
                return View(model);                 
            }
            var result = await _signInManager.PasswordSignInAsync(user,model.Password,false,false);
            if(result.Succeeded){
                return Redirect(model.ReturnUrl??"~/");
            }
            ModelState.AddModelError("","Parola Hatalı");
            return View(model);
        }

        //Register
        public IActionResult Register()
        {

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken] // to prevent csrf attacks
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                TempData.Put("message",new AlertMessage(){
                    Title = "Form Invalid ",
                    Message = "Form Invalid !",
                    AlertType = "warning"
                });
                // CreateMessage("Form Invalid !","warning");
                return View(model);
            }
            var user = new User(){
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user,model.Password);
            
            if(result.Succeeded)
            {
                //generate token 
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new {
                    userId = user.Id,
                    token = token
                });
                Console.WriteLine(url);
                //email confirm
                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Confirm your ShopApp account",
                    $"Please click the <a href ='https://localhost:5001{url}'>link</a> to confirm your account"
                );
                TempData.Put("message",new AlertMessage(){
                    Title = "Email send ",
                    Message = "Please check your mail box and confirm your account",
                    AlertType = "warning"
                });    
                // CreateMessage("Please check your mail box and confirm your account","warning");
                return RedirectToAction("Login","Account");
            }
            
            // CreateMessage(result.Errors.ToString(),"warning");
            
            return View(model);
        }
   


   
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token ==null) {
                //Message
                TempData.Put("message",new AlertMessage(){
                    Title = "Invalid token ",
                    Message = "Invalid token !",
                    AlertType = "warning"
                });
                // CreateMessage("Invalid token !","warning");
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null) {
                var result = await _userManager.ConfirmEmailAsync(user,token);
                if(result.Succeeded){
                    //Defining Cart Object for user

                    _cartService.InitializeCart(userId);

                    
                    TempData.Put("message",new AlertMessage(){
                        Title = "Success !  ",
                        Message = "Acount Confirmed !",
                        AlertType = "success"
                    });
                    // CreateMessage("Acount Confirmed !","success");
                    return View();
                }
            };
            TempData.Put("message",new AlertMessage(){
                Title = "Warning !  ",
                Message = "User not found !",
                AlertType = "warning"
            });            
            // CreateMessage("User not found !","warning");
            return View();
            
        }


        // Password Reset
        public IActionResult ForgotPassword(){
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(MailModel model){
            if(String.IsNullOrEmpty(model.Email))
            {
                TempData.Put("message",new AlertMessage(){
                    Title = "Warning !  ",
                    Message = "Please fill the form !",
                    AlertType = "danger"
                });     
                // CreateMessage("Please fill the form !","danger");
                return View();
            } 
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user==null)
            {
                TempData.Put("message",new AlertMessage(){
                    Title = "Warning !  ",
                    Message = "Email doesn't exists !",
                    AlertType = "danger"
                }); 
                // CreateMessage("Email doesn't exists !","danger");
                return View();
            }
            //generate token 
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword","Account",new {
                userId = user.Id,
                token = token
            });
            Console.WriteLine(token);
            //reset password
            await _emailSender.SendEmailAsync(
                model.Email,
                "Password Reset",
                $"Please click the <a href ='https://localhost:5001{url}'>link</a> to reset your password"
            );
            TempData.Put("message",new AlertMessage(){
                Title = "Warning !  ",
                Message = "Please check your mail box !",
                AlertType = "warning"
            }); 
            // CreateMessage("Please check your mail box !","warning");
            return View();
        }
        public IActionResult ResetPassword(string userId, string token)
        {
            if(userId == null || token ==null) {
                //Message
                TempData.Put("message",new AlertMessage(){
                    Title = "Danger !  ",
                    Message = "Invalid token !",
                    AlertType = "danger"
                }); 
                // CreateMessage("Invalid token !","warning");
                return View();
            }
            var model = new PasswordResetModel(){
                Token =token
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetModel model)
        {   
            if(!ModelState.IsValid) {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null) {
                TempData.Put("message",new AlertMessage(){
                    Title = "Danger !  ",
                    Message = "User not found !",
                    AlertType = "danger"
                });                 
                // CreateMessage("User not found !","warning");
                return View(model);
            };
            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if(result.Succeeded){
                TempData.Put("message",new AlertMessage(){
                    Title = "Success !  ",
                    Message = "Password Reset Successful !",
                    AlertType = "success"
                });                  
                // CreateMessage("Password Reset Successful !","success");
                return RedirectToAction("Login","Account");
            }
            foreach (var e in result.Errors)
            {
                Console.WriteLine(e.Description);
            }
            TempData.Put("message",new AlertMessage(){
                Title = "Danger !  ",
                Message = "Something gone wrong !",
                AlertType = "danger"
            });  
            // CreateMessage("Something gone wrong !","warning");
            return View(model);
            
        }




        // Logout 
        public async Task<IActionResult> Logout()
        {
            TempData.Put("message",new AlertMessage(){
                Title = "success !  ",
                Message = "Logout Success !",
                AlertType = "success"
            }); 
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }
        
        public IActionResult AccessDenied(){
            TempData.Put("message",new AlertMessage(){
                Title = "Access Denied",
                Message = "Authority limit exceeded",
                AlertType = "warning"
            });
            return RedirectToAction("Index","Home");
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