﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ConnectionStringParser.cs --
 * Ars Magna project, http://arsmagna.ru 
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace AM
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
#if !WINMOBILE && !PocketPC
    [DebuggerDisplay("KeyDelimiter={KeyDelimiter} ValueDelimiter={ValueDelimiter}")]
#endif
    public sealed class ConnectionStringParser
    {
        #region Constants

        /// <summary>
        /// Разделитель ключа по умолчанию
        /// </summary>
        public const string DefaultKeyDelimiter = "=";

        /// <summary>
        /// Разделитель значений по умолчанию
        /// </summary>
        public const string DefaultValueDelimiter = ";";

        #endregion

        #region Properties

        /// <summary>
        /// Разделитель ключа
        /// </summary>
        [NotNull]
        public NonNullValue<string> KeyDelimiter { get; set; }

        /// <summary>
        /// Разделитель значений
        /// </summary>
        [NotNull]
        public NonNullValue<string> ValueDelimiter { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectionStringParser()
        {
            KeyDelimiter = DefaultKeyDelimiter;
            ValueDelimiter = DefaultValueDelimiter;
        }

        #endregion

        #region Private members

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" />
        /// that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" />
        /// that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format
                (
                    "KeyDelimiter: {0}, ValueDelimiter: {1}",
                    KeyDelimiter,
                    ValueDelimiter
                );
        }

        #endregion
    }
}
