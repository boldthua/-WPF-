using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using 理髮師問題WPF專案.Models;

namespace 理髮師問題WPF專案.ViewModels
{
    internal class CustomerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ID { get; set; }
        private bool isEmpty = true;
        public string Seated => isEmpty ? "空" : "客";
        public string ShowingID => isEmpty ? "0000" : ID;

        public CustomerModel(ModelCustomer customer)
        {
            ID = customer.ID;
            IsEmpty = false;
        }
        public CustomerModel()
        {
            IsEmpty = true;
        }

        public bool IsEmpty
        {
            get { return isEmpty; }
            set
            {
                isEmpty = value;
                OnPropertyChanged(Seated);
                OnPropertyChanged(ShowingID);
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
