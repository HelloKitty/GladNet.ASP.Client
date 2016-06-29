using GladLive.Web.Payloads.Authentication;
using GladNet.ASP.Client.Lib;
using GladNet.Common;
using GladNet.Serializer.Protobuf;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Common.Logging;
using GladNet.ASP.Client.RestSharp;

namespace GladNet.ASP.Client.Test.Manual
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ReadKey();

			//client.AddHandler("application/Protobuf-Net", )

			Mock<INetworkMessageReceiver> reciever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			reciever.Setup(x => x.OnNetworkMessageReceive(It.IsAny<IResponseMessage>(), It.IsAny<IMessageParameters>()))
				.Callback<IResponseMessage, IMessageParameters>(Test);

			RestSharpCurrentThreadEnqueueRequestHandlerStrategy strat = new RestSharpCurrentThreadEnqueueRequestHandlerStrategy(@"http://localhost:5000", new ProtobufnetDeserializerStrategy(), reciever.Object);

			new ProtobufnetRegistry().Register(typeof(AuthRequest));
			new ProtobufnetRegistry().Register(typeof(AuthResponse));

			AuthRequest request = new AuthRequest(IPAddress.Broadcast, new LoginDetails("test", new byte[5]));


			PacketPayload actualAuthRequest = (new ProtobufnetDeserializerStrategy()).Deserialize<PacketPayload>((new ProtobufnetSerializerStrategy().Serialize(request)));
			//strat.EnqueueRequest(new ProtobufnetSerializerStrategy().Serialize("hello"),
			//	client, nameof(AuthRequest));

			//strat.EnqueueRequest(null,
			//	client, nameof(AuthRequest));

			strat.EnqueueRequest(new ProtobufnetSerializerStrategy().Serialize(actualAuthRequest),
				nameof(AuthRequest));

			Console.ReadKey();
		}

		private static void Test(IResponseMessage message, IMessageParameters parameters)
		{
			Console.WriteLine(((AuthResponse)message.Payload.Data).ResponseCode);
		}
	}
}
