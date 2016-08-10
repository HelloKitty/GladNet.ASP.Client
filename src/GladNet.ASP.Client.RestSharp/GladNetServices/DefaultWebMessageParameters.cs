using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;

namespace GladNet.ASP.Client.RestSharp
{
	/// <summary>
	/// Web <see cref="IMessageParameters"/> implementation.
	/// </summary>
	public class DefaultWebMessageParameters : IMessageParameters
	{
		public static DefaultWebMessageParameters Default = new DefaultWebMessageParameters();

		/// <summary>
		/// Default web channel is 0.
		/// </summary>
		public byte Channel { get; } = 0;

		/// <summary>
		/// Indicates TCP/WEB style delivery.
		/// </summary>
		public DeliveryMethod DeliveryMethod { get; } = DeliveryMethod.ReliableOrdered;

		//TODO: Implement HTTPS checking.
		/// <summary>
		/// Indicates if HTTPS was used.
		/// </summary>
		public bool Encrypted { get; } = false;
	}
}
