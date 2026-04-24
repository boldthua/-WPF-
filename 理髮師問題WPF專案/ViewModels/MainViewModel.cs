using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using 理髮師問題WPF專案.Enums;
using 理髮師問題WPF專案.Models;
using 理髮師問題WPF專案.Presenters;
using 理髮師問題WPF專案.Utilities;
using 理髮師問題WPF專案.ViewModels;
using static 理髮師問題WPF專案.Contracts.MainContract;

namespace 理髮師問題WPF專案
{
    internal class MainViewModel : IMainView, INotifyPropertyChanged
    {
        IMainPresenter presenter { get; set; }
        private ObservableCollection<CustomerModel> _waitingList { get; set; }
        private ViewSalonChair _salonChair { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<string> _messages { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Message
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }
        public ViewSalonChair SalonChair
        {
            get => _salonChair;
            set
            {
                _salonChair = value;
                OnPropertyChanged();
            }
        }
        object obj = new object();
        public ObservableCollection<CustomerModel> WaitingList
        {
            get => _waitingList;
            set
            {
                _waitingList = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddCommand { get; set; }

        public MainViewModel()
        {
            presenter = new MainPresenter(this);
            WaitingList = new ObservableCollection<CustomerModel>()
            {
                new CustomerModel(),
                new CustomerModel(),
                new CustomerModel(),
                new CustomerModel(),
                new CustomerModel(),
                new CustomerModel(),
            };
            SalonChair = new ViewSalonChair();
            AddCommand = new RelayCommand(() =>
            {
                lock (obj) // 防止同時進門
                {
                    presenter.AddCustomer();

                    int countEmptySeat = 6 - WaitingList.Count;
                    for (int i = 0; i < countEmptySeat; i++)
                        WaitingList.Add(new CustomerModel());
                }
            });

            presenter.StartHaircut();//開始營業
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Message.Add(message);
            });
        }


        public void ReflashWaitingList(ConcurrentQueue<ModelCustomer> waitingList)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WaitingList.Clear();
                foreach (var customer in waitingList)
                {
                    CustomerModel model = new CustomerModel(customer);
                    WaitingList.Add(model);
                }
                int countEmptySeat = 6 - WaitingList.Count;
                for (int i = 0; i < countEmptySeat; i++)
                    WaitingList.Add(new CustomerModel());
            });

        }

        public void ReflashSalonChair(BarberStatus barber) //客人離座
        {
            SalonChair.Barber = barber;
            SalonChair.Chair = (ChairStatus)barber;
            SalonChair.IsSeated = false;
            SalonChair.servedCust = null;
        }



        public void ReflashSalonChair(ModelCustomer customer)
        {
            SalonChair.Barber = BarberStatus.理髮中;
            SalonChair.Chair = (ChairStatus)BarberStatus.理髮中;
            CustomerModel model = new CustomerModel(customer);
            SalonChair.servedCust = model;
            SalonChair.IsSeated = true;
        }
    }
}
