using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Order
    {
        public int OrderId { get; set; }

        public User User { get; set; }

        public Movie Movie { get; set; }    

        public DateTime OrderTime { get; set; }

        public int Price { get; set; }

        public int Payment { get; set; }
    }
}
