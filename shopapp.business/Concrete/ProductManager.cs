using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    // Burada iş kurallarını uygulayıp kontrol sağlayacağız
    public class ProductManager : IProductService
    {   
        // böyle yaparsak ileride farklı bir data access teknolojisi kullandığımızda buraya gelip hepsini düzeltmemiz gerekir . bunun yerine Efprd.repos. nin implemente edildiği Iproductrepo. kullanalım.
        //EfCoreProductRepository productRepository = new EfCoreProductRepository();
        private IProductRepository _productRepository ;
        public ProductManager(IProductRepository productRepository)
        {
            _productRepository=productRepository;
        }




        public bool Create(Product entity)
        {
            if(Validation(entity)) 
            {
                _productRepository.Create(entity);
                return true;
            }
            return false;
        }

        public bool Create(Product entity, int[] categoryIds)
        {
            if(Validation(entity)) 
            {
                _productRepository.Create(entity,categoryIds);
                return true;
            }
            return false;

        }

  
        public void Delete(Product entity)
        {
            // iş kurallarını uygula 
            _productRepository.Delete(entity); 
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategory(int id)
        {
            return _productRepository.GetByIdWithCategory(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return _productRepository.GetProductDetails(url);
        }

        public List<Product> GetProductsByCategory(string name,int page, int pageSize)
        {
            return _productRepository.GetProductsByCategory(name,page,pageSize);
        }

        public List<Product> GetSearchResult(string searchstring)
        {
            return _productRepository.GetSearchResult(searchstring);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public void Update(Product entity, int[] categoryIds)
        {
            _productRepository.Update(entity,categoryIds);
        }

        public string ErrorMessage { get; set; }
        public bool Validation(Product entity)
        {
            var isValid =true;
            if(String.IsNullOrEmpty(entity.Name)){

                ErrorMessage +="Product name is required.\n";
                isValid =false;
            }
            if(entity.Price<0){

                ErrorMessage +="Price can not be a negative number.\n";
                isValid =false;
            }
            return isValid;
        }
    }
}