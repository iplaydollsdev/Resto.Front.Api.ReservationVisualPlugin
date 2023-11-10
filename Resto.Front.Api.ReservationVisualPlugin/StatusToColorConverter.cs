using Resto.Front.Api.Data.Brd;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Resto.Front.Api.ReservationVisualPlugin
{
    /// <summary>
    /// Конвертирует статус в цвет для отображения в окне
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {    
         /// <summary>
         /// Возвращает цвет стола
         /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TableStatus status)
            {
                switch (status)
                {
                    case TableStatus.Reserved:
                        return Brushes.Green;
                    case TableStatus.Started:
                        return Brushes.Blue;
                    case TableStatus.Free:
                        return Brushes.Gray;
                    // Цвет по умолчанию
                    default:
                        return Brushes.Gray; 
                }
            }
            // Цвет по умолчанию
            return Brushes.Gray; 
        }

        /// <summary>
        /// Не используется
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
