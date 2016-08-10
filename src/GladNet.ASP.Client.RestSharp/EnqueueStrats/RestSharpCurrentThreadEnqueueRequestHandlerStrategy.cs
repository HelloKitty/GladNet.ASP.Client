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
using GladNet.Engine.Common;

namespace GladNet.ASP.Client.RestSharp
{
	/// <summary>
	/// Simple implementation of <see cref="IWebRequestEnqueueStrategy"/> that uses a single
	/// thread, the current thread as the caller, to handle a request. This strategy will block.
	/// </summary>
	public class RestSharpCurrentThreadEnqueueRequestHandlerStrategy : IWebRequestEnqueueStrategy, IMiddlewareRegistry
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

		/// <summary>
		/// The connection AUID of the paired connection for this enqueue strategy.
		/// </summary>
		private int pairedConnectionAUID { get; }

		private INetworkMessageRouteBackService messageRoutebackService { get; }

		/// <summary>
		/// Enumerable collection of <see cref="IRestSharpMiddleWare"/> services.
		/// </summary>
		private IList<IRestSharpMiddleWare> restsharpMiddlewares { get; }

		//Message responses are dispatched right after requests with this strategy. This is much different
		//than what is normally found in GladNet2 implementations.
		/// <summary>
		/// Creates a new <see cref="IWebRequestEnqueueStrategy"/> that handles requests on the calling thread
		/// and blocks.
		/// </summary>
		/// <param name="baseURL">Base string for the end-poind webserver. (Ex. www.google.com/)</param>
		/// <param name="deserializationService">Deserialization strategy for responses.</param>
		/// <param name="responseReciever">Message receiver service for dispatching recieved resposne messages.</param>
		public RestSharpCurrentThreadEnqueueRequestHandlerStrategy(string baseURL, IDeserializerStrategy deserializationService, ISerializerStrategy serializerStrategy, INetworkMessageReceiver responseReciever, int connectionAUID
#if !ENDUSER
			, INetworkMessageRouteBackService routebackService) //unfortunaly the single-threaded blocking enqueue strat needs the routeback service to function.
#endif
		{
			if (String.IsNullOrEmpty(baseURL))
				throw new ArgumentException($"Parameter {baseURL} is not valid. Either null or empty. Base URL must be the base URL of the web server. (Ex. www.google.com)");

			if (deserializationService == null)
				throw new ArgumentNullException(nameof(baseURL), $"Parameter {deserializationService} must not be null. Deserialization for incoming responses is required.");
#if !ENDUSER
			if (routebackService == null)
				throw new ArgumentNullException(nameof(routebackService), $"{nameof(RestSharpCurrentThreadEnqueueRequestHandlerStrategy)} requires {(nameof(INetworkMessageRouteBackService))} as it handles responses too.");
#endif
			serializer = serializerStrategy;
			deserializer = deserializationService;
			responseMessageRecieverService = responseReciever;

			restsharpMiddlewares = new List<IRestSharpMiddleWare>(2);
			httpClient = new RestClient(baseURL);

#if !ENDUSER
			messageRoutebackService = routebackService;
#endif
		}

		/// <summary>
		/// Attempts to register the middleware.
		/// </summary>
		/// <param name="middleware">The middleware to register.</param>
		/// <returns>Indicates if the registration was successful.</returns>
		public bool Register(IRestSharpMiddleWare middleware)
		{
			restsharpMiddlewares.Add(middleware);

			return true;
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

			return EnqueueRequest(new RequestMessage(requestPayload), requestPayload.GetType().Name, requestPayload);
		}

		public SendResult EnqueueRequest(RequestMessage requestMessage)
		{
			PacketPayload payload = requestMessage.Payload.Data; //reference to save the payload

			requestMessage.Payload.Serialize(serializer); //have to serialize payload first
			byte[] serializedData = serializer.Serialize(requestMessage);

			//This shouldn't happen but if it does we'll want some information on which type failed.
			if (serializedData == null)
				throw new InvalidOperationException($"Payload failed to serialize {requestMessage} of Type: {requestMessage?.Payload?.Data?.GetType()?.Name}.");

			string payloadName = requestMessage.Payload?.Data?.GetType().Name;

			if (payloadName == null)
				throw new InvalidOperationException($"Failed to determine endpoint URL due to invalid or unavailable payload name.");

			return EnqueueRequest(serializedData, payloadName, payload);
		}

		public SendResult EnqueueRequest(RequestMessage requestMessage, string payloadName, PacketPayload payload)
		{
			requestMessage.Payload.Serialize(serializer); //have to serialize payload first
			byte[] serializedData = serializer.Serialize(requestMessage);

			//This shouldn't happen but if it does we'll want some information on which type failed.
			if (serializedData == null)
				throw new InvalidOperationException($"Payload failed to serialize {requestMessage} of Type: {requestMessage?.Payload?.Data?.GetType()?.Name}.");

			if (payloadName == null)
				throw new InvalidOperationException($"Failed to determine endpoint URL due to invalid or unavailable payload name.");

			return EnqueueRequest(serializedData, payloadName, payload);
		}

		public SendResult EnqueueRequest(byte[] serializedRequest, string payloadName, PacketPayload payload)
		{
			//Create a new request that targets the API/RequestName controller
			//on the ASP server.
			RestRequest request = new RestRequest($"/api/{payloadName}", Method.POST);

			//TODO: Don't just assume serialization format
			//sends the request with the protobuf content type header.
			request.AddParameter("application/gladnet", serializedRequest, ParameterType.RequestBody);

			//Below we process outgoing and then incoming middlewares
			ProcessRequestMiddlewares(request, payload);

			IRestResponse response = httpClient.Post(request);

			ProcessResponseMiddlewares(response);

			//We should check the bytes returned in a response
			//We expect a NetworkMessage (in GladNet2 to preserve routing information)
			if (response.RawBytes != null && response.RawBytes.Count() > 0)
			{
				//With GladNet we expect to get back a NetworkMessage so we should attempt to deserialize one.
				ResponseMessage responseMessage = deserializer.Deserialize<ResponseMessage>(response.RawBytes);

#if !ENDUSER
				//If the message is routing back we don't need it to hit the user.
				//We can routeback now
				if(responseMessage.isRoutingBack)
				{
					//This is a difficult decision but we should indicate Sent after routing back as the message definitely was sent
					messageRoutebackService.Route(responseMessage, DefaultWebMessageParameters.Default); //TODO: Deal with message parameters

					return SendResult.Sent; //return, don't let the message be dispatched
				}

				//For GladNet2 2.x routing we have to push the AUID into the response message if it's not routing back
				responseMessage.Push(pairedConnectionAUID);
#endif

				responseMessage.Dispatch(responseMessageRecieverService, DefaultWebMessageParameters.Default); //TODO: Add message parameters.
			}

			return SendResult.Sent;
		}

		/// <summary>
		/// Handles outgoing requests using the internal registered middlewares.
		/// </summary>
		/// <param name="request">Request to process.</param>
		/// <param name="payload">Payload to process.</param>
		private void ProcessRequestMiddlewares(IRestRequest request, PacketPayload payload)
		{
			//We move forward through the middlewares to give them a chance
			//to process the requests.
			for (int i = 0; i < restsharpMiddlewares.Count; i++)
				restsharpMiddlewares[i].ProcessOutgoingRequest(request, payload);
		}


		/// <summary>
		/// Handles incoming responses using the internal registered middlewares.
		/// </summary>
		/// <param name="response">Request to process.</param>
		/// <param name="payload">Payload to process.</param>
		private void ProcessResponseMiddlewares(IRestResponse response)
		{
			//We move backwards through the middlewares to give them a chance
			//to process the responses.
			for (int i = restsharpMiddlewares.Count - 1; i >= 0; i--)
				restsharpMiddlewares[i].ProcessIncomingResponse(response);
		}
	}
}
