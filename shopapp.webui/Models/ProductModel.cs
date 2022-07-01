using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        // [Required(ErrorMessage ="Name Is Required")]
        // [StringLength(50,MinimumLength =5,ErrorMessage ="Must be in range 5-50")]
        public string Name  { get; set; }

        // [Required(ErrorMessage ="Url Is Required")]
        public string Url  { get; set; }

        // [Required(ErrorMessage ="Price Is Required")]
        // [Range(100,100000,ErrorMessage ="Price must be in the range 100-100000")]
        public double? Price { get; set; }
        
        public string ImageUrl { get; set; }
       [Display(Name = "Description")]
        public string Desc { get; set; }
        public bool IsApproved { get; set; }
        public bool IsHomePage { get; set; }
        public List<Category> SelectedCategories { get; set; }
    }
}