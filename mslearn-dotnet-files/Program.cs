using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var projectDirectory = Path.Combine(currentDirectory, "..", "..", "..");
var storesDirectory = Path.Combine(projectDirectory, "stores");
var salesTotalDir = Path.Combine(projectDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);
var salesFiles = FindFiles(storesDirectory);
var salesTotal = CalculateSalesTotal(salesFiles);
File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");
SalesSummaryReport(salesFiles, Path.Combine(salesTotalDir, "salesSummary.txt"));


IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        // The file name will contain the full path, so only check the end of it
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}




double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void SalesSummaryReport(IEnumerable<string> salesFiles, string reportFilePath)
{
    double overallTotal = 0;
    List<string> reportLines = new List<string>();

    reportLines.Add("Sales Summary");
    reportLines.Add("----------------------------");

    // gets details for each indiividual file
    reportLines.Add("Details:");
    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double total = data?.Total ?? 0;
        overallTotal += total;

        string fileName = Path.GetFileName(file);
        reportLines.Add($" {fileName}: ${total:N2}");
    }

    // adds the total sales line
    reportLines.Insert(2, $" Total Sales: ${overallTotal:N2}");

    // srites all calculations to the report file
    File.WriteAllLines(reportFilePath, reportLines);
}

record SalesData(double Total);

