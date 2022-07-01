using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface IProductService:IValidator<Product>
    {
        Product GetById(int id);
        Product GetByIdWithCategory(int id);
        Product GetProductDetails(string url);        
        List<Product> GetAll();
        List<Product> GetProductsByCategory(string name,int page, int pageSize);
        List<Product> GetHomePageProducts();
        List<Product> GetSearchResult(string searchstring);
        bool Create(Product entity);
        void Update(Product entity);
        void Update(Product entity, int[] categoryIds);
        void Delete(Product entity);
        int GetCountByCategory(string category);
        
        bool Create(Product entity, int[] categoryIds);
    }
}