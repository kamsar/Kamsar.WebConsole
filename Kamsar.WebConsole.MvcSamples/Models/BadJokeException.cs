using System;
using System.Runtime.Serialization;

namespace Kamsar.WebConsole.MvcSamples.Models
{
	[Serializable]
	public class BadJokeException : Exception
	{
		public BadJokeException()
		{
		}

		public BadJokeException(string message) : base(message)
		{
		}

		public BadJokeException(string message, Exception inner) : base(message, inner)
		{
		}

		protected BadJokeException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}	
}