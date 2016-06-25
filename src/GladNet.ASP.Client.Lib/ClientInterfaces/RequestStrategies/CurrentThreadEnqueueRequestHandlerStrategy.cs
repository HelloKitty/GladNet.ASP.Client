using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using RestSharp;
using GladNet.Serializer;

namespace GladNet.ASP.Client.Lib
{
	public class CurrentThreadEnqueueRequestHandlerStrategy : IWebRequestHandlerStrategy
	{
		private IDeserializerStrategy deserializer { get; }

		private INetworkMessageReceiver responseMessageRecieverService { get; }

		public CurrentThreadEnqueueRequestHandlerStrategy(IDeserializerStrategy deserializationService, INetworkMessageReceiver responseReciever)
		{
			deserializer = deserializationService;
			responseMessageRecieverService = responseReciever;
		}

		public SendResult EnqueueRequest(byte[] serializedRequest, IRestClient webClient, string requestName)
		{
			//Create a new request that targets the API/RequestName controller
			//on the ASP server.
			RestRequest request = new RestRequest($"/api/{requestName}", Method.POST);

			//sends the request with the protobuf content type header.
			request.AddParameter("application/Protobuf-Net", serializedRequest, ParameterType.RequestBody);

			IRestResponse response = webClient.Post(request);

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
