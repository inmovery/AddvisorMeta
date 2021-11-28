namespace DotComServer.Domain.Entities
{
	public class SearchMatch
	{
		public SearchMatch()
		{
			LineNumber = -1;
			TextContent = string.Empty;
		}

		public SearchMatch(int documentCounter, string textContent, bool isPresentation)
		{
			TextContent = textContent;
			if (isPresentation)
			{
				LineNumber = -1;
				SlideNumber = documentCounter;
			}
			else
			{
				LineNumber = documentCounter;
				SlideNumber = -1;
			}

			RowNumber = -1;
			CellNumber = -1;
			SheetNumber = -1;
		}

		public SearchMatch(int sheetNumber, int rowNumber, int cellNumber, string textContent)
		{
			LineNumber = -1;
			SlideNumber = -1;
			RowNumber = rowNumber;
			CellNumber = cellNumber;
			SheetNumber = sheetNumber;
			TextContent = textContent;
		}

		public int LineNumber { get; set; }

		public int RowNumber { get; set; }

		public int CellNumber { get; set; }

		public int SheetNumber { get; set; }

		public int SlideNumber { get; set; }

		public string TextContent { get; set; }
	}
}
