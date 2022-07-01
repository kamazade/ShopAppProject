using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public class EfCoreOrderRepository : EfCoreGenericRepository<Order, ShopContext>, IOrderRepository
    {
        public List<Order> GetByUserId(string userId)
        {
            using (var contex = new ShopContext())
            {
                return contex.Orders
                    .Include(i=> i.OrderItems)
                    .ThenInclude(i=>i.Product)
                    .Where(i=> i.UserId == userId).ToList();                    
            }
        }
    }
}