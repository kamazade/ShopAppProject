using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class AdressManager : IAdressService
    {
        private IAdressRepository _adressrepository;
        public AdressManager(IAdressRepository adressrepository)
        {
            _adressrepository = adressrepository;
        }

        public void AddAddress(Adress adress)
        {
            _adressrepository.AddAddress(adress);

        }

        public List<Adress> GetAdressesByUserId(string userId)
        {
           return _adressrepository.GetAdressesByUserId(userId);
        }
    }
}