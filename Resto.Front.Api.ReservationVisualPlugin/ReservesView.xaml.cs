using Resto.Front.Api.Data.Brd;
using Resto.Front.Api.Data.Orders;
using Resto.Front.Api.Data.Organization.Sections;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Resto.Front.Api.ReservationVisualPlugin
{
    /// <summary>
    /// Обображение столов и изменения их статусов в окне
    /// </summary>
    public partial class ReservesView : UserControl
    {
        public ObservableCollection<IReserve> Reserves { get; set; } = new ObservableCollection<IReserve>();
        public ObservableCollection<IOrder> Orders { get; set; } = new ObservableCollection<IOrder>();
        public ObservableCollection<TableElementViewModel> TableElements { get; set; } = new ObservableCollection<TableElementViewModel>();

        public ReservesView()
        {
            // Инициализируем UserControl
            InitializeComponent();
            // Загружаем схему столов
            LoadTableElements();
            // Загружаем брони
            LoadReserves();
            // Загружаем заказы
            LoadOrders();
            // Присваиваем контекс к этому классу
            DataContext = this;
        }

        /// <summary>
        /// Выгружает список броней
        /// </summary>
        public void LoadReserves()
        {
            // Очищаем список броней
            Reserves.Clear();
            // Получаем брони без статуса Closed из PluginContext и добавляем их в коллекцию
            PluginContext.Operations.GetReserves().Where(r => r.Status != ReserveStatus.Closed).ForEach(Reserves.Add);

            // Обновляем статусы столов
            ReloadTableStatuses();
        }
        /// <summary>
        /// Выгружает список заказов
        /// </summary>
        public void LoadOrders()
        {
            // Очищаем список заказов
            Orders.Clear();

            // Получаем заказов без статуса Closed и Deleted из PluginContext и добавляем их в коллекцию
            PluginContext.Operations.GetOrders().Where(o => o.Status != OrderStatus.Closed && o.Status != OrderStatus.Deleted).ForEach(Orders.Add);

            // Обновляем статусы столов
            ReloadTableStatuses();
        }

        /// <summary>
        /// Выгружает схемы заведения, получает столы и распределяет статусы столов
        /// </summary>
        public void LoadTableElements()
        {
            // Получаем схему заведения д
            var sectionSchemas = PluginContext.Operations.GetTerminalsGroupRestaurantSections(PluginContext.Operations.GetHostTerminalsGroup())
                .Select(section => section.TryGetSectionSchema())
                .Where(schema => schema != null)
                .ToList();

            // Очищаем список столов
            TableElements.Clear();

            // Задаем отступ для разных зон (первая 0)
            double yOffset = 0;

            foreach (var schema in sectionSchemas)
            {
                foreach (var table in schema.TableElements)
                {
                    // Создаем элемент для каждого стола в зоне
                    TableElements.Add(new TableElementViewModel
                    {
                        Number = table.Table.Number,
                        Name = table.Table.Name,
                        IsActive = table.Table.IsActive,
                        X = table.X,
                        Y = table.Y + yOffset
                    });
                }

                // Увеличиваем отступ перед переходом на следующую зону
                yOffset += 100;
            }
            // Обновляем статусы
            ReloadTableStatuses();
        }

        /// <summary>
        /// Обновляет статусы столов в зависимости от того есть на него бронь или активный заказ
        /// </summary>
        public void ReloadTableStatuses()
        {
            // Проходим по каждому столу в получившийся коллекции
            foreach (var tableElement in TableElements)
            {
                // Получаем бронь для текущего стола (если она есть)
                var reserveForTable = Reserves.FirstOrDefault(r => r.Tables.Any(t => t.Number == tableElement.Number));
                // Получаем заказы для текущего стола (если они есть)
                var ordersForTable = Orders.FirstOrDefault(r => r.Tables.Any(t => t.Number == tableElement.Number));

                // Если бронь найдена, то устанавливаем статус стола в статус брони
                if (reserveForTable != null)
                {
                    tableElement.Status = TableStatus.Reserved;
                }
                // Если заказы найдены,то устанавливаем статус стола в Started
                else if (ordersForTable != null)
                {
                    tableElement.Status = TableStatus.Started;
                }
                // Если не нашлось ни брони, ни заказов значит наш стол свободен, присваем Free
                else
                {
                    tableElement.Status = TableStatus.Free;
                }
            }
        }
    }
}
