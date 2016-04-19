/* EventUtility.cs -- Useful routines for event manipulations
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Useful routines for event manipulations.
    /// </summary>
    [PublicAPI]
    public static class EventUtility
    {
        #region Public methods

        /// <summary>
        /// Raises the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <typeparam name="T">Type of event arguments</typeparam>
        public static void Raise <T>
            (
                [CanBeNull] EventHandler <T> handler,
                [CanBeNull] object sender,
                [CanBeNull] T args
            )
            where T : EventArgs
        {
            if ( handler != null )
            {
                handler ( sender, args );
            }
        }

        /// <summary>
        /// Raises the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <typeparam name="T">Type of event arguments.</typeparam>
        public static void Raise <T> 
            (
                [CanBeNull] EventHandler <T> handler,
                [CanBeNull] object sender
            )
            where T : EventArgs
        {
            if ( handler != null )
            {
                handler ( sender, null );
            }
        }

        /// <summary>
        /// Raises the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <typeparam name="T">Type of event arguments.</typeparam>
        public static void Raise <T> 
            (
                [CanBeNull] EventHandler <T> handler
            )
            where T : EventArgs
        {
            if ( handler != null )
            {
                handler ( null, null );
            }
        }

        #endregion
    }
}