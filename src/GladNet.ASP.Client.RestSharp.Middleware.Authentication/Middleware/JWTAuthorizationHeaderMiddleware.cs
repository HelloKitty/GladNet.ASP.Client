using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Payload;
using RestSharp;
using Easyception;
using Fasterflect;

namespace GladNet.ASP.Client.RestSharp.Middleware.Authentication
{
	/// <summary>
	/// <see cref="RestSharp"/> client middleware that puts the JWT authroization header into <see cref="IRestRequest"/>s
	/// that require authentication.
	/// </summary>
	public class JWTAuthorizationHeaderMiddleware : IRestSharpMiddleWare
	{
		private ITokenService tokenService { get; }

		public JWTAuthorizationHeaderMiddleware(ITokenService service)
		{
			if (service == null) throw new ArgumentNullException(nameof(service));

			tokenService = service;
		}

		public void ProcessIncomingResponse(IRestResponse response)
		{
			//We don't need to do anything in a response that had a JWT authorization added
		}

		public bool ProcessOutgoingRequest(IRestRequest request, PacketPayload payload)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			//If the payload requires authorization to handle on the server then we should
			//send the JWT header with the request.

			//Check if the payload requires authorizatation
			if (!payload.GetType().HasAttribute<AuthorizationRequiredAttribute>())
				return true; //return true means the middleware didn't encounter a failure

			//If an authorization required payload is being sent but we have no token we must
			//throw an error
			if (!tokenService.isTokenAvailable)
				throw new InvalidOperationException($"{nameof(ITokenService)} indicated the token was unavailable. Service cannot send {payload.GetType().Name} as it requires the token to authenticate.");

			request.AddHeader("Authorization", tokenService.FullTokenHeaderValue);

			return true;
		}
	}
}
