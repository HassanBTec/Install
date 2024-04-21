/***
 winget install Microsoft.DotNet.SDK.8 or extract bin archive
 dotnet tool install -g dotnet-script
 (dotnet script .\Program.csx)
 another tool :
 dotnet tool install -g csharprepl

***/

#r "nuget: System.IO.Packaging, 8.0.0"
#r "C:\Users\DCG9678\Downloads\archive\DocumentFormat.OpenXml.Framework.dll"
#r "C:\Users\DCG9678\Downloads\archive\DocumentFormat.OpenXml.dll"
#r "C:\WINDOWS\assembly\GAC_MSIL\Microsoft.Office.Interop.Excel\15.0.0.0__71e9bce111e9429c\Microsoft.Office.Interop.Excel.dll"

using System;
using System.IO;
using System.IO.Packaging;
using System.Configuration;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml;  
using DocumentFormat.OpenXml.Packaging;  
using DocumentFormat.OpenXml.Spreadsheet;  


public class ConvProgram
{

        public void editFile(string file)  
        {    

            // Create a spreadsheet document by supplying the file name.  
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(file, SpreadsheetDocumentType.Workbook);  

            // Add a WorkbookPart to the document.  
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();  
            workbookpart.Workbook = new Workbook();  

            // Add a WorksheetPart to the WorkbookPart.  
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();  
            worksheetPart.Worksheet = new Worksheet(new SheetData());  

            // Add Sheets to the Workbook.  
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.  
                AppendChild<Sheets>(new Sheets());  

            // Append a new worksheet and associate it with the workbook.  
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.  
                GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };  
            sheets.Append(sheet);  

	        //IEnumerable<Row> ShRows = sheetData.Elements<Row>();
	        //var _RCount = SHRows.Count();

            // Close the document.  
            spreadsheetDocument.Dispose();  

            Console.WriteLine("The spreadsheet document has been created.");  

        }  


   public void convert(string files)
   {

        const string EXCEL_PROG_ID = "Excel.Application";

 	    string[] extensions = new[] { ".xls", ".csv", ".xlsm" };

        dynamic xlApp = null;
	    dynamic xlWorkBook = null;
	    dynamic sheet = null;
	    dynamic cell = null;


        try {
            if (null == xlApp)
                xlApp = Activator.CreateInstance(Type.GetTypeFromProgID(EXCEL_PROG_ID));

            if (null == xlApp)
            {
                Console.Write("Unable to start Excel");
                return;
            }

	        xlApp.DisplayAlerts = false;

            // xlWorkBook = xlApp.ActiveWorkbook ?? xlApp.Workbooks.Add();

	        DirectoryInfo dinfo = new DirectoryInfo(files);

            /* master workbook */
            string mfile = @"C:\Users\DCG9678\Downloads\result.xlsx";

            var xlMaster = xlApp.Workbooks.Open(mfile, 0, false);

            Excel.Worksheet msheet = (Excel.Worksheet)xlMaster.Worksheets.Item[1];

            Excel.Range rng = null;

            int rid = 1;

            foreach (FileInfo file in dinfo.GetFiles()
                                .Where(f => extensions.Contains(f.Extension.ToLower()))
                                .ToArray())
            {

                Console.WriteLine("-- Conversion  --" + file);

                if (file.Extension.ToLower() == ".csv" )
                    xlWorkBook = xlApp.Workbooks.Open(file.FullName, 
                                    Type.Missing, false, Type.Missing, Type.Missing, 
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                                    Type.Missing, Type.Missing);


                        /*xlWorkBook = xlApp.Workbooks.OpenText(file.FullName,
                                    DataType: Excel.XlTextParsingType.xlDelimited,
                                    TextQualifier: Excel.XlTextQualifier.xlTextQualifierNone,
                                    ConsecutiveDelimiter: true,
                                    Semicolon: true);*/
                else
                        xlWorkBook = xlApp.Workbooks.Open(file.FullName, 0, true);

                        sheet = xlWorkBook.ActiveSheet;

                /*** 
                *** Copy all to master sheet 
                ***/

                Excel.Range firstCell = null;

                if (sheet.UsedRange.Rows.Count > 0)
                    firstCell = sheet.Cells[1 + sheet.UsedRange.Rows.Count, 1];
                else
                    firstCell = sheet.Cells[1, 1];

                int LastRow = firstCell.get_End(Excel.XlDirection.xlUp).Row;

                        cell = msheet.Cells[rid + LastRow, 3];
                        cell.Value = sheet.Name;

                        cell = msheet.Cells[rid + LastRow, 4];
                        cell.Value = "Count";

                        cell = msheet.Cells[rid + LastRow, 5];
                        cell.Value = LastRow;

                Excel.Range range;

                if (LastRow > 0)
                    range = sheet.Range["A1:B" + LastRow];
                else
                    range = sheet.Range["A1:B2"];

                /* Master sheet */
                rng = (Excel.Range)msheet.Cells[rid, 1];

                range.Copy(rng);	

                    xlWorkBook.SaveAs(Filename: Path.ChangeExtension(file.FullName, ".xlsx"), FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook);

                        xlWorkBook.Close(); 

                rid = rid + LastRow;
                

                }
            
                xlMaster.Save();
                
                xlMaster.Close();

                    xlApp.Quit();

                Marshal.ReleaseComObject(xlMaster);

        }         
        catch(Exception ex)
        {
            Console.WriteLine("Exception:" + ex.Message); 

            Console.WriteLine(
                    new System.Diagnostics.StackTrace(true).ToString()
                );


            if (xlApp != null)
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);
        }

        Marshal.ReleaseComObject(xlWorkBook);
        Marshal.ReleaseComObject(xlApp);

   } 
 
}

string mfile = @"C:\Users\DCG9678\Downloads\result.xlsx";

new ConvProgram().editFile(mfile);

Console.WriteLine("*** Result file created.");

string path = @"C:\Users\DCG9678\Downloads\archive\xls";

new ConvProgram().convert(path);

Console.WriteLine("*** Conversion done.");
