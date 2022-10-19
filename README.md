# ApiShared
Api Shared Core Application

.Net List to Excel Export (using ep-plus)
```csharp
            var result1 = repo.Query<CHARACTERISTIC_DETAIL>().ToList();
            var result2 = repo.Query<CHARACTERISTIC>().ToList();

            var excelBuilder = new ListExcelBuilder() { IsRTL = false };
            var excelResult = excelBuilder
                .Create()
                .AddSheet("CH_1", result1)
                .SetColumnProvider(new DefaultColumnProvider() { HasAttributeOnly = true })
                .AddSheet("CH_2", result2)
                .Build();

            File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\resutl1.xlsx", excelResult);
```
Ado.Net Command to Excel Export  (using ep-plus) support multi result set
```csharp
            var result3 = sqlExcelExport.GetFile<CHARACTERISTIC_DETAIL>(
                new SqlQueryDto(myBuilder.ConnectionString, "sELECT * FROM MasterData.CHARACTERISTIC_DETAIL"),
                new SqlExcelExportOption(new List<string> { "CH_3" })
            {
                Rtl = false,
                AutoFitColumns = true,
                //SheetTitles = new List<string>() { "sHEET 1" },
                HasRowNumber = false,                
            });
```
Import excel file to .NET List
```csharp
            var exportedFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\resutl2.xlsx");
            using var info = exportedFile.OpenRead();
            ExcelListImporter excelListImporter = new ExcelListImporter();

            excelListImporter.SetStream(info).LoadFile();
            excelListImporter.SetCurrentSheet("CH_3");

            var result4 = excelListImporter.GetList<CHARACTERISTIC_DETAIL>();
            Console.WriteLine(result4.Count);
```
