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
using NPOI.POIFS.FileSystem;
using NPOI.POIFS.Storage;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using Syncfusion.DocIO.DLS;
using Syncfusion.Presentation;
using ICell = NPOI.XWPF.UserModel.ICell;
using FormatType = Syncfusion.DocIO.FormatType;
using IShape = Syncfusion.Presentation.IShape;

namespace DotComServer.Business
{
	public class GlobalSearch
	{
		private readonly IDocumentsFileService _documentsFileService;

		private readonly string _filename;
		private readonly XWPFDocument _docWrite = new();

		private string _lastFileName = string.Empty;

		public GlobalSearch(IDocumentsFileService documentsFileService)
		{
			_documentsFileService = documentsFileService;

			_docWrite.GetProperties().CoreProperties.Creator = string.Empty;
			_filename = string.Empty;
		}

		public SearchResultDto DoSearch(string searchableContent, int id = default)
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
				case "dot":
				case "docm":
				case "dotx":
				case "rtf":
					var wordDocument = documentData as List<string>;
					searchMatchList = ParseWordDocument(wordDocument, searchableContent);
					break;

				case "ppt":
				case "pptx":
				case "pptm":
				case "potx":
				case "potm":
					var powerPointDocument = documentData as IPresentation;
					searchMatchList = ParsePowerPointDocument(powerPointDocument, searchableContent);
					break;

				case "pdf":
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

				var searchMatch = new SearchMatch(rowIndex + 1, docxLines[rowIndex], false);
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

		private List<SearchMatch> ParsePowerPointDocument(IPresentation presentation, string searchableContent)
		{
			var searchMatchList = new List<SearchMatch>();

			var resultContent = new StringBuilder();
			foreach (var slide in presentation.Slides)
			{
				foreach (var slideItem in slide.Shapes)
				{
					var shape = (IShape)slideItem;

					// Check whether the shape is an auto-shape. Other types can be charts, tables or SmartArt diagrams.
					if (shape.SlideItemType == SlideItemType.AutoShape)
						resultContent.Append(shape.TextBody.Text);
				}
			}

			return searchMatchList;
		}

		private (object, string) ReadDocuments(int id)
		{
			object data;

			var document = new List<DocumentFileDto>() { _documentsFileService.Get(id) };

			var fileData = _documentsFileService.Get(id);
			var fileContent = fileData.FileContent;
			var fileExtension = Path.GetExtension(fileData.Filename)?.Replace(".", "");

			var fileContentStream = new MemoryStream();
			fileContentStream.Write(fileContent, 0, fileContent.Length);
			fileContentStream.Position = 0;

			using (fileContentStream)
			{
				data = ConfigureContent(fileContentStream, fileExtension);
			}

			return (data, fileExtension);
		}

		private List<DataTable> ConfigureExcelDocument(IWorkbook workbook = default)
		{
			var dataTables = new List<DataTable>();

			for (var sheetIndex = 0; sheetIndex < workbook?.NumberOfSheets; sheetIndex++)
			{
				var dataTable = new DataTable();
				dataTable.Rows.Clear();
				dataTable.Columns.Clear();

				dataTables.Add(dataTable);
			}

			for (var sheetIndex = 0; sheetIndex < workbook?.NumberOfSheets; sheetIndex++)
			{
				var sheet = workbook.GetSheetAt(sheetIndex);
				var headerRow = sheet.GetRow(0);
				int columnsCount = headerRow.LastCellNum;

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
						// ignored
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

		private IPresentation ConfigurePowerPointDocument(Stream fileContentStream)
		{
			var powerPointDocument = Presentation.Open(fileContentStream);

			return powerPointDocument;
		}

		private object ConfigureContent(Stream fileContentStream, string fileExtension)
		{
			switch (fileExtension)
			{
				case "doc":
				case "docs":
				case "dot":
				case "docm":
				case "dotx":
				case "rtf":
					return ConfigureWordDocument(fileContentStream);

				case "xls":
					var hssWorkbook = new HSSFWorkbook(fileContentStream);
					return ConfigureExcelDocument(hssWorkbook);

				case "xlsx":
					var xssWorkbook = new XSSFWorkbook(fileContentStream);
					return ConfigureExcelDocument(xssWorkbook);

				case "ppt":
				case "pptx":
				case "pptm":
				case "potx":
				case "potm":
					return ConfigurePowerPointDocument(fileContentStream);

				case "pdf":
					return new object();

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
