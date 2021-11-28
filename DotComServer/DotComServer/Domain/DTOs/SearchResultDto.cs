using System.Collections.Generic;
using DotComServer.Domain.Entities;

namespace DotComServer.Domain.DTOs
{
	public class SearchResultDto
	{
		public SearchResultDto()
		{
		}

		public List<SearchMatch> Match { get; set; }
	}
}
