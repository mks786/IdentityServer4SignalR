﻿using System;
using System.Linq;
using Akka.Util.Internal;
using Microsoft.AspNetCore.Builder;

namespace ChatAPI.Auth
{

    /// <summary>
    /// Middleware to intercept a query string bearer token value (since SignalR isn't
    /// able to use a Header) into an auth header so that the Jwt header can handle it.
    /// </summary>
    public static class JwtSignalRExtensions
    {
        private static readonly String AUTH_QUERY_STRING_KEY = "authtoken";

        public static void UseJwtSignalRAuthentication(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                {
                    try
                    {
                        if (context.Request.QueryString.HasValue)
                        {

                            var token = context.Request.QueryString.Value
                                .Split('&')
                                .SingleOrDefault(x => x.Contains(AUTH_QUERY_STRING_KEY))?
                                .Split('=')
                                .Drop(1)
                                .First();

                            if (!string.IsNullOrWhiteSpace(token))
                            {
                                context.Request.Headers.Add("Authorization", new[] {$"Bearer {token}"});
                            }

                        }

                    }
                    catch
                    {
                        // if multiple headers it may throw an error.  Ignore both.
                    }
                }
                await next.Invoke();
            });

        }
    }
}
