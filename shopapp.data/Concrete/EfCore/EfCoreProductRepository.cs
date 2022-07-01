using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, ShopContext>, IProductRepository
    {
        
        public void Update(Product entity, int[] categoryIds)
        {
            using(var contex = new ShopContext())
            {
                var product = contex.Products
                    .Include(i=> i.ProductCategories)
                    .FirstOrDefault(i=> i.ProductId == entity.ProductId);
                if( product != null)
                {
                    product.Name = entity.Name;
                    product.Price = entity.Price;
                    product.Desc = entity.Desc;
                    product.Url = entity.Url;
                    product.ImageUrl = entity.ImageUrl;
                    product.IsApproved = entity.IsApproved;
                    product.IsHomePage= entity.IsHomePage;

                    product.ProductCategories = categoryIds
                            .Select(
                                catid=> new ProductCategory(){
                                    CategoryId = catid,
                                    ProductId = entity.ProductId
                                }
                            ).ToList();
                    contex.SaveChanges();
                }                   
            }    
        }
        
        public Product GetByIdWithCategory(int id)
        {
            using(var contex = new ShopContext())
            {
                var product  = contex.Products
                    .Where(p=> p.ProductId == id)
                    .Include(p=>p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                    .FirstOrDefault();

                return product;    

            }

        }


        public int GetCountByCategory(string category)
        {
            using(var contex = new ShopContext())
            {
                var products = contex.Products
                        .Where(i=> i.IsApproved)
                        .AsQueryable();
                if(!string.IsNullOrEmpty(category))
                {
                    products = products 
                        .Include(i=> i.ProductCategories)
                        .ThenInclude(i=>i.Category)
                        .Where(i=>i.ProductCategories.Any(a=>a.Category.Url == category));
                }
                return products.Count();
            }
            
        }

        public List<Product> GetHomePageProducts()
        {
            using(var contex = new ShopContext())
            {
                var products = contex.Products
                    .Where(p=>p.IsHomePage && p.IsApproved).ToList();
                
                return products;
            }
            
        }

        public List<Product> GetPopularProducts()
        {
            throw new NotImplementedException();
        }

        public Product GetProductDetails(string url)
        {
            using(var contex = new ShopContext())
            {
                var product = contex.Products
                        .Where(p=> p.Url == url)
                        .Include(i=> i.ProductCategories)
                        .ThenInclude(i=> i.Category)
                        .FirstOrDefault(); 
                return product;
            }           
        }

        public List<Product> GetProductsByCategory(string name,int page,int pageSize)
        {
            using(var contex = new ShopContext())
            {
                var products = contex.Products
                        .AsQueryable();
                if(!string.IsNullOrEmpty(name))
                {
                    products = products 
                        .Include(i=> i.ProductCategories)
                        .ThenInclude(i=>i.Category)
                        .Where(i=>i.ProductCategories.Any(a=>a.Category.Url == name));
                }
                return products.Skip((page-1)*pageSize).Take(pageSize).ToList();
            }
        }

        public List<Product> GetSearchResult(string searchstring)
        {
            using(var contex = new ShopContext())
            {
                var result  = contex.Products
                                .Where(p=> p.Name.ToLower().Contains(searchstring.ToLower())|| p.Desc.ToLower().Contains(searchstring.ToLower()))
                                .ToList();
                return result;
            }
        }

        public void Create(Product entity, int[] categoryIds)
        {
            using(var contex = new ShopContext())
            {
                Product product = new Product()
                {
                    Name = entity.Name,
                    Url = entity.Url,
                    Desc = entity.Desc,
                    ImageUrl = entity.ImageUrl,
                    Price = entity.Price,
                    ProductCategories = categoryIds.Select(catid=> new ProductCategory(){
                        CategoryId = catid,
                        ProductId = contex.Products.Max(p=> p.ProductId)+1
                    }).ToList()
                };
                contex.Products.Add(product);
                contex.SaveChanges();
            }
        }
    }
}