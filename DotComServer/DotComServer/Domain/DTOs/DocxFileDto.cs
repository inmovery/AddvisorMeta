namespace DotComServer.Domain.DTOs
{
	public class DocxFileDto
	{
		public DocxFileDto()
		{
		}

		public int Id { get; set; }

		public long Size { get; set; }

		public byte[] FileContent { get; set; }

		public string Filename { get; set; }
	}
}
