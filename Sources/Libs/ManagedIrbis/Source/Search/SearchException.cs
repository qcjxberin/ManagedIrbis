﻿/* SearchException.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Search
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public class SearchException
        : IrbisException
    {
        #region Properties

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchException
            (
                string message
            )
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchException
            (
                string message,
                Exception innerException
            )
            : base
            (
                message,
                innerException
            )
        {
        }

        #endregion

        #region Public methods

        #endregion
    }
}