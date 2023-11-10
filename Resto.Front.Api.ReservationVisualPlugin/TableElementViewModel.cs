namespace Resto.Front.Api.ReservationVisualPlugin
{    
     /// <summary>
     /// Модель представления стола
     /// </summary>
    public class TableElementViewModel
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public TableStatus Status { get; set; }
    }
}
