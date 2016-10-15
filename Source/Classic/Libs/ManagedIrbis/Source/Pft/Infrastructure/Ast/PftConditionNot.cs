﻿/* PftConditionNot.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Pft.Infrastructure.Ast
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class PftConditionNot
        : PftCondition
    {
        #region Properties

        /// <summary>
        /// Inner condition
        /// </summary>
        [CanBeNull]
        public PftCondition InnerCondition { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftConditionNot()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftConditionNot
            (
                [NotNull] PftToken token
            )
            : base(token)
        {
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        #endregion

        #region PftNode members

        /// <inheritdoc />
        public override void Execute
            (
                PftContext context
            )
        {
            OnBeforeExecution(context);

            if (ReferenceEquals(InnerCondition, null))
            {
                throw new PftSyntaxException();
            }

            InnerCondition.Execute(context);
            Value = !InnerCondition.Value;

            OnAfterExecution(context);
        }

        #endregion
    }
}