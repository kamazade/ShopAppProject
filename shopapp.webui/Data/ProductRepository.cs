using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.webui.Data
{
    public static class ProductRepository
    {
        private static List<Product> _products = null;
        static ProductRepository()
        {
            _products = new List<Product> {
                new Product(){ProductId=1,Name="Samsung S8", Price =6000,Desc="good Phone",ImageUrl="1.jpg"},
                new Product(){ProductId=2,Name="Samsung S9", Price =7000,Desc="good Phone",ImageUrl="2.jpg"},
                new Product(){ProductId=3,Name="Samsung S7", Price =5000,Desc="good Phone",ImageUrl="3.jpg"},
                new Product(){ProductId=4,Name="Samsung S9", Price =7000,Desc="good Phone",ImageUrl="4.jpg"},
                new Product(){ProductId=5,Name="Samsung S10", Price =8000,Desc="good Phone",ImageUrl="5.jpg"},
                new Product(){ProductId=6,Name="Lenova ", Price =9000,Desc="good ",ImageUrl="6.jpg"},
                new Product(){ProductId=7,Name="Casper Nirvana", Price =10000,Desc="good",ImageUrl="7.jpg"},
                new Product(){ProductId=8,Name="Arçelik", Price =11000,Desc="good",ImageUrl="2.jpg"},
            };
            
        }

        public static List<Product> Products {
            get {
                return _products;
            }
        }

        public static void AddProduct(Product product) {
            _products.Add(product);
        }

        public static Product GetProductById (int id) {
            return _products.FirstOrDefault(p=> p.ProductId == id);
             
        }
        public static void EditProduct(Product product) 
        {
            foreach (var p in _products)
            {
                if(p.ProductId == product.ProductId)
                {
                    p.Name = product.Name;
                    p.Price = product.Price;
                    p.Desc = product.Desc;
                    p.ImageUrl = product.ImageUrl;
                    
                }
            }
        }

        
        public static void DeleteProduct(int id)
        {
            var product = GetProductById(id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }

    }
}