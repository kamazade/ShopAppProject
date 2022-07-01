using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface ICartService
    {
        void InitializeCart(string userId);

        Cart GetCartByUserId(string userId);
        void AddToCart(string userId,int productId,int quantity);    
        void RemoveFormCart(int productId,string userId);
        void ClearCart(int cartId);
    }
}