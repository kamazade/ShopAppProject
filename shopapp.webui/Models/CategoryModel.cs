using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Name Is Required")]
        [StringLength(20,MinimumLength =6,ErrorMessage ="Name must be in range 6-20")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Url Is Required")]
        [StringLength(20,MinimumLength =6,ErrorMessage ="Url must be in range 6-20")]

        public string Url { get; set; }
        public List<Product> Products { get; set; }
        
    }
}