using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using RestSharp;
using GladNet.Serializer;
using GladNet.ASP.Client.Lib;

namespace GladNet.ASP.Client.RestSharp
{
	/// <summary>
	/// Simple implementation of <see cref="IWebRequestHandlerStrategy"/> that uses a single
	/// thread, the current thread as the caller, to handle a request. This strategy will block.
	/// </summary>
	public class RestSharpCurrentThreadEnqueueRequestHandlerStrategy : IWebRequestHandlerStrategy
	{
		/// <summary>
		/// Internal deserialization strategy for incoming response payloads.
		/// </summary>
		private IDeserializerStrategy deserializer { get; }

		/// <summary>
		/// Internal dispatching service for message receival.
		/// </summary>
		private INetworkMessageReceiver responseMessageRecieverService { get; }

		private IRestClient httpClient { get; }

		//Message responses are dispatched right after requests with this strategy. This is much different
		//than what is normally found in GladNet2 implementations.
		/// <summary>
		/// Creates a new <see cref="IWebRequestHandlerStrategy"/> that handles requests on the calling thread
		/// and blocks.
		/// </summary>
		/// <param name="deserializationService">Deserialization strategy for responses.</param>
		/// <param name="responseReciever">Message receiver service for dispatching recieved resposne messages.</param>
		public RestSharpCurrentThreadEnqueueRequestHandlerStrategy(string baseURL, IDeserializerStrategy deserializationService, INetworkMessageReceiver responseReciever)
		{
			httpClient = new RestClient(baseURL);
			deserializer = deserializationService;
			responseMessageRecieverService = responseReciever;
		}

		public SendResult EnqueueRequest(byte[] serializedRequest, string requestName)
		{
			//Create a new request that targets the API/RequestName controller
			//on the ASP server.
			RestRequest request = new RestRequest($"/api/{requestName}", Method.POST);

			//sends the request with the protobuf content type header.
			request.AddParameter("application/Protobuf-Net", serializedRequest, ParameterType.RequestBody);

			IRestResponse response = httpClient.Post(request);

			if(response.RawBytes != null && response.RawBytes.Count() > 0)
			{
				//With GladNet we expect to get back a PacketPayload so we should attempt to deserialize one.
				PacketPayload responsePayload = deserializer.Deserialize<PacketPayload>(response.RawBytes);

				responseMessageRecieverService.OnNetworkMessageReceive(new WebResponseNetworkMessageAdapter(responsePayload), null); //TODO: Add message parameters.
			}

			return SendResult.Sent;
		}
	}
}
