using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DotComServer.Domain.DTOs;
using DotComServer.Domain.Entities;
using DotComServer.Domain.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using ICell = NPOI.XWPF.UserModel.ICell;

namespace DotComServer.Business
{
	public class GlobalSearch
	{
		private readonly IDocxFileService _docxFileService;

		private readonly string _filename;
		private readonly XWPFDocument _docWrite = new();

		private string _lastFileName = string.Empty;

		public GlobalSearch(IDocxFileService docxFileService)
		{
			_docxFileService = docxFileService;

			_docWrite.GetProperties().CoreProperties.Creator = string.Empty;
			_filename = string.Empty;
		}

		public SearchResultDto DoSearch(string searchableContent, int id)
		{
			var searchMatchList = new List<SearchMatch>();

			var (documentData, extension) = ReadDocuments(id);

			switch (extension)
			{
				case "xls":
				case "xlsx":
					var excelDocument = documentData as List<DataTable>;
					searchMatchList = ParseExcelDocument(excelDocument, searchableContent);
					break;

				case "doc":
				case "docs":
					var wordDocument = documentData as List<string>;
					searchMatchList = ParseWordDocument(wordDocument, searchableContent);
					break;

				default:
					break;
			}

			var searchResultDto = new SearchResultDto()
			{
				Match = searchMatchList
			};

			return searchResultDto;
		}

		private List<SearchMatch> ParseWordDocument(List<string> lines, string searchableContent)
		{
			var searchMatchList = new List<SearchMatch>();

			var mergedContent = new StringBuilder();
			foreach (var line in lines)
				mergedContent.AppendLine(line);

			if (!mergedContent.ToString().Contains(searchableContent))
				searchMatchList.Add(new SearchMatch());

			var docxLines = mergedContent.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			for (var rowIndex = 0; rowIndex < docxLines.Length; rowIndex++)
			{
				if (!docxLines[rowIndex].Contains(searchableContent))
					continue;

				var searchMatch = new SearchMatch(rowIndex + 1, docxLines[rowIndex]);
				searchMatchList.Add(searchMatch);
			}

			return searchMatchList;
		}

		private List<SearchMatch> ParseExcelDocument(List<DataTable> tables, string searchableContent)
		{
			var searchMatchList = new List<SearchMatch>();
			for (var sheetIndex = 0; sheetIndex < tables.Count; sheetIndex++)
			{
				var data = tables[sheetIndex];
				for (var rowIndex = 0; rowIndex < data.Rows.Count; ++rowIndex)
				{
					for (var columnIndex = 0; columnIndex < data.Columns.Count; ++columnIndex)
					{
						var cellContent = data.Rows[rowIndex][columnIndex].ToString() ?? string.Empty;
						if (!cellContent.Contains(searchableContent))
							continue;

						var searchMatch = new SearchMatch(sheetIndex, rowIndex + 1, columnIndex + 1, cellContent);
						searchMatchList.Add(searchMatch);
					}
				}
			}

			return searchMatchList;
		}

		private (object, string) ReadDocuments(int id)
		{
			var data = new object();

			var documents = id == -1 ? _docxFileService.Get() : new List<DocxFileDto>() { _docxFileService.Get(id) };

			foreach (var document in documents)
			{

			}

			var fileData = _docxFileService.Get(id);
			var fileContent = fileData.FileContent;
			var fileExtension = Path.GetExtension(fileData.Filename)?.Replace(".", "");

			// By filename - File.OpenRead(_filename)
			var fileContentStream = new MemoryStream();
			fileContentStream.Write(fileContent, 0, fileContent.Length);
			fileContentStream.Position = 0;

			using (fileContentStream)
			{
				data = ConfigureContent(fileContentStream, fileExtension);
			}

			return (data, fileExtension);
		}

		private List<DataTable> ConfigureExcelDocument(IWorkbook workbook)
		{
			var allRowList = new List<string>();
			var dataTables = new List<DataTable>();
			var rowList = new List<string>();

			for (var sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
			{
				var dataTable = new DataTable();
				dataTable.Rows.Clear();
				dataTable.Columns.Clear();

				dataTables.Add(dataTable);
			}

			for (var sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
			{
				var sheet = workbook.GetSheetAt(sheetIndex);
				var headerRow = sheet.GetRow(0);
				int columnsCount = headerRow.LastCellNum;

				// Need necessary columns
				for (var cellIndex = 0; cellIndex < columnsCount; cellIndex++)
				{
					var cell = headerRow.GetCell(cellIndex);
					if (cell == null)
						continue;

					try
					{
						dataTables[sheetIndex].Columns.Add(cell.ToString());
					}
					catch (DuplicateNameException)
					{
						continue;
					}
				}

				var rowIndex = 0;
				var currentRow = sheet.GetRow(rowIndex);
				while (currentRow != null)
				{
					var dataRow = dataTables[sheetIndex].NewRow();
					for (var cellIndex = 0; cellIndex < currentRow.Cells.Count; cellIndex++)
					{
						var cell = currentRow.GetCell(cellIndex);
						if (cell != null)
						{
							try
							{
								dataRow[cellIndex] = cell.CellType switch
								{
									CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
										? cell.DateCellValue.ToString(CultureInfo.InvariantCulture)
										: cell.NumericCellValue.ToString(CultureInfo.InvariantCulture),
									CellType.String => cell.StringCellValue,
									CellType.Blank => string.Empty,
									_ => dataRow[cellIndex]
								};
							}
							catch
							{
								// ignored
							}
						}
					}

					dataTables[sheetIndex].Rows.Add(dataRow);
					rowIndex++;
					currentRow = sheet.GetRow(rowIndex);
				}

				//for (var rowIndex = (sheet.FirstRowNum + 1); rowIndex <= sheet.LastRowNum; rowIndex++)
				//{
				//	var row = sheet.GetRow(rowIndex);
				//	if (row == null)
				//		continue;

				//	if (row.Cells.All(d => d.CellType == CellType.Blank))
				//		continue;

				//	var dataRow = dataTables[sheetIndex].NewRow();
				//	for (var cellIndex = row.FirstCellNum; cellIndex < cellCount; cellIndex++)
				//	{
				//		var cell = row.GetCell(cellIndex);
				//		if (cell == null)
				//			continue;

				//		//var isCellEmpty = string.IsNullOrEmpty(cell.ToString());
				//		//var isCellConsistsFromWhiteSpaces = string.IsNullOrWhiteSpace(cell.ToString());

				//		//if (!isCellEmpty && !isCellConsistsFromWhiteSpaces)
				//		//	rowList.Add(cell.ToString());

				//		switch (cell.CellType)
				//		{
				//			case CellType.Numeric:
				//				dataRow[cellIndex] = DateUtil.IsCellDateFormatted(cell)
				//					? cell.DateCellValue.ToString(CultureInfo.InvariantCulture)
				//					: cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
				//				break;
				//			case CellType.String:
				//				dataRow[cellIndex] = cell.StringCellValue;
				//				break;
				//			case CellType.Blank:
				//				dataRow[cellIndex] = string.Empty;
				//				break;
				//		}
				//	}

				//	allRowList.AddRange(rowList);
				//	if (rowList.Count > 0)
				//		dataTables[sheetIndex].Rows.Add(dataRow);

				//	rowList.Clear();
				//}
			}

			return dataTables;
		}

		private List<string> ConfigureWordDocument(Stream fileContentStream)
		{
			var lines = new List<string>();

			var readableDocument = new XWPFDocument(fileContentStream);
			if (!readableDocument.Paragraphs.Any())
				return lines;

			var paragraphsContents = readableDocument.Paragraphs.Select(paragraph => paragraph.ParagraphText);
			lines.AddRange(paragraphsContents);

			var tablesContent = readableDocument.Tables.Select(table => table.Text);
			lines.AddRange(tablesContent);

			var images = readableDocument.AllPictures.Select(image => image.Data).ToList();

			// FontSize = docRead.Paragraphs[0].Runs[0].FontSize;
			// Font = docRead.Paragraphs[0].Runs[0].FontFamily;

			return lines;
		}

		private object ConfigureContent(Stream fileContentStream, string fileExtension)
		{
			var results = new List<string>();
			var dataTables = new List<DataTable>();
			switch (fileExtension)
			{
				case "doc":
				case "docx":
					return ConfigureWordDocument(fileContentStream);

				case "xls":
					var hssWorkbook = new HSSFWorkbook(fileContentStream);
					return ConfigureExcelDocument(hssWorkbook);

				case "xlsx":
					var xssWorkbook = new XSSFWorkbook(fileContentStream);
					return ConfigureExcelDocument(xssWorkbook);
				default:
					return new object();
			}
		}

		private bool IsMatchFound(string content)
		{
			return string.IsNullOrEmpty(content);
		}
	}
}
