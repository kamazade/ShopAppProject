using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface IAdressService
    {
        void AddAddress(Adress adress);
        List<Adress> GetAdressesByUserId(string userId);
    }
}