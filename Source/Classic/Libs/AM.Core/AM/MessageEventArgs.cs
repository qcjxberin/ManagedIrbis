﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MessageEventArgs.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace AM
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageEventArgs
        : EventArgs
    {
        #region Properties

        private string _message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            [DebuggerStepThrough]
            get
            {
                return _message;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageEventArgs(string message)
        {
            Code.NotNull(message, "message");
            _message = message;
        }

        #endregion
    }
}