start-job {
  $sc = New-Object -ComObject MSScriptControl.ScriptControl.1
  $sc.Language = 'VBScript'
  $sc.AddCode('
    Function MyFunction(byval sFolder)

	csv_format = 6

	xlsx_format = 51

	Set oFSO = CreateObject("Scripting.FileSystemObject")

	Dim oExcel
	Set oExcel = CreateObject("Excel.Application")

	oExcel.Application.DisplayAlerts = False

	For Each oFile In oFSO.GetFolder(sFolder).Files

	  	If LCase(oFSO.GetExtensionName(oFile.Name)) = "xls" Then

		src_file = oFSO.GetAbsolutePathName(oFile)

		dest_file = Replace(Replace(src_file,".xlsx",".csv"),".xls",".csv")

		dest_xlsx = Replace(Replace(src_file,".xlsx",".csv"),".xls",".xlsx")

		Dim oBook
		Set oBook = oExcel.Workbooks.Open(src_file)

		Rem oBook.SaveAs dest_file, csv_format

		oBook.SaveAs dest_xlsx, xlsx_format

		oBook.Close False

  		End if
	Next

	Set oFSO = Nothing

	oExcel.Quit

    End Function
  ')
  
  $sc.codeobject.MyFunction("C:\Users\hbahri\Documents\")
} -runas32 | wait-job | receive-job
