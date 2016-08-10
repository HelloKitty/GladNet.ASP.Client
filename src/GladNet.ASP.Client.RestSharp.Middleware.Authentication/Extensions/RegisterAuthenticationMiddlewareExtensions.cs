using GladNet.ASP.Client.RestSharp;
using GladNet.ASP.Client.RestSharp.Middleware.Authentication;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client
{
	/// <summary>
	/// Extensions for registering the middlewares.
	/// </summary>
	public static class RegisterAuthenticationMiddlewareExtensions
	{
		public static TMiddlewareRegistryType RegisterAuthenticationMiddlewares<TMiddlewareRegistryType>(this TMiddlewareRegistryType middlewarRegistry, ISerializerStrategy serializer, IDeserializerStrategy deserializer, ITokenRegistry tokenRegister, ITokenService tokenService)
			where TMiddlewareRegistryType : IMiddlewareRegistry
		{
			//Register the middlewares
			middlewarRegistry.Register(new JWTAuthenticationMiddleware(deserializer, serializer, tokenRegister));
			middlewarRegistry.Register(new JWTAuthorizationHeaderMiddleware(tokenService));

			//return for fluent chaining
			return middlewarRegistry;
		}
	}
}
