using System;
using Microsoft.Extensions.Logging;

namespace DotComServer.Loggers
{
	public class FileLoggerProvider : ILoggerProvider
	{
		private readonly string _filePath;

		public FileLoggerProvider(string filePath)
		{
			_filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new FileLogger(_filePath);
		}

		public void Dispose()
		{
		}
	}
}
