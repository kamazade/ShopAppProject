using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface ICategoryService:IValidator<Category>
    {   
        void DeleteFromCategory(int productId,int categoryId);
        Category GetByIdWithProducts(int id);
        Category GetById(int id);        
        List<Category> GetAll();

        void Create(Category entity);
        void Update(Category entity);
        void Delete(Category entity);
    }
}