using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using 理髮師問題WPF專案.Enums;
using 理髮師問題WPF專案.Models;
using 理髮師問題WPF專案.Utilities;

namespace 理髮師問題WPF專案.Contracts
{
    internal class MainContract
    {
        public interface IMainView
        {
            void AddMessage(string message);
            void ReflashWaitingList(ConcurrentQueue<ModelCustomer> waitingList);

            void ReflashWaitingList();
            void ReflashSalonChair(ModelCustomer customer);
            void ReflashSalonChair(BarberStatus barber);
        }

        public interface IMainPresenter
        {
            void StartHaircut(CancellationToken token);

            void AddCustomer();

            void Reset();

            void Dispose();
        }
    }
}
