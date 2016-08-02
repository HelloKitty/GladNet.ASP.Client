using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using RestSharp;
using GladNet.Serializer;
using GladNet.ASP.Client.Lib;
using GladNet.Message;
using GladNet.Payload;

namespace GladNet.ASP.Client.RestSharp
{
	/// <summary>
	/// Simple implementation of <see cref="IWebRequestEnqueueStrategy"/> that uses a single
	/// thread, the current thread as the caller, to handle a request. This strategy will block.
	/// </summary>
	public class RestSharpCurrentThreadEnqueueRequestHandlerStrategy : IWebRequestEnqueueStrategy
	{
		/// <summary>
		/// Internal deserialization strategy for incoming response payloads.
		/// </summary>
		private IDeserializerStrategy deserializer { get; }

		/// <summary>
		/// Internal serialization strategy for outgoing response payloads.
		/// </summary>
		private ISerializerStrategy serializer { get; }

		/// <summary>
		/// Internal dispatching service for message receival.
		/// </summary>
		private INetworkMessageReceiver responseMessageRecieverService { get; }

		/// <summary>
		/// Internal <see cref="RestSharp"/> HTTP client.
		/// </summary>
		private IRestClient httpClient { get; }

		//Message responses are dispatched right after requests with this strategy. This is much different
		//than what is normally found in GladNet2 implementations.
		/// <summary>
		/// Creates a new <see cref="IWebRequestEnqueueStrategy"/> that handles requests on the calling thread
		/// and blocks.
		/// </summary>
		/// <param name="baseURL">Base string for the end-poind webserver. (Ex. www.google.com/)</param>
		/// <param name="deserializationService">Deserialization strategy for responses.</param>
		/// <param name="responseReciever">Message receiver service for dispatching recieved resposne messages.</param>
		public RestSharpCurrentThreadEnqueueRequestHandlerStrategy(string baseURL, IDeserializerStrategy deserializationService, INetworkMessageReceiver responseReciever)
		{
			if (String.IsNullOrEmpty(baseURL))
				throw new ArgumentException($"Parameter {baseURL} is not valid. Either null or empty. Base URL must be the base URL of the web server. (Ex. www.google.com)");

			if (deserializationService == null)
				throw new ArgumentNullException(nameof(baseURL), $"Parameter {deserializationService} must not be null. Deserialization for incoming responses is required.");
		
			deserializer = deserializationService;
			responseMessageRecieverService = responseReciever;

			httpClient = new RestClient(baseURL);
		}

		/// <summary>
		/// Enqueues a webrequest to be handled with the provided serialized data.
		/// This 
		/// </summary>
		/// <param name="requestPayload">Request payload.</param>
		/// <param name="requestName">String <see cref="Type"/> name of the <see cref="PacketPayload"/> type.</param>
		/// <returns>Returns the result of the enqueued request.</returns>
		public SendResult EnqueueRequest(PacketPayload requestPayload)
		{
			//Check param
			if (requestPayload == null)
				throw new ArgumentNullException(nameof(requestPayload), $"Provided parameter request payload {requestPayload} is null.");

			return EnqueueRequest(new RequestMessage(requestPayload), requestPayload.GetType().Name);
		}

		public SendResult EnqueueRequest(RequestMessage requestMessage)
		{
			requestMessage.Payload.Serialize(serializer); //have to serialize payload first
			byte[] serializedData = serializer.Serialize(requestMessage);

			//This shouldn't happen but if it does we'll want some information on which type failed.
			if (serializedData == null)
				throw new InvalidOperationException($"Payload failed to serialize {requestMessage} of Type: {requestMessage?.Payload?.Data?.GetType()?.Name}.");

			string payloadName = requestMessage.Payload?.Data?.GetType().Name;

			if (payloadName == null)
				throw new InvalidOperationException($"Failed to determine endpoint URL due to invalid or unavailable payload name.");

			return EnqueueRequest(serializedData, payloadName);
		}

		public SendResult EnqueueRequest(RequestMessage requestMessage, string payloadName)
		{
			requestMessage.Payload.Serialize(serializer); //have to serialize payload first
			byte[] serializedData = serializer.Serialize(requestMessage);

			//This shouldn't happen but if it does we'll want some information on which type failed.
			if (serializedData == null)
				throw new InvalidOperationException($"Payload failed to serialize {requestMessage} of Type: {requestMessage?.Payload?.Data?.GetType()?.Name}.");

			if (payloadName == null)
				throw new InvalidOperationException($"Failed to determine endpoint URL due to invalid or unavailable payload name.");

			return EnqueueRequest(serializedData, payloadName);
		}

		public SendResult EnqueueRequest(byte[] serializedRequest, string payloadName)
		{
			//Create a new request that targets the API/RequestName controller
			//on the ASP server.
			RestRequest request = new RestRequest($"/api/{payloadName}", Method.POST);

			//TODO: Don't just assume serialization format
			//sends the request with the protobuf content type header.
			request.AddParameter("application/Protobuf-Net", serializedRequest, ParameterType.RequestBody);

			IRestResponse response = httpClient.Post(request);

			//We should check the bytes returned in a response
			//We expect a NetworkMessage (in GladNet2 to preserve routing information)
			if (response.RawBytes != null && response.RawBytes.Count() > 0)
			{
				//With GladNet we expect to get back a NetworkMessage so we should attempt to deserialize one.
				NetworkMessage responseMessage = deserializer.Deserialize<NetworkMessage>(response.RawBytes);

				responseMessage.Dispatch(responseMessageRecieverService, null); //TODO: Add message parameters.
			}

			return SendResult.Sent;
		}
	}
}
