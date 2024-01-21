using System;

namespace Multiplayer
{
	internal static class SinkLogAdapter
	{
		public static void SetSink(Action<string> action)
		{
			NetworkLogger.SetSink(action);
		}

		public static void SetSinkError(Action<string> action)
		{
			NetworkLogger.SetSinkError(action);
		}

		static SinkLogAdapter()
		{
			NetworkLogger.SetSinkError(x => { 
				Console.ForegroundColor = ConsoleColor.Red; 
				Console.WriteLine(x);
				Console.ForegroundColor = ConsoleColor.White;
			});

			NetworkLogger.SetSink(x => Console.WriteLine(x));
		}
	}

	public static class NetworkLogger
	{
		public static void Log(object message) => _action.Invoke(message.ToString());

		public static void LogError(object message) => _actionError.Invoke(message.ToString());

		private static Action<string> _action;
		private static Action<string> _actionError;

		internal static void SetSink(Action<string> action)
		{
			_action = action;
		}

		internal static void SetSinkError(Action<string> action)
		{
			_actionError = action;
		}
	}
}
