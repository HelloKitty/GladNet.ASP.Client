﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Payload;
using RestSharp;
using GladNet.Serializer;
using GladNet.Message;
using GladNet.Payload.Authentication;

namespace GladNet.ASP.Client.RestSharp.Middleware.Authentication
{
	/// <summary>
	/// <see cref="RestSharp"/> client middleware that handles ingoing and outgoing JWT authentication.
	/// </summary>
	public class JWTAuthenticationMiddleware : IRestSharpMiddleWare
	{
		/// <summary>
		/// Deserializer to use for spoofing request/responses.
		/// </summary>
		private IDeserializerStrategy deserializerStrategy { get; }

		/// <summary>
		/// Serializer to use for spoofing request/responses
		/// </summary>
		private ISerializerStrategy serializerStrategy { get; }

		public JWTAuthenticationMiddleware(IDeserializerStrategy deserializer, ISerializerStrategy serializer)
		{
			deserializerStrategy = deserializer;
		}

		/// <summary>
		/// Process the incoming <see cref="IRestResponse"/> and returning false to indicate a failure.
		/// </summary>
		/// <param name="request">The rest request.</param>
		/// <param name="payload">The <see cref="PacketPayload"/> being sent.</param>
		/// <returns>True if the middleware processed the request and false if there was an error.</returns>
		public void ProcessIncomingResponse(IRestResponse response)
		{
			//TODO: Add Header to JWT response.
			//if the contenttype is not for JWT we can ignore
			if(response.ContentType == null || response.ContentType.Contains("application/json"))
			{
				return;
			}

			//If the response is HTTP OK then authentication was successful
			AuthenticationResponse responsePayload = response.StatusCode == System.Net.HttpStatusCode.OK ? new AuthenticationResponse() //create a success result.
				: BuildResponseFromContent(response.Content); 

			//We spoof the GladNet2 response based on the incoming JSON object.
			ResponseMessage message = new ResponseMessage(responsePayload); 

			//serialize payload
			message.Payload.Serialize(serializerStrategy);

			//spoof the response by replacing the bytes with a response message
			response.RawBytes = serializerStrategy.Serialize(message);

			//TODO: Grab the accesstoken and push it into a service.
		}

		private AuthenticationResponse BuildResponseFromContent(string content)
		{
			return new AuthenticationResponse(ResponseErrorCode.Error, $"Failed to authenticate with the server.");
		}

		/// <summary>
		/// Process the outgoing <see cref="IRestRequest"/> and returning false to indicate a failure.
		/// </summary>
		/// <param name="request">The rest request.</param>
		/// <param name="payload">The <see cref="PacketPayload"/> being sent.</param>
		/// <returns>True if the middleware processed the request and false if there was an error.</returns>
		public bool ProcessOutgoingRequest(IRestRequest request, PacketPayload payload)
		{
			AuthenticationRequest requestPayload = payload as AuthenticationRequest;

			//We return true to indicate the middleware functioned correctly
			if (requestPayload == null)
				return true;

			//Clear out the normal parameters
			request.Parameters.Clear();

			//the JWT service is expecting authentication to be query string in the body
			//In this form: username=Admin&password=Test123$&grant_type=password
			request.AddParameter("application/x-www-form-urlencoded", $"username={requestPayload.UserName}&password={requestPayload.Password}&grant_type=password", ParameterType.RequestBody);

			return true;
		}
	}
}