﻿/* NetworkDebugger.cs -- debugger for network protocol
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Network
{
    /// <summary>
    /// Debugger for network protocol.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public static class NetworkDebugger
    {
        #region Properties

        /// <summary>
        /// Active?
        /// </summary>
        public static bool Active { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Break point.
        /// </summary>
        public static void Break
            (
                [CanBeNull] byte[] sendPacket,
                [CanBeNull] byte[] receivedPacket
            )
        {
            if (!Active)
            {
                return;
            }

            Debugger.Break();
        }

        /// <summary>
        /// Dump server response.
        /// </summary>
        public static void DumpResponse
            (
                [NotNull] ServerResponse response
            )
        {
            Code.NotNull(response, "response");

            string filePath = Path.Combine
                (
                    Path.GetTempPath(),
                    "response.packet"
                );
            File.WriteAllBytes
                (
                    filePath,
                    response.RawAnswer
                );
        }

        /// <summary>
        /// Write line.
        /// </summary>
        public static void Log
            (
                string text
            )
        {
#if NETCORE

            Debug.WriteLine(text);

#else

            Debugger.Log
                (
                    0,
                    "IRBIS",
                    text
                );

#endif
        }

        /// <summary>
        /// Write line.
        /// </summary>
        public static void Log
            (
                string[] lines
            )
        {
            foreach (string line in lines)
            {
                Log(line);
            }
        }

#endregion
    }
}
