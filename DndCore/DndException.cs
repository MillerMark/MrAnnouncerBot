using System;
using System.Linq;
using System.Runtime.Serialization;

namespace DndCore
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class DndException : Exception
	{
		// constructors...
		#region DndException()
		/// <summary>
		/// Constructs a new DndException.
		/// </summary>
		public DndException() { }
		#endregion
		#region DndException(string message)
		/// <summary>
		/// Constructs a new DndException.
		/// </summary>
		/// <param name="message">The exception message</param>
		public DndException(string message) : base(message) { }
		#endregion
		#region DndException(string message, Exception innerException)
		/// <summary>
		/// Constructs a new DndException.
		/// </summary>
		/// <param name="message">The exception message</param>
		/// <param name="innerException">The inner exception</param>
		public DndException(string message, Exception innerException) : base(message, innerException) { }
		#endregion
		#region DndException(SerializationInfo info, StreamingContext context)
		/// <summary>
		/// Serialization constructor.
		/// </summary>
		protected DndException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}
}
