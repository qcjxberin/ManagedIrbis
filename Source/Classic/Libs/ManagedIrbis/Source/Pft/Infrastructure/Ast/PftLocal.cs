﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PftLocal.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM.Collections;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Pft.Infrastructure.Diagnostics;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Pft.Infrastructure.Ast
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class PftLocal
        : PftNode
    {
        #region Properties

        /// <summary>
        /// List of names.
        /// </summary>
        [NotNull]
        public NonNullCollection<string> Names { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftLocal()
        {
            Names = new NonNullCollection<string>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftLocal
            (
                [NotNull] PftToken token
            )
            : base(token)
        {
            Code.NotNull(token, "token");
            token.MustBe(PftTokenKind.Local);

            Names = new NonNullCollection<string>();
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        #endregion

        #region ICloneable members

        /// <inheritdoc />
        public override object Clone()
        {
            PftLocal result = (PftLocal) base.Clone();

            result.Names = new NonNullCollection<string>();
            result.Names.AddRange(Names);

            return result;
        }

        #endregion

        #region PftNode members

        /// <inheritdoc />
        public override void Execute
            (
                PftContext context
            )
        {
            OnBeforeExecution(context);

            PftContext localContext = context.Push();
            localContext.Output = context.Output;
            PftVariableManager localManager
                = new PftVariableManager(context.Variables);
            localContext.SetVariables(localManager);

            foreach (string name in Names)
            {
                PftVariable variable = new PftVariable
                    (
                        name,
                        false
                    );

                localManager.Registry.Add
                    (
                        name,
                        variable
                    );
            }

            localContext.Execute(Children);

            OnAfterExecution(context);
        }

        /// <inheritdoc/>
        public override PftNodeInfo GetNodeInfo()
        {
            PftNodeInfo result = new PftNodeInfo
            {
                Node = this,
                Name = "Local",
                Value = string.Join
                    (
                        ", ",
                        Names.ToArray()
                    )
            };

            foreach (PftNode node in Children)
            {
                result.Children.Add(node.GetNodeInfo());
            }

            return result;
        }

        #endregion
    }
}
