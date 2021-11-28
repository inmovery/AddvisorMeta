namespace DotComServer.Domain.Entities
{
	public class DocxFile
	{
		public int Id { get; set; }
		public long Size { get; set; }
		public string FileContent { get; set; }
		public string Filename { get; set; }
	}
}