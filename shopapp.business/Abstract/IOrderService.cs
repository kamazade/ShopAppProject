using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface IOrderService
    {
        void Create(Order entity);
        List<Order> GetAll();
        List<Order> GetbyUserId(string userId);
    }
}