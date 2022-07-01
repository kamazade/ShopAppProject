using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, ShopContext>, ICategoryRepository
    {
        public void DeleteFromCategory(int productId, int categoryId)
        {
            using(var context = new ShopContext())
            {
                var cmd = "delete from ProductCategory where ProductId = @p0 and CategoryId =@p1";
                context.Database.ExecuteSqlRaw(cmd,productId,categoryId);
            }
        }

        public Category GetByIdWithProducts(int id)
        {
            using(var context = new ShopContext())
            {
                var category = context.Categories
                        .Where(i=> i.CategoryId == id)
                        .Include(i=> i.ProductCategories)
                        .ThenInclude(i=> i.Product)
                        .FirstOrDefault();
                return category;
            }
        }

    
        public List<Category> GetPopularCategories()
        {
            throw new NotImplementedException();
        }
    }
}