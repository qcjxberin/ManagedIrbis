﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MonitoringData.cs -- 
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

using AM;
using AM.Collections;
using AM.IO;
using AM.Runtime;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Monitoring
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class MonitoringData
    {
        #region Properties

        /// <summary>
        /// Moment of time.
        /// </summary>
        [JsonProperty("moment")]
        public DateTime Moment { get; set; }

        /// <summary>
        /// Number of running clients.
        /// </summary>
        [JsonProperty("clients")]
        public int Clients { get; set; }

        /// <summary>
        /// Command count.
        /// </summary>
        [JsonProperty("commands")]
        public int Commands { get; set; }

        /// <summary>
        /// Data for databases.
        /// </summary>
        [CanBeNull]
        [JsonProperty("databases")]
        public DatabaseData[] Databases { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        #endregion

        #region Object members

        #endregion
    }
}
