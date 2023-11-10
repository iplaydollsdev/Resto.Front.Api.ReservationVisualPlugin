using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Windows;

namespace Resto.Front.Api.ReservationVisualPlugin
{
    internal sealed class ReservesViewer : IDisposable
    {
        private readonly object syncObject = new object();
        private readonly CompositeDisposable resources = new CompositeDisposable();
        private bool disposed;

        public ReservesViewer()
        {
            // Добавляем кнопку в меню плагинов, привязывая действие старт
            PluginContext.Operations.AddButtonToPluginsMenu("ReservationVisualPlugin", _ => Start());
        }

        private void Start()
        {
            // Создаем отдельный потом для плагина
            var windowThread = new Thread(EntryPoint);
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();

            PluginContext.Log.Info("ReservesViewer started");
        }

        private void EntryPoint()
        {
            // Создаем WPF окно
            Window window;
            // Блокируем его от раннего удаления и одного потока
            lock (syncObject)
            {
                if (disposed)
                    return;

                // Создаем UserControl для отображения столов
                var reservesView = new ReservesView();

                // Задаем параметры окна, в том числе контент - ReservesView
                window = new Window
                             {
                                 SizeToContent = SizeToContent.WidthAndHeight,
                                 ResizeMode = ResizeMode.CanResize,
                                 Content = reservesView,
                                 Title = GetType().Name,
                                 Topmost = true
                             };

                // Задаем корректное завершение работы окна при dispose
                resources.Add(Disposable.Create(() =>
                {
                    window.Dispatcher.InvokeShutdown();
                    window.Dispatcher.Thread.Join();
                }));
                // Подписываемся на изменение броней в iiko
                resources.Add(PluginContext.Notifications.ReserveChanged
                    .Subscribe(_ => window.Dispatcher.BeginInvoke((Action)(reservesView.LoadReserves))));
                // Подписываемся на изменение столов в iiko
                resources.Add(PluginContext.Notifications.TableChanged
                    .Subscribe(_ => window.Dispatcher.BeginInvoke((Action)(reservesView.LoadTableElements))));
                // Подписываемся на изменение заказов в iiko
                resources.Add(PluginContext.Notifications.OrderChanged
                    .Subscribe(_ => window.Dispatcher.BeginInvoke((Action)(reservesView.LoadOrders))));
            }

            PluginContext.Log.Info("Show ReservesView dialog...");
            window.ShowDialog();
            PluginContext.Log.Info("Close ReservesView dialog...");
        }

        public void Dispose()
        {
            if (disposed)
                return;
            lock (syncObject)
            {
                resources.Dispose();
                PluginContext.Log.Info("ReservesViewer stopped");
                disposed = true;
            }
        }
    }
}
