namespace DotComServer.Domain.Entities
{
	public class SearchMatch
	{
		public SearchMatch()
		{
			Line = -1;
			TextContent = string.Empty;
		}

		public SearchMatch(int line, string textContent)
		{
			Line = line;
			TextContent = textContent;
			RowNumber = -1;
			CellNumber = -1;
			SheetNumber = -1;
		}

		public SearchMatch(int sheetNumber, int rowNumber, int cellNumber, string textContent)
		{
			Line = -1;
			RowNumber = rowNumber;
			CellNumber = cellNumber;
			SheetNumber = sheetNumber;
			TextContent = textContent;
		}

		public int Line { get; set; }

		public int RowNumber { get; set; }

		public int CellNumber { get; set; }

		public int SheetNumber { get; set; }

		public string TextContent { get; set; }
	}
}
