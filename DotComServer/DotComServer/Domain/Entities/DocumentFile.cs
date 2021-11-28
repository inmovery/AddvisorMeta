namespace DotComServer.Domain.Entities
{
	public class DocumentFile
	{
		public int Id { get; set; }
		public long Size { get; set; }
		public string FileContent { get; set; }
		public string Filename { get; set; }
	}
}