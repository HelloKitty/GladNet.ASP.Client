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
using GladNet.Message;
using GladNet.Payload;
using GladNet.Engine.Common;
using GladNet.ASP.Client.RestSharp.Middleware.Authentication;
using GladNet.Payload.Authentication;
using GladNet.Serializer;

namespace GladNet.ASP.Client.Test.Manual
{
	class Program
	{
		static void Main(string[] args)
		{
			new ProtobufnetRegistry().RegisterAutenticationPayloads();
			new ProtobufnetRegistry().Register(typeof(NetworkMessage));
			new ProtobufnetRegistry().Register(typeof(RequestMessage));	
			new ProtobufnetRegistry().Register(typeof(ResponseMessage));

			Mock<INetworkMessageReceiver> reciever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			reciever.Setup(x => x.OnNetworkMessageReceive(It.IsAny<IResponseMessage>(), It.IsAny<IMessageParameters>()))
				.Callback<IResponseMessage, IMessageParameters>(Test);

			RestSharpCurrentThreadEnqueueRequestHandlerStrategy strat = new RestSharpCurrentThreadEnqueueRequestHandlerStrategy(@"http://localhost:5000", new ProtobufnetDeserializerStrategy(), new ProtobufnetSerializerStrategy(), reciever.Object, 0, Mock.Of<INetworkMessageRouteBackService>());

			JWTTokenServiceManager tokenServiceManager = new JWTTokenServiceManager();

			strat.RegisterAuthenticationMiddlewares(new ProtobufnetSerializerStrategy(), new ProtobufnetDeserializerStrategy(), tokenServiceManager, tokenServiceManager);
			//AuthRequest request = new AuthRequest(IPAddress.Broadcast, new LoginDetails("test", new byte[5]));

			AuthenticationRequest authRequest = new AuthenticationRequest("Admin", "Test123$");

			strat.EnqueueRequest(authRequest);

			//PacketPayload actualAuthRequest = (new ProtobufnetDeserializerStrategy()).Deserialize<PacketPayload>((new ProtobufnetSerializerStrategy().Serialize(request)));
			//strat.EnqueueRequest(new ProtobufnetSerializerStrategy().Serialize("hello"),
			//	client, nameof(AuthRequest));

			//strat.EnqueueRequest(null,
			//	client, nameof(AuthRequest));

			//strat.EnqueueRequest(actualAuthRequest);

			Console.ReadKey();
		}

		private static void Test(IResponseMessage message, IMessageParameters parameters)
		{
			Console.WriteLine("Recieved response");
		}
	}
}
