using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Payload;
using RestSharp;

namespace GladNet.ASP.Client.RestSharp.Middleware.Authentication
{
	/// <summary>
	/// <see cref="RestSharp"/> client middleware that puts the JWT authroization header into <see cref="IRestRequest"/>s
	/// that require authentication.
	/// </summary>
	public class JWTAuthorizationHeaderMiddleware : IRestSharpMiddleWare
	{
		public void ProcessIncomingResponse(IRestResponse response)
		{
			throw new NotImplementedException();
		}

		public bool ProcessOutgoingRequest(IRestRequest request, PacketPayload payload)
		{
			//If the payload requires authorization to handle on the server then we should
			//send the JWT header with the request.

			request.AddHeader("Authorization", )
		}
	}
}
