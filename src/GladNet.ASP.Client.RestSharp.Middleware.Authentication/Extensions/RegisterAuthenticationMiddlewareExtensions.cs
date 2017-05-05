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
		/// <summary>
		/// Registers the Middlewares required for authentication.
		/// </summary>
		/// <typeparam name="TMiddlewareRegistryType"></typeparam>
		/// <param name="middlewarRegistry"></param>
		/// <param name="serializer"></param>
		/// <param name="deserializer"></param>
		/// <param name="tokenRegister"></param>
		/// <returns>Registy to fluently chain on.</returns>
		public static TMiddlewareRegistryType RegisterAuthenticationMiddleware<TMiddlewareRegistryType>(this TMiddlewareRegistryType middlewarRegistry, ISerializerStrategy serializer, IDeserializerStrategy deserializer, ITokenRegistry tokenRegister)
			where TMiddlewareRegistryType : IMiddlewareRegistry
		{
			//Register the middlewares
			middlewarRegistry.Register(new JWTAuthenticationMiddleware(deserializer, serializer, tokenRegister));

			//return for fluent chaining
			return middlewarRegistry;
		}

		/// <summary>
		/// Registers the Middlewares required for JWT authorization.
		/// </summary>
		/// <typeparam name="TMiddlewareRegistryType"></typeparam>
		/// <param name="middlewarRegistry"></param>
		/// <param name="serializer"></param>
		/// <param name="deserializer"></param>
		/// <param name="tokenService"></param>
		/// <returns>Registy to fluently chain on.</returns>
		public static TMiddlewareRegistryType RegisterAuthorizationMiddleware<TMiddlewareRegistryType>(this TMiddlewareRegistryType middlewarRegistry, ISerializerStrategy serializer, IDeserializerStrategy deserializer, ITokenService tokenService)
			where TMiddlewareRegistryType : IMiddlewareRegistry
		{
			//Register the middlewares
			middlewarRegistry.Register(new JWTAuthorizationHeaderMiddleware(tokenService));

			//return for fluent chaining
			return middlewarRegistry;
		}
	}
}
