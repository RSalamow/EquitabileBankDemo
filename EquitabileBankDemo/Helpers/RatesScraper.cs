using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquitabileBankDemo.Helpers
{
    public class RatesScraper
    {
        public string Url { get; set; }
        private string _HtmlContent;

        public RatesScraper(string url)
        {
            Url = url;
        }

        public async Task<string> GetHtmlContentAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                _HtmlContent = await client.GetStringAsync(Url);
            }
            return _HtmlContent;
        }

        public DataTable ParseMortgageRates(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            var tableNode = document.DocumentNode.SelectSingleNode("//table[contains(@class, 'eq-table') and contains(@class, 'k-table')]");
            if (tableNode == null)
            {
                Console.WriteLine("Table not found.");
                return null;
            }
            DataTable dataTable = new DataTable();
            var headerNodes = tableNode.SelectNodes(".//tr[1]/th");
            foreach (var headerNode in headerNodes)
            {
                dataTable.Columns.Add(headerNode.InnerText.Trim());
            }
            var rowNodes = tableNode.SelectNodes(".//tr[position() > 1]");
            foreach (var rowNode in rowNodes)
            {
                DataRow dataRow = dataTable.NewRow();
                var cellNodes = rowNode.SelectNodes("./th|./td");
                for (int i = 0; i < cellNodes.Count && i < dataTable.Columns.Count; i++)
                {
                    dataRow[i] = cellNodes[i].InnerText.Trim();
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        public DataTable GetMortgageFlexRates(string html, RateType rateType)
        {
            DataTable ratesTable = new DataTable();
            var selectedRate = "";
            switch (rateType)
            {
                case RateType.Flex:
                    selectedRate = "Reverse Mortgage Flex Rate";
                    break;
                case RateType.FlexPLUS:
                    selectedRate = "Reverse Mortgage Flex PLUS Rate";
                    break;
                case RateType.FlexLite:
                    selectedRate = "Reverse Mortgage Flex Lite Rate";
                    break;
            }

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var tableNodes = document.DocumentNode.SelectNodes("//table[contains(@class, 'eq-table rates-table k-table')]");
            var tableNode = tableNodes 
                ?.FirstOrDefault(node => node.InnerText.Contains(selectedRate, StringComparison.OrdinalIgnoreCase));

            // Rates table
            if (tableNode == null)
                throw new Exception("Rates table not found.");

            var headerNodes = tableNode.SelectNodes(".//tr[1]/th");
            foreach (var headerNode in headerNodes)
            {
                ratesTable.Columns.Add(headerNode.InnerText.Trim());
            }

            var rowNodes = tableNode.SelectNodes(".//tr[position() > 1]");

            foreach (var rowNode in rowNodes)
            {
                DataRow dataRow = ratesTable.NewRow();
                var cellNodes = rowNode.SelectNodes("./th|./td");

                for (int i = 0; i < cellNodes.Count && i < ratesTable.Columns.Count; i++)
                {
                    dataRow[i] = cellNodes[i].InnerText.Trim().Replace('\u00A0', ' ').Trim();
                }

                ratesTable.Rows.Add(dataRow);
            }
            return ratesTable;
        }

    }

    public enum RateType
    {
        Flex,
        FlexPLUS,
        FlexLite
    }
}
