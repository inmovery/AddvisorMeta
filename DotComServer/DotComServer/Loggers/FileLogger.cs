using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DotComServer.Loggers
{
	public class FileLogger : ILogger
	{
		private readonly string _filepath;
		private readonly object _lock = new();

		public FileLogger(string path)
		{
			_filepath = path ?? throw new ArgumentNullException(nameof(path));
		}

		public IDisposable BeginScope<TState>(TState state) => default;

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (formatter == null)
				return;

			lock (_lock)
			{
				File.AppendAllText(_filepath, formatter(state, exception) + Environment.NewLine);
			}
		}
	}
}
