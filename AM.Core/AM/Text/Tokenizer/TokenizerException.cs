﻿/* TokenizerException.cs -- exception for StringTokenizer
 * Ars Magna project, http://arsmagna.ru 
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace AM.Text.Tokenizer
{
    /// <summary>
    /// Exception class for <see cref="StringTokenizer"/>.
    /// </summary>
    [PublicAPI]
    public sealed class TokenizerException
        : ApplicationException
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public TokenizerException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TokenizerException
            (
                [CanBeNull] string message
            ) 
            : base(message)
        {
        }

        public TokenizerException
            (
                [CanBeNull] string message, 
                [CanBeNull] Exception innerException
            ) 
            : base
            (
                message, 
                innerException
            )
        {
        }

        #endregion
    }
}