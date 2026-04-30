using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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

        private bool _isAutoAdding = false;
        public string TextAutoAdding => _isAutoAdding ? "取消自動增加" : "自動增加客人";

        public System.Threading.Timer timer = null;
        public CancellationTokenSource tokenSource = new CancellationTokenSource();

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
        public ICommand Add15Command { get; set; }
        public ICommand AddPer500msCommand { get; set; }

        public ICommand ProgramReset { get; set; }

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
            AddCommand = new RelayCommand(ExecuteAddCustomer);

            Add15Command = new RelayCommand(() => ExecuteAdd15Customer(tokenSource.Token));

            AddPer500msCommand = new RelayCommand(ExecuteAutoAddCommand);

            ProgramReset = new RelayCommand(ExecuteReset);

            presenter.StartHaircut(tokenSource.Token);//開始營業
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

        public bool IsAutoAdding
        {
            get { return _isAutoAdding; }
            set
            {
                _isAutoAdding = value;
                OnPropertyChanged(nameof(TextAutoAdding));
            }
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

        public void ReflashWaitingList()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WaitingList.Clear();
                int countEmptySeat = 6;
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

        // 按鈕們

        private void ExecuteAddCustomer()
        {
            lock (obj) // 防止同時進門
            {
                presenter.AddCustomer();

                int countEmptySeat = 6 - WaitingList.Count;
                for (int i = 0; i < countEmptySeat; i++)
                {
                    WaitingList.Add(new CustomerModel());
                }
            }
        }

        //private async void ExecuteAdd15Customer()
        //{
        //    var tasks = new List<Task>();
        //    for (int i = 0; i < 15; i++)
        //    {
        //        tasks.Add(Task.Run(() => ExecuteAddCustomer()));
        //    }

        //    await Task.WhenAll(tasks);
        //}

        private Task ExecuteAdd15Customer(CancellationToken token)
        {
            return Task.Run(() =>
            Parallel.For(0, 15, new ParallelOptions() { MaxDegreeOfParallelism = 5, CancellationToken = token }, (i) =>
            {

                ExecuteAddCustomer();
            })
            );
        }

        private void ExecuteAutoAddCommand()
        {
            if (IsAutoAdding == false)
            {
                IsAutoAdding = true;
                timer = new System.Threading.Timer((obj) =>
                {
                    ExecuteAddCustomer();
                }, null, 0, 500);
                return;
            }
            IsAutoAdding = false;
            timer.Dispose();
            timer = null;
        }

        private void ExecuteReset()
        {
            tokenSource.Cancel();

            ReflashWaitingList();
            Message.Clear();
            if (IsAutoAdding == true)
            {
                IsAutoAdding = false;
                timer.Dispose();
                timer = null;
            }
            presenter.Dispose();
            presenter = new MainPresenter(this);
            SalonChair = new ViewSalonChair();
            ModelCustomer.Reset();
            tokenSource = new CancellationTokenSource();
            presenter.StartHaircut(tokenSource.Token);
        }
    }
}
