using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using 理髮師問題WPF專案.Enums;

namespace 理髮師問題WPF專案.ViewModels
{
    internal class ViewSalonChair : INotifyPropertyChanged
    {
        public CustomerModel servedCust { get; set; }
        private bool _isSeated = false;
        private BarberStatus _barber = BarberStatus.睡覺中;
        private ChairStatus _chair = ChairStatus.理髮師;
        public event PropertyChangedEventHandler PropertyChanged;
        public string CustID { get; set; } = "0000";
        public string CallingID { get; set; } = "0000";

        public BarberStatus Barber
        {
            get { return _barber; }
            set
            {
                _barber = value;
                OnPropertyChanged();
            }
        }
        public ChairStatus Chair
        {
            get { return _chair; }
            set
            {
                _chair = value;
                OnPropertyChanged();
            }
        }
        // status 和 whoOnChair的顯示會同步
        // 理髮中:顧客ID ， 準備中:空 ， ZzzZz:理髮師

        public ViewSalonChair() // 本身就是理髮師 
        {

        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsSeated
        {
            get { return _isSeated; }
            set
            {
                _isSeated = value;
                if (_isSeated == true)
                {
                    CustID = servedCust.ID;
                    CallingID = CustID;
                }
                else if (Barber == BarberStatus.睡覺中)
                {
                    CustID = "ZzzZz";
                }
                else
                {
                    CustID = "0000";
                }
                OnPropertyChanged(nameof(CustID));
                OnPropertyChanged(nameof(servedCust));
                OnPropertyChanged(nameof(CallingID));
            }
        }


    }
}
