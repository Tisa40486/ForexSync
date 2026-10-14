using ForexSync.Dtos;
using System.Globalization;
using System.Xml.Linq;

namespace ForexSync.Services
{
    public class XmlParserService
    {
        public List<ExchangeRateDto> Parse(string xml)
        {
            var document = XDocument.Parse(xml);

            var result = new List<ExchangeRateDto>();

            var dateNodes = document.Descendants().Where(x => x.Name.LocalName == "Cube" && x.Attribute("time") != null);

            foreach (var dateNode in dateNodes)
            {
                var date = DateOnly.ParseExact((string)dateNode.Attribute("time")!, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                foreach (var currencyNode in dateNode.Elements().Where(x => x.Name.LocalName == "Cube"))
                {
                    var currency = (string?)currencyNode.Attribute("currency");

                    var rateString = (string?)currencyNode.Attribute("rate");

                    if (string.IsNullOrWhiteSpace(currency) || string.IsNullOrWhiteSpace(rateString))
                        throw new FormatException("Missing currency or rate in XML.");

                    var rate = decimal.Parse(rateString, NumberStyles.Float, CultureInfo.InvariantCulture);

                    if (currency.Length != 3 || rate <= 0)
                        throw new FormatException($"Invalid rate for {currency}.");

                    result.Add(new ExchangeRateDto( date, currency.ToUpperInvariant(), rate));
                }
            }

            if (result.Count == 0)
                throw new FormatException("No exchange rates found in the XML.");

            if (result.GroupBy(x => new { x.Date, x.Currency }).Any(g => g.Count() > 1))
                throw new FormatException("Duplicate exchange rates found.");

            return result;
        }
    }
}