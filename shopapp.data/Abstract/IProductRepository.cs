using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;


namespace shopapp.data.Abstract
{
    public interface IProductRepository :IRepository<Product>
    {   
        Product GetByIdWithCategory(int id);
        Product GetProductDetails(string url);
        List<Product> GetProductsByCategory(string name,int page,int pageSize);
        List<Product> GetPopularProducts();
        List<Product> GetSearchResult(string searchstring);
        List<Product> GetHomePageProducts();
        int GetCountByCategory(string category);
        void Update(Product entity, int[] categoryIds);
        void Create(Product entity, int[] categoryIds);
    }
}