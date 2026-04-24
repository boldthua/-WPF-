using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using 理髮師問題WPF專案.ViewModels;

namespace 理髮師問題WPF專案.Models
{
    internal class ModelCustomer
    {
        public string ID { get; set; }
        public static int counter = 1;
        Random random = new Random(Guid.NewGuid().GetHashCode());
        public int time = 0;

        public ModelCustomer()
        {

        }
        public void TakeTicket()
        {
            ID = counter.ToString("D4");
            counter++;

            time = random.Next(3, 6) * 1000;
        }
    }
}
