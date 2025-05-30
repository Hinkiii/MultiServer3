// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using NetworkLibrary.Extension;
using System;
#if !NET5_0_OR_GREATER
using System.Collections.Generic;
#endif
using System.Diagnostics.CodeAnalysis;
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
using System.Net;
#endif
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SpaceWizards.HttpListener.WebSockets
{
    internal static partial class HttpWebSocket
    {
        internal const string SecWebSocketKeyGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        internal const string WebSocketUpgradeToken = "websocket";
        internal const int DefaultReceiveBufferSize = 16 * 1024;
        internal const int DefaultClientSendBufferSize = 16 * 1024;

        [SuppressMessage("Microsoft.Security", "CA5350", Justification = "SHA1 used only for hashing purposes, not for crypto.")]
        internal static string GetSecWebSocketAcceptString(string secWebSocketKey)
        {
            string acceptString = string.Concat(secWebSocketKey, HttpWebSocket.SecWebSocketKeyGuid);
            byte[] toHash = Encoding.UTF8.GetBytes(acceptString);

            // SHA1 used only for hashing purposes, not for crypto. Check here for FIPS compat.
            using (SHA1 sha1 = SHA1.Create())
            {
                return Convert.ToBase64String(sha1.ComputeHash(toHash));
            }
        }

        // return value here signifies if a Sec-WebSocket-Protocol header should be returned by the server.
        internal static bool ProcessWebSocketProtocolHeader(string clientSecWebSocketProtocol,
            string subProtocol,
            out string acceptProtocol)
        {
            acceptProtocol = string.Empty;
            if (string.IsNullOrEmpty(clientSecWebSocketProtocol))
            {
                // client hasn't specified any Sec-WebSocket-Protocol header
                if (subProtocol != null)
                {
                    // If the server specified _anything_ this isn't valid.
                    throw new WebSocketException(WebSocketError.UnsupportedProtocol,
                        SR.Format(SR.net_WebSockets_ClientAcceptingNoProtocols, subProtocol));
                }
                // Treat empty and null from the server as the same thing here, server should not send headers.
                return false;
            }

            // here, we know the client specified something and it's non-empty.

            if (subProtocol == null)
            {
                // client specified some protocols, server specified 'null'. So server should send headers.
                return true;
            }

            // here, we know that the client has specified something, it's not empty
            // and the server has specified exactly one protocol
#if NET5_0_OR_GREATER
            string[] requestProtocols = clientSecWebSocketProtocol.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
#elif NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            string[] rawRequestProtocols = clientSecWebSocketProtocol.Split(',', StringSplitOptions.RemoveEmptyEntries);
            string[] requestProtocols = new string[rawRequestProtocols.Length];

            for (int i = 0; i < rawRequestProtocols.Length; i++)
            {
                requestProtocols[i] = rawRequestProtocols[i].Trim();
            }
#else
            string[] rawRequestProtocols = clientSecWebSocketProtocol.Split(',');
            List<string> requestProtocolsList = new List<string>();
            foreach (string protocol in rawRequestProtocols)
            {
                string trimmedProtocol = protocol.Trim();
                if (!string.IsNullOrEmpty(trimmedProtocol))
                {
                    requestProtocolsList.Add(trimmedProtocol);
                }
            }
            string[] requestProtocols = requestProtocolsList.ToArray();
#endif
            acceptProtocol = subProtocol;

            // client specified protocols, serverOptions has exactly 1 non-empty entry. Check that
            // this exists in the list the client specified.
            for (int i = 0; i < requestProtocols.Length; i++)
            {
                string currentRequestProtocol = requestProtocols[i];
                if (string.Equals(acceptProtocol, currentRequestProtocol, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            throw new WebSocketException(WebSocketError.UnsupportedProtocol,
                SR.Format(SR.net_WebSockets_AcceptUnsupportedProtocol,
                    clientSecWebSocketProtocol,
                    subProtocol));
        }

        internal static void ValidateOptions(string subProtocol, int receiveBufferSize, int sendBufferSize, TimeSpan keepAliveInterval)
        {
            if (subProtocol != null)
            {
                WebSocketValidate.ValidateSubprotocol(subProtocol);
            }

            if (receiveBufferSize < MinReceiveBufferSize)
            {
                throw new ArgumentOutOfRangeException(nameof(receiveBufferSize), receiveBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooSmall, MinReceiveBufferSize));
            }

            if (sendBufferSize < MinSendBufferSize)
            {
                throw new ArgumentOutOfRangeException(nameof(sendBufferSize), sendBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooSmall, MinSendBufferSize));
            }

            if (receiveBufferSize > MaxBufferSize)
            {
                throw new ArgumentOutOfRangeException(nameof(receiveBufferSize), receiveBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooBig,
                        nameof(receiveBufferSize),
                        receiveBufferSize,
                        MaxBufferSize));
            }

            if (sendBufferSize > MaxBufferSize)
            {
                throw new ArgumentOutOfRangeException(nameof(sendBufferSize), sendBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooBig,
                        nameof(sendBufferSize),
                        sendBufferSize,
                        MaxBufferSize));
            }

            if (keepAliveInterval < Timeout.InfiniteTimeSpan) // -1 millisecond
            {
                throw new ArgumentOutOfRangeException(nameof(keepAliveInterval), keepAliveInterval,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooSmall, Timeout.InfiniteTimeSpan.ToString()));
            }
        }

        internal const int MinSendBufferSize = 16;
        internal const int MinReceiveBufferSize = 256;
        internal const int MaxBufferSize = 64 * 1024;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private static void ValidateWebSocketHeaders(HttpListenerContext context)
        {
            if (!WebSocketsSupported)
            {
                throw new PlatformNotSupportedException(SR.net_WebSockets_UnsupportedPlatform);
            }

            if (!context.Request.IsWebSocketRequest)
            {
                throw new WebSocketException(WebSocketError.NotAWebSocket,
                    SR.Format(SR.net_WebSockets_AcceptNotAWebSocket,
                    nameof(ValidateWebSocketHeaders),
                    HttpKnownHeaderNames.Connection,
                    HttpKnownHeaderNames.Upgrade,
                    WebSocketUpgradeToken,
                    context.Request.Headers[HttpKnownHeaderNames.Upgrade]));
            }

            string secWebSocketVersion = context.Request.Headers[HttpKnownHeaderNames.SecWebSocketVersion];
            if (string.IsNullOrEmpty(secWebSocketVersion))
            {
                throw new WebSocketException(WebSocketError.HeaderError,
                    SR.Format(SR.net_WebSockets_AcceptHeaderNotFound,
                    nameof(ValidateWebSocketHeaders),
                    HttpKnownHeaderNames.SecWebSocketVersion));
            }

            if (!string.Equals(secWebSocketVersion, SupportedVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new WebSocketException(WebSocketError.UnsupportedVersion,
                    SR.Format(SR.net_WebSockets_AcceptUnsupportedWebSocketVersion,
                    nameof(ValidateWebSocketHeaders),
                    secWebSocketVersion,
                    SupportedVersion));
            }

            string secWebSocketKey = context.Request.Headers[HttpKnownHeaderNames.SecWebSocketKey];
            bool isSecWebSocketKeyInvalid = string.IsNullOrWhiteSpace(secWebSocketKey);
            if (!isSecWebSocketKeyInvalid)
            {
                try
                {
                    // key must be 16 bytes then base64-encoded
                    isSecWebSocketKeyInvalid = secWebSocketKey!.IsBase64().Item2.Length != 16;
                }
                catch
                {
                    isSecWebSocketKeyInvalid = true;
                }
            }
            if (isSecWebSocketKeyInvalid)
            {
                throw new WebSocketException(WebSocketError.HeaderError,
                    SR.Format(SR.net_WebSockets_AcceptHeaderNotFound,
                    nameof(ValidateWebSocketHeaders),
                    HttpKnownHeaderNames.SecWebSocketKey));
            }
        }
#endif
    }
}
