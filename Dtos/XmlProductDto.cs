namespace ForexSync.Dtos
{
    public record ExchangeRateDto(DateOnly Date, string Currency, decimal Rate);
}