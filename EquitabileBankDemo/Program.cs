// See https://aka.ms/new-console-template for more information
using System.Data;
using EquitabileBankDemo.Helpers;
using HtmlAgilityPack;

Console.WriteLine("Hello, World!");
var url = "https://www.equitablebank.ca/mortgage-rates";

var ratesScraper = new RatesScraper(url);
var htmlContent = await ratesScraper.GetHtmlContentAsync();
var ratesTable = ratesScraper.ParseMortgageRates(htmlContent);
var flexRatesTable = ratesScraper.GetMortgageFlexRates(htmlContent, RateType.Flex);
var flexPlusRatesTable = ratesScraper.GetMortgageFlexRates(htmlContent, RateType.FlexPLUS);
var flexLiteRatesTable = ratesScraper.GetMortgageFlexRates(htmlContent, RateType.FlexLite);


DataTable rates;
ParseMortgageRates(url, out rates);




using (HttpClient client = new HttpClient())
{
    var html = await client.GetStringAsync(url);

    HtmlDocument document = new HtmlDocument();
    document.LoadHtml(html);

    var tableNode = document.DocumentNode.SelectSingleNode("//table[contains(@class, 'eq-table') and contains(@class, 'k-table')]");

    if (tableNode == null)
    {
        Console.WriteLine("Table not found.");
        return;
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

    foreach (DataRow row in dataTable.Rows)
    {
        foreach (DataColumn col in dataTable.Columns)
        {
            Console.Write($"{row[col]}\t");
        }
        Console.WriteLine();
    }
}

static void ParseMortgageRates(string url, out DataTable dataTable)
{
    dataTable = new DataTable();


    using (HttpClient client = new HttpClient())
    {
        var html = client.GetStringAsync(url).GetAwaiter().GetResult();

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(html);

        var tableNode = document.DocumentNode.SelectSingleNode("//table[contains(@class, 'eq-table') and contains(@class, 'k-table')]");
        var tableNodes = document.DocumentNode.SelectNodes("//table[contains(@class, 'eq-table rates-table k-table')]");

        var tableNodeFlex = tableNodes
            ?.FirstOrDefault(node => node.InnerText.Contains("Reverse Mortgage Flex Rate", StringComparison.OrdinalIgnoreCase));
        var tableNodeFlexPLUS = tableNodes
            ?.FirstOrDefault(node => node.InnerText.Contains("Reverse Mortgage Flex PLUS Rate", StringComparison.OrdinalIgnoreCase));
        var tableNodeFlexLite = tableNodes
            ?.FirstOrDefault(node => node.InnerText.Contains("Reverse Mortgage Flex Lite Rate", StringComparison.OrdinalIgnoreCase));
        
        // Rates table
        if (tableNode == null)
            throw new Exception("Rates table not found.");

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
                dataRow[i] = cellNodes[i].InnerText.Trim().Replace('\u00A0', ' ').Trim();
            }

            dataTable.Rows.Add(dataRow);
        }

        // Rates table Flex
        if (tableNode == null)
            throw new Exception("Rates Flex table not found.");

        headerNodes = tableNode.SelectNodes(".//tr[1]/th");
        foreach (var headerNode in headerNodes)
        {
            dataTable.Columns.Add(headerNode.InnerText.Trim());
        }

        rowNodes = tableNode.SelectNodes(".//tr[position() > 1]");

        foreach (var rowNode in rowNodes)
        {
            DataRow dataRow = dataTable.NewRow();
            var cellNodes = rowNode.SelectNodes("./th|./td");

            for (int i = 0; i < cellNodes.Count && i < dataTable.Columns.Count; i++)
            {
                dataRow[i] = cellNodes[i].InnerText.Trim().Replace('\u00A0', ' ').Trim();
            }

            dataTable.Rows.Add(dataRow);
        }

        // Rates table Flex
        if (tableNode == null)
            throw new Exception("Rates Flex table not found.");
        var ratesFlex = new DataTable();

        headerNodes = tableNode.SelectNodes(".//tr[1]/th");
        foreach (var headerNode in headerNodes)
        {
            ratesFlex.Columns.Add(headerNode.InnerText.Trim());
        }

        rowNodes = tableNode.SelectNodes(".//tr[position() > 1]");

        foreach (var rowNode in rowNodes)
        {
            DataRow dataRow = ratesFlex.NewRow();
            var cellNodes = rowNode.SelectNodes("./th|./td");

            for (int i = 0; i < cellNodes.Count && i < dataTable.Columns.Count; i++)
            {
                dataRow[i] = cellNodes[i].InnerText.Trim().Replace('\u00A0', ' ').Trim();
            }

            ratesFlex.Rows.Add(dataRow);
        }

        Console.WriteLine("");
    }
}