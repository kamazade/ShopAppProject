using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class OrderListModel
    {
        public List<Order> Orders { get; set; }
    }
}