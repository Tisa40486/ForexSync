namespace ForexSync.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public string Currency { get; set; } = string.Empty;

        public decimal Rate { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}