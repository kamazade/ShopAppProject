using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public class EfCoreAdressRepository : IAdressRepository
    {
        public void AddAddress(Adress adress)
        {
            using(var context = new ShopContext())
            {
                context.Adresses.Add(adress);
                context.SaveChanges();
            }
        }

        public List<Adress> GetAdressesByUserId(string userId)
        {
            using(var context = new ShopContext() ){
                var adresses =  context.Adresses.Where(i => i.UserId == userId).ToList();
                return adresses;
            }
            
        }

 
    }
}