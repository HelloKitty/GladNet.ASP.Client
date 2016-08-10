using GladNet.Payload;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.RestSharp
{
	/// <summary>
	/// Contract for middleware that can act on <see cref="IRestRequest"/>s.
	/// </summary>
	public interface IRestSharpMiddleWare
	{
		/// <summary>
		/// Process the outgoing <see cref="IRestRequest"/> and returning false to indicate a failure.
		/// </summary>
		/// <param name="request">The rest request.</param>
		/// <param name="payload">The <see cref="PacketPayload"/> being sent.</param>
		/// <returns>True if the middleware processed the request and false if there was an error.</returns>
		bool ProcessOutgoingRequest(IRestRequest request, PacketPayload payload);

		/// <summary>
		/// Process the incoming <see cref="IRestResponse"/> and returning false to indicate a failure.
		/// </summary>
		/// <param name="request">The rest request.</param>
		/// <param name="payload">The <see cref="PacketPayload"/> being sent.</param>
		/// <returns>True if the middleware processed the request and false if there was an error.</returns>
		void ProcessIncomingResponse(IRestResponse response);
	}
}
