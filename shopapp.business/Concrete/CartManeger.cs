using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class CartManeger : ICartService
    {
        private ICartRepository _cartRepository;
        public CartManeger(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public void AddToCart(string userId,int productId,int quantity)
        {
            var cart = GetCartByUserId(userId);
            if(cart!= null){
                var index = cart.CartItems.FindIndex(i=> i.ProductId==productId);
                // product exists in cart(update)
                if(index>=0){
                    cart.CartItems[index].Quantity+=quantity;
                    
                }
                // product does not exists in cart(new registery)
                else{
                    cart.CartItems.Add(new CartItem(){
                        ProductId = productId,
                        CartId = cart.Id,
                        Quantity = quantity
                        
                    });
                }     
                _cartRepository.Update(cart);
            }


        }

        public void ClearCart(int cartId)
        {
         _cartRepository.ClearCart(cartId);   
        }

        public Cart GetCartByUserId(string userId)
        {
            return _cartRepository.GetByUserId(userId);
        }

        public void InitializeCart(string userId)
        {
            _cartRepository.Create(new Cart(){
                UserId = userId
            });
        }
    
        public void RemoveFormCart(int productId,string userId)
        {
            var cart = GetCartByUserId(userId);
            if(cart!= null){
                _cartRepository.RemoveFromCart(cart.Id,productId);
            }
        }
    
    }
}