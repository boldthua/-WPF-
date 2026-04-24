using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using 理髮師問題WPF專案.Models;
using 理髮師問題WPF專案.Utilities;
using static 理髮師問題WPF專案.Contracts.MainContract;

namespace 理髮師問題WPF專案.Presenters
{
    internal class MainPresenter : IMainPresenter
    {
        IMainView view;
        public ConcurrentQueue<ModelCustomer> waitingList { get; set; } = new ConcurrentQueue<ModelCustomer>();
        private AutoResetEvent autoReset = new AutoResetEvent(false);
        int CustomerLeaved = 0;
        public SalonChair chair01 = new SalonChair();
        Object objCallBarber = new Object();
        bool isBarberSleeping = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPresenter(IMainView view)
        {
            this.view = view;
        }
        public async void StartHaircut()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (waitingList.Count == 0)
                    {

                        view.AddMessage("目前沒有顧客，理髮師坐在理髮椅上睡著了。");
                        isBarberSleeping = true;
                        view.ReflashSalonChair(Enums.BarberStatus.睡覺中);
                        autoReset.WaitOne();
                    }

                    if (isBarberSleeping == true)
                    {
                        view.AddMessage("顧客喚醒理髮師，理髮師準備中…");
                        view.ReflashSalonChair(Enums.BarberStatus.準備中);
                        isBarberSleeping = false;
                    }
                    else
                        view.AddMessage("理髮師清理座位，準備中…");

                    await Task.Delay(3000);
                    view.AddMessage("理髮師準備完成，開始叫號。");
                    // 開始理髮
                    waitingList.TryDequeue(out ModelCustomer customer);
                    view.ReflashWaitingList(waitingList);
                    view.ReflashSalonChair(customer);
                    view.AddMessage($"來賓號碼 {customer.ID} 號請入內理髮。");
                    view.AddMessage($"來賓 {customer.ID} 號理髮中…");
                    await chair01.DoHairCut(customer);
                    view.AddMessage($"來賓 {customer.ID} 號理髮完成！！");
                    view.ReflashSalonChair(Enums.BarberStatus.準備中);
                }
            });
        }

        public void AddCustomer()
        {
            lock (objCallBarber)
            {
                ModelCustomer customer = new ModelCustomer();
                if (waitingList.Count >= 6)
                {
                    CustomerLeaved++;
                    view.AddMessage($"沒有剩餘空位，進門顧客被氣走了！");
                    view.AddMessage($"累計氣走顧客：{CustomerLeaved} 位");
                    // 新增氣走顧客訊息或動畫
                    return;
                }
                waitingList.Enqueue(customer);
                customer.TakeTicket();
                view.AddMessage($"顧客編號 {customer.ID} 進門了。");
                view.ReflashWaitingList(waitingList);

                autoReset.Set(); // 喚醒理髮師

            }
        }

    }
}
