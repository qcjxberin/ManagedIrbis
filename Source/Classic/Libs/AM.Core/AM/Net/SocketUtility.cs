﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SocketUtility.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace AM.Net
{
#if NOTDEF

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public static class SocketUtility
    {
        #region Private members

        #endregion

        #region Public methods

        ///// <summary>
        ///// Gets IP address from hostname.
        ///// </summary>
        ///// <returns>Resolved IP address of the host.</returns>
        //[NotNull]
        //public static IPAddress IPAddressFromHostname
        //    (
        //        [NotNull]string hostname
        //    )
        //{
        //    Code.NotNull(hostname, "hostname");

        //    if (hostname.OneOf("localhost","local", "(local)"))
        //    {
        //        return IPAddress.Loopback;
        //    }

        //    IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
        //    if (hostEntry.AddressList.Length == 0)
        //    {
        //        throw new SocketException();
        //    }

        //    return hostEntry.AddressList
        //            [
        //                new Random().Next(hostEntry.AddressList.Length)
        //            ];
        //}

        [NotNull]
        public static Task<Socket> AcceptAsync
            (
                [NotNull] this Socket socket
            )
        {
            var tcs = new TaskCompletionSource<Socket>(socket);

            socket.BeginAccept
                (
                    iar =>
                    {
                        var t = (TaskCompletionSource<Socket>) iar.AsyncState;
                        var s = (Socket) t.Task.AsyncState;
                        try
                        {
                            t.TrySetResult(s.EndAccept(iar));
                        }
                        catch (Exception ex)
                        {
                            t.TrySetException(ex);
                        }
                    },
                    tcs
                );

            return tcs.Task;
        }

        [NotNull]
        public static Task ConnectAsync
            (
                [NotNull] this Socket socket,
                [NotNull] IPAddress address,
                int port
            )
        {
            EndPoint endPoint = new IPEndPoint(address, port);
            return ConnectAsync(socket, endPoint);
        }

        [NotNull]
        public static Task ConnectAsync
            (
                [NotNull] this Socket socket,
                [NotNull] EndPoint endPoint
            )
        {
            var tcs = new TaskCompletionSource<bool>(socket);

            socket.BeginConnect
                (
                    endPoint,
                    iar =>
                    {
                        var t = (TaskCompletionSource<bool>)iar.AsyncState;
                        var s = (Socket)t.Task.AsyncState;
                        try
                        {
                            s.EndConnect(iar);
                            t.TrySetResult(true);
                        }
                        catch (Exception ex)
                        {
                            t.TrySetException(ex);
                        }
                    },
                    tcs
                );

            return tcs.Task;
        }

        [NotNull]
        public static Task DisconnectAsync
            (
                [NotNull] this Socket socket,
                bool reuseSocket
            )
        {
            var tcs = new TaskCompletionSource<bool>(socket);

            socket.BeginDisconnect
                (
                    reuseSocket,
                    iar =>
                    {
                        var t = (TaskCompletionSource<bool>) iar.AsyncState;
                        var s = (Socket) t.Task.AsyncState;
                        try
                        {
                            s.EndDisconnect(iar);
                            t.TrySetResult(true);
                        }
                        catch (Exception ex)
                        {
                            t.TrySetException(ex);
                        }
                    },
                    tcs
                );

            return tcs.Task;
        }

        [NotNull]
        public static Task DisconnectAsync
            (
                [NotNull] this Socket socket
            )
        {
            return DisconnectAsync(socket, false);
        }

        [NotNull]
        public static Task<int> ReceiveAsync
            (
                [NotNull] this Socket socket,
                [NotNull] byte[] buffer,
                int offset,
                int size,
                SocketFlags socketFlags
            )
        {
            var tcs = new TaskCompletionSource<int>(socket);

            socket.BeginReceive
                (
                    buffer,
                    offset,
                    size,
                    socketFlags,
                    iar =>
                    {
                        var t = (TaskCompletionSource<int>)iar.AsyncState;
                        var s = (Socket)t.Task.AsyncState;
                        try
                        {
                            t.TrySetResult(s.EndReceive(iar));
                        }
                        catch (Exception exc)
                        {
                            t.TrySetException(exc);
                        }
                    },
                    tcs
                );

            return tcs.Task;
        }

        [NotNull]
        public static Task<int> ReceiveAsync
            (
                [NotNull] this Socket socket,
                [NotNull] byte[] buffer
            )
        {
            return ReceiveAsync
                (
                    socket,
                    buffer,
                    0,
                    buffer.Length,
                    SocketFlags.None
                );
        }
            
        [NotNull]
        public static Task<int> SendAsync
            (
                [NotNull] this Socket socket,
                [NotNull] byte[] buffer,
                int offset,
                int size,
                SocketFlags socketFlags
            )
        {
            var tcs = new TaskCompletionSource<int>(socket);

            socket.BeginSend
                (
                    buffer,
                    offset,
                    size,
                    socketFlags,
                    iar =>
                    {
                        var t = (TaskCompletionSource<int>)iar.AsyncState;
                        var s = (Socket)t.Task.AsyncState;
                        try
                        {
                            t.TrySetResult(s.EndReceive(iar));
                        }
                        catch (Exception exc)
                        {
                            t.TrySetException(exc);
                        }
                    },
                    tcs
                );

            return tcs.Task;
        }

        [NotNull]
        public static Task<int> SendAsync
            (
                [NotNull] this Socket socket,
                [NotNull] byte[] buffer,
                int length
            )
        {
            return SendAsync
                (
                    socket,
                    buffer,
                    0,
                    length,
                    SocketFlags.None
                );
        }

        [NotNull]
        public static Task<int> SendAsync
            (
                [NotNull] this Socket socket,
                [NotNull] byte[] buffer
            )
        {
            return SendAsync
                (
                    socket,
                    buffer,
                    0,
                    buffer.Length,
                    SocketFlags.None
                );
        }

        #endregion
    }

#endif
}
