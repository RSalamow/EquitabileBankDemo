// See https://aka.ms/new-console-template for more information
using System.Data;
using HtmlAgilityPack;

Console.WriteLine("Hello, World!");
var url = "https://www.equitablebank.ca/mortgage-rates";

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