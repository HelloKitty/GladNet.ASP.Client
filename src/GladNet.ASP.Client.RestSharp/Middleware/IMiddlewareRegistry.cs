using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.ASP.Client.RestSharp
{
	/// <summary>
	/// A middleware registry.
	/// </summary>
	public interface IMiddlewareRegistry
	{
		/// <summary>
		/// Attempts to register the middleware.
		/// </summary>
		/// <param name="middleware">The middleware to register.</param>
		/// <returns>Indicates if the registration was successful.</returns>
		bool Register(IRestSharpMiddleWare middleware);
	}
}
