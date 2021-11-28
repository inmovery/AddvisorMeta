using Microsoft.Extensions.Logging;

namespace DotComServer.Loggers
{
	public static class FileLoggerExtensions
	{
		public static ILoggingBuilder AddFile(this ILoggingBuilder loggingBuilder, string filePath)
		{
			loggingBuilder.AddProvider(new FileLoggerProvider(filePath));
			return loggingBuilder;
		}
	}
}
