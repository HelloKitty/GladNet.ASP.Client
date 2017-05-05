using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace GladNet.ASP.Client.Lib
{
	/// <summary>
	/// Contract for web-request handling functionality.
	/// Handles a serialized <see cref="PacketPayload"/> with the given request name.
	/// </summary>
	public interface IWebRequestEnqueueStrategy
	{
		/// <summary>
		/// Enqueues a webrequest to be handled with the provided serialized data.
		/// </summary>
		/// <param name="requestPayload">Request payload.</param>
		/// <returns>Returns the result of the enqueued request.</returns>
		SendResult EnqueueRequest([NotNull] PacketPayload requestPayload);

		/// <summary>
		/// Enqueues a webrequest to be handled with the provided request message.
		/// </summary>
		/// <param name="requestMessage">Request message.</param>
		/// <returns>Returns the result of the enqueued request.</returns>
		SendResult EnqueueRequest([NotNull] RequestMessage requestMessage);
	}
}
