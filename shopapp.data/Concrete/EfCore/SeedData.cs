using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public static class SeedData
    {
        public static void Seed(){
            var context = new ShopContext();
            if(context.Database.GetPendingMigrations().Count()==0){
                if(context.Categories.Count()==0){
                    context.Categories.AddRange(Categories);
                }
                if(context.Products.Count()==0){
                    context.Products.AddRange(Products);
                    context.AddRange(ProductCategories);
                }
            }
            context.SaveChanges();
        }

        private static List<Category> Categories = new List<Category>(){
            new Category(){Name="Telefon",Url="telefon"},    
            new Category(){Name="Bilgisayar",Url="bilgisayar"},    
            new Category(){Name="Elektronik",Url="elektronik"},    
            new Category(){Name="Beyaz Eşya",Url="beyaz-esya"},    
        };
        private static List<Product> Products = new List<Product>(){
            new Product(){Name="Samsung S7",Url="samsung-s7" ,Price=5000,Desc="iyi telefon",ImageUrl="2.jpg",IsApproved=true,IsHomePage=false},
            new Product(){Name="Samsung S6",Url="samsung-s6" ,Price=4000,Desc="iyi telefon",ImageUrl="1.jpg",IsApproved=true,IsHomePage=false},
            new Product(){Name="Samsung S8",Url="samsung-s8" ,Price=6000,Desc="iyi telefon",ImageUrl="3.jpg",IsApproved=true,IsHomePage=false},
            new Product(){Name="Samsung S9",Url="samsung-s9" ,Price=7000,Desc="iyi telefon",ImageUrl="4.jpg",IsApproved=true,IsHomePage=false},
            new Product(){Name="Samsung S10",Url="samsung-s10" ,Price=8000,Desc="iyi telefon",ImageUrl="5.jpg",IsApproved=true,IsHomePage=false},
            new Product(){Name="Samsung S11",Url="samsung-s11" ,Price=9,Desc="iyi telefon",ImageUrl="1.jpg",IsApproved=true,IsHomePage=true},
            new Product(){Name="Samsung S12",Url="samsung-s12" ,Price=10000,Desc="iyi telefon",ImageUrl="3.jpg",IsApproved=true,IsHomePage=true},
            new Product(){Name="IPhone X",Url="ıphone-x",Price=13000,Desc="iyi telefon",ImageUrl="4.jpg",IsApproved=true,IsHomePage=true},
            new Product(){Name="IPhone 11",Url="ıphone-11",Price=15000,Desc="iyi telefon",ImageUrl="5.jpg",IsApproved=true,IsHomePage=true},
            new Product(){Name="Casper Nirvana",Url="casper-nirvana",Price=15000,Desc="iyi telefon",ImageUrl="5.jpg",IsApproved=true,IsHomePage=false},
            new Product(){Name="Moster Abra",Url="monster-abra",Price=15000,Desc="iyi telefon",ImageUrl="5.jpg",IsApproved=true,IsHomePage=true},

        };
    
        private static List<ProductCategory> ProductCategories = new List<ProductCategory>(){
            new ProductCategory(){Product = Products[0] , Category = Categories[0]},
            new ProductCategory(){Product = Products[0] , Category = Categories[2]},
            new ProductCategory(){Product = Products[1] , Category = Categories[0]},
            new ProductCategory(){Product = Products[1] , Category = Categories[2]},
            new ProductCategory(){Product = Products[2] , Category = Categories[0]},
            new ProductCategory(){Product = Products[2] , Category = Categories[2]},
            new ProductCategory(){Product = Products[3] , Category = Categories[0]},
            new ProductCategory(){Product = Products[3] , Category = Categories[2]},
            new ProductCategory(){Product = Products[4] , Category = Categories[0]},
            new ProductCategory(){Product = Products[4] , Category = Categories[2]},
            new ProductCategory(){Product = Products[5] , Category = Categories[0]},
            new ProductCategory(){Product = Products[5] , Category = Categories[2]},
            new ProductCategory(){Product = Products[6] , Category = Categories[0]},
            new ProductCategory(){Product = Products[6] , Category = Categories[2]},
            new ProductCategory(){Product = Products[7] , Category = Categories[0]},
            new ProductCategory(){Product = Products[7] , Category = Categories[2]},
            new ProductCategory(){Product = Products[8] , Category = Categories[0]},
            new ProductCategory(){Product = Products[8] , Category = Categories[2]},
            new ProductCategory(){Product = Products[9] , Category = Categories[1]},
            new ProductCategory(){Product = Products[9] , Category = Categories[2]},
            new ProductCategory(){Product = Products[10] , Category = Categories[1]},
            new ProductCategory(){Product = Products[10] , Category = Categories[2]},

        };
    
    
    
    
    
    
    
    
    }

    
}