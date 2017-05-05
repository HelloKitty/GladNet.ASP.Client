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
using GladNet.Serializer;
using Booma.Payloads.ServerSelection;
using GladNet.Payload.Authentication;

namespace GladNet.ASP.Client.Test.Manual
{
	class Program
	{
		static void Main(string[] args)
		{
			new ProtobufnetRegistry().RegisterAuthenticationPayloads();
			new ProtobufnetRegistry().Register(typeof(SimpleGameServerDetailsModel));
			new ProtobufnetRegistry().Register(typeof(NetworkMessage));
			new ProtobufnetRegistry().Register(typeof(AuthenticationRequest));
			new ProtobufnetRegistry().Register(typeof(AuthenticationResponse));
			new ProtobufnetRegistry().Register(typeof(RequestMessage));
			new ProtobufnetRegistry().Register(typeof(ResponseMessage));
			new ProtobufnetRegistry().RegisterServerSelectionPayloads();

			Mock<INetworkMessageReceiver> reciever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			reciever.Setup(x => x.OnNetworkMessageReceive(It.IsAny<IResponseMessage>(), It.IsAny<IMessageParameters>()))
				.Callback<IResponseMessage, IMessageParameters>(Test);

			RestSharpCurrentThreadEnqueueRequestHandlerStrategy strat = new RestSharpCurrentThreadEnqueueRequestHandlerStrategy(@"https://localhost:44300", new ProtobufnetDeserializerStrategy(), new ProtobufnetSerializerStrategy(), reciever.Object, 0, Mock.Of<INetworkMessageRouteBackService>());

			JWTTokenServiceManager tokenServiceManager = new JWTTokenServiceManager();

			//fake token
			//tokenServiceManager.RegisterToken("eyJhbGciOiJSUzI1NiIsImtpZCI6IjYyN0YyQUFDMTZERTlENjNDMkY3NDQyQzk1OUFBNjEyQjIyOTlENDciLCJ0eXAiOiJKV1QiLCJ4NXQiOiJZbjhxckJiZW5XUEM5MFFzbFpxbUVySXBuVWMifQ.eyJBc3BOZXQuSWRlbnRpdHkuU2VjdXJpdHlTdGFtcCI6ImFjMWY2YTJmLTUyNzEtNDAxOS1iMDAyLTlhZjcxZjk5MWFjZCIsIlVzZXJOYW1lIjoiQWRtaW4iLCJqdGkiOiJkOWQ0NTQ2My1hYWNlLTQwZDktYjU2OS00YjhkM2EyYWQxNGIiLCJ1c2FnZSI6ImFjY2Vzc190b2tlbiIsInNjb3BlIjoidXNlciIsInN1YiI6ImQ1Njc5OTE2LTdkOTctNDZmMy04NmNlLTM1OTRiODBmMjhmZSIsIm5iZiI6MTQ3MDkwMTM5NCwiZXhwIjoxNDcwOTAzMTk0LCJpYXQiOjE0NzA5MDEzOTQsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzAwLyJ9.TfcaeRtk-rS8xJEH2moJiq_JjSyDiUKFkVpSzWbW2NRTdTIwyH3Asq84yj57vBrkbSE_gXdcUAqwtxseMF1-dBYfVFC0XxAaTPVNx17Ze-oSM9JjPM2MiZQvoVu-zaBXoJp_yiG1aKR8oAKuOjW_JDsZj35eBaPegC9hrcAq6usKTUqn-YrIgGulYBqAZQxoqHJpcjQMsXTFlsLSAb68AHD0PJa-3rWQQlc3qVx6K_wMkhCsAyN3v-VdkbhTC4uWQeJnxn159Ow9FT8SwS-hVb_f-fFX3I0jlhijWW2pH3IZWN93ZnYdxbuM1GY-49N0IjDuRa8YCDBnkZza_lkJkhjhzdSyZv3vX0IZNaBjB2wK49RtPCHHKEHc75tps2meWjy5kNGjTK0nRSfc6Gc-QtCkLFZRkbZTB7uW9i_mCWfk1m7Pzo_2tAclv9VRCQy0-IvvaeW8E5M8rYmxrRPGpPQsEKh7QdUlRoVeW28tyOK5MwR9HjhO-gAT6Fc9sCz6pXuf7G0tGiUINBeSCWhJIM4-71-kvf1C5oJJ76giknEnpf5i_vPke4gzk-uE4EdUQcrMBuRKMVI83Q8_0e4nQ0CekyZXQ2CDU_0Nh3oigRPiJa-QAiJOjWbzOHkOGgH-pyWbw2PWlmuZSJNMyP0QDQ0kztNzGUCHrNLNn35BccU");
			strat.RegisterAuthenticationMiddleware(new ProtobufnetSerializerStrategy(), new ProtobufnetDeserializerStrategy(), tokenServiceManager);
			//AuthRequest request = new AuthRequest(IPAddress.Broadcast, new LoginDetails("test", new byte[5]));

			//AuthenticationRequest authRequest = new AuthenticationRequest("Admin", "Test123$");

			strat.EnqueueRequest(new AuthenticationRequest("Admin", "Password69!"));

			//PacketPayload actualAuthRequest = (new ProtobufnetDeserializerStrategy()).Deserialize<PacketPayload>((new ProtobufnetSerializerStrategy().Serialize(request)));
			//strat.EnqueueRequest(new ProtobufnetSerializerStrategy().Serialize("hello"),
			//	client, nameof(AuthRequest));

			//strat.EnqueueRequest(null,
			//	client, nameof(AuthRequest));

			//strat.EnqueueRequest(actualAuthRequest);

			//Console.WriteLine($"isTokenAvailable: {tokenServiceManager.isTokenAvailable}\n\n\n\n Token: {tokenServiceManager.TokenString}\n\n\n\n TokenHeader: {tokenServiceManager.FullTokenHeaderValue}");
			Console.ReadKey();
		}

		private static void Test(IResponseMessage message, IMessageParameters parameters)
		{
			AuthenticationResponse payload = message.Payload.Data as AuthenticationResponse;

			Console.WriteLine($"Authenticated: {payload.AuthenticationSuccessful} OptionalError: {payload.OptionalError} OptionalErrorMessage: {payload.OptionalErrorMessage}");
		}
	}
}
