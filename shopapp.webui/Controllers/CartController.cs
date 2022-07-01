using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Extensions;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [Authorize]
    public class CartController:Controller
    {
        private IOrderService _orderService;
        private ICartService _cartService;
        private  IAdressService _adressService;
        private UserManager<User> _userManager;
        public CartController(ICartService cartService,UserManager<User> userManager,IAdressService adressService,IOrderService orderService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _adressService = adressService;
            _orderService = orderService;
        }
    
        public IActionResult Index ()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            var model = new CartModel(){
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i=> new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (double)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity

                    
                }).ToList()
            };



            return View(model);
        }

        [HttpPost]
        public IActionResult AddToCart (int productId , int quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.AddToCart(userId,productId,quantity);
            
            return RedirectToAction("Index");
        }
    
        [HttpPost]
        public IActionResult RemoveFormCart(int productId){
            var userId = _userManager.GetUserId(User);
            // Console.WriteLine(productId);
            _cartService.RemoveFormCart(productId,userId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout(){
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            var model = new CartModel(){
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i=> new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (double)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity

                    
                }).ToList()
            };

            var orderModel = new OrderModel(){
                CartModel = model
            };
            // ViewBag.Adresses =_adressService.GetAdressesByUserId(_userManager.GetUserId(User));
            return View(orderModel);
        }
        [HttpPost]
        public IActionResult Checkout(OrderModel model){
             
            // if(model.IsAdressSave){
            //     var userId = _userManager.GetUserId(User);
            //     var adress = new Adress(){
            //         UserId =userId,
            //         City = model.City,
            //         AdressDetail = model.Adress
            //     };
            //     _adressService.AddAddress(adress);
            // }

            if(ModelState.IsValid){
                var userId = _userManager.GetUserId(User);
                var cart = _cartService.GetCartByUserId(userId);
                model.CartModel = new CartModel(){
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i=> new CartItemModel()
                    {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity
                    }).ToList()
                };
                
                
                var payment = PaymentProcess(model);
                if(payment.Status == "success"){
                    SaveOrder(model,payment,userId);
                    ClearCart(model.CartModel.CartId);
                    TempData.Put("message",new AlertMessage(){
                        Title = "",
                        Message = "Order Recieved Successfuly !",
                        AlertType = "success"
                    });
                    return RedirectToAction("Index");
                }
                TempData.Put("message",new AlertMessage(){
                    Title = "",
                    Message = payment.ErrorMessage,
                    AlertType = "warning"
                });
                return RedirectToAction("Index");

            }
            return View();

        }

        private void ClearCart(int cartId)
        {
            _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            var order = new Order();
            order.OrderNumber = new Random().Next(111111,999999).ToString();
            order.OrderState = EnumOrderState.waiting;
            order.PaymentType=EnumPaymentType.CreditCart;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
            order.OrderDate = DateTime.Now;
            order.FirstName = model.FirstName;
            order.LastName = model.LastName;
            order.UserId = userId;
            order.Adress = model.Adress;
            order.City = model.City;
            order.Phone = model.Phone;
            order.Email = model.Email;
            order.Note = model.Note;
            order.OrderItems = new List<entity.OrderItem>();
            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new shopapp.entity.OrderItem(){
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);




        }
        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-RvKDZCXuF6q4khIMW0o0FoZJNXUGIWte";
            options.SecretKey = "sandbox-wQC7EmkHXWeUA66xNnhX6uX6VRznA3xq";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";
                    
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(111111111,999999999).ToString();
            request.Price = model.CartModel.TotalPrice().ToString();
            request.PaidPrice = model.CartModel.TotalPrice().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            // paymentCard.CardNumber = "5528790000000008";
            // paymentCard.ExpireMonth = "12";
            // paymentCard.ExpireYear = "2030";
            // paymentCard.Cvc = "123";

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.Adress;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;
            foreach (var item in model.CartModel.CartItems)
            {
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Telefon";
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItem.Category2 = "Elektronik";
                basketItem.Price = item.Price.ToString();
                basketItems.Add(basketItem);

            }

            request.BasketItems = basketItems;

           return Payment.Create(request, options);
        }
    
        public IActionResult OrderList()
        {
            var userId = _userManager.GetUserId(User);
            var model = new OrderListModel(){
                Orders = _orderService.GetbyUserId(userId)
            };
            return View(model);
        }
    
    
    }
}