using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using 理髮師問題WPF專案.Models;

namespace 理髮師問題WPF專案.Utilities
{
    internal class SalonChair
    {
        public int chairNumber = 0;
        public ModelCustomer customer { get; set; }

        public SalonChair()
        {

        }

        public async Task DoHairCut(ModelCustomer customer)
        {
            chairNumber++;
            this.customer = customer;
            Thread.Sleep(customer.time);
        }

    }
}
