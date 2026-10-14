using ForexSync.Dtos;
using ForexSync.Models;
using ForexSync.Data;
using Microsoft.EntityFrameworkCore;

namespace ForexSync.Services
{
    public class SynchronizationService
    {
        private readonly AppDbContext _context;

        public SynchronizationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SynchronizeAsync( List<ExchangeRateDto> rates, CancellationToken cancellationToken = default)
        {
            if (rates.Count == 0)
                return;

            int inserted = 0;
            int updated = 0;

            var minDate = rates.Min(x => x.Date);
            var maxDate = rates.Max(x => x.Date);

            var existingRates = await _context.ExchangeRates.Where(x => x.Date >= minDate && x.Date <= maxDate).ToListAsync(cancellationToken);

            var dictionary = existingRates.ToDictionary(x => (x.Date, x.Currency));

            foreach (var rate in rates)
            {
                var key = (rate.Date, rate.Currency);

                if (dictionary.TryGetValue(key, out var existing))
                {
                    if (existing.Rate != rate.Rate)
                    {
                        existing.Rate = rate.Rate;
                        existing.UpdatedAt = DateTime.UtcNow;

                        updated++;
                    }
                }
                else
                {
                    var entity = new ExchangeRate
                    {
                        Date = rate.Date,
                        Currency = rate.Currency,
                        Rate = rate.Rate,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.ExchangeRates.Add(entity);
                    dictionary.Add(key, entity);

                    inserted++;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"Import done : {inserted} creations, {updated} updates, {rates.Count - inserted - updated} unchanged.");
        }
    }
}