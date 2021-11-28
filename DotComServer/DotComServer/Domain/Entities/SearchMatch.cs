namespace DotComServer.Domain.Entities
{
	public class SearchMatch
	{
		public SearchMatch()
		{
			LineNumber = -1;
			SlideNumber = -1;
			RowNumber = -1;
			CellNumber = -1;
			SheetNumber = -1;
			TextContent = string.Empty;
			Filename = string.Empty;
			FromAttachment = string.Empty;
		}

		public SearchMatch(int documentCounter, string textContent, bool isPresentation, string filename, string fromAttachment)
		{
			TextContent = textContent;
			Filename = filename;
			FromAttachment = fromAttachment;
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

		public SearchMatch(int sheetNumber, int rowNumber, int cellNumber, string textContent, string filename, string fromAttachment)
		{
			LineNumber = -1;
			SlideNumber = -1;
			RowNumber = rowNumber;
			CellNumber = cellNumber;
			SheetNumber = sheetNumber;
			TextContent = textContent;
			Filename = filename;
			FromAttachment = fromAttachment;
		}

		public string Filename { get; set; }

		public string FromAttachment { get; set; }

		public int LineNumber { get; set; }

		public int RowNumber { get; set; }

		public int CellNumber { get; set; }

		public int SheetNumber { get; set; }

		public int SlideNumber { get; set; }

		public string TextContent { get; set; }
	}
}
