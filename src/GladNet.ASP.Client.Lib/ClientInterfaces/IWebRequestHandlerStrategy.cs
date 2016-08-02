using GladNet.Common;
using GladNet.Payload;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.Lib
{
	/// <summary>
	/// Contract for web-request handling functionality.
	/// Handles a serialized <see cref="PacketPayload"/> with the given request name.
	/// </summary>
	public interface IWebRequestHandlerStrategy
	{
		/// <summary>
		/// Enqueues a webrequest to be handled with the provided serialized data.
		/// </summary>
		/// <param name="requestPayload">Request payload.</param>
		/// <param name="requestName">String <see cref="Type"/> name of the <see cref="PacketPayload"/> type.</param>
		/// <returns>Returns the result of the enqueued request.</returns>
		SendResult EnqueueRequest(PacketPayload requestPayload);
	}
}
