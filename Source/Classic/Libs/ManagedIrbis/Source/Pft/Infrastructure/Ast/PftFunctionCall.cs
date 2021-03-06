﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PftFunctionCall.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Linq;
using AM;
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
    public sealed class PftFunctionCall
        : PftNode
    {
        #region Properties

        /// <summary>
        /// Function name.
        /// </summary>
        [CanBeNull]
        public string Name { get; set; }

        /// <summary>
        /// Array of arguments.
        /// </summary>
        [NotNull]
        public NonNullCollection<PftNode> Arguments { get; private set; }

        /// <inheritdoc/>
        public override bool ExtendedSyntax
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override IList<PftNode> Children
        {
            get
            {
                if (ReferenceEquals(_virtualChildren, null))
                {

                    _virtualChildren = new VirtualChildren();
                    List<PftNode> nodes = new List<PftNode>();
                    nodes.AddRange(Arguments);
                    _virtualChildren.SetChildren(nodes);
                }

                return _virtualChildren;
            }
            protected set
            {
                // Nothing to do here
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftFunctionCall()
        {
            Arguments = new NonNullCollection<PftNode>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftFunctionCall
            (
                [NotNull] string name
            )
        {
            Code.NotNullNorEmpty(name, "name");

            Name = name;
            Arguments = new NonNullCollection<PftNode>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftFunctionCall
            (
                [NotNull] PftToken token
            )
            : base(token)
        {
            Code.NotNull(token, "token");
            token.MustBe(PftTokenKind.Identifier);

            Name = token.Text;
            Arguments = new NonNullCollection<PftNode>();
        }

        #endregion

        #region Private members

        private VirtualChildren _virtualChildren;

        #endregion

        #region Public methods

        #endregion

        #region ICloneable members

        /// <inheritdoc />
        public override object Clone()
        {
            PftFunctionCall result = (PftFunctionCall) base.Clone();

            result._virtualChildren = null;

            result.Arguments = Arguments.CloneNodes().ThrowIfNull();

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

            string name = Name;
            if (string.IsNullOrEmpty(name))
            {
                throw new PftSyntaxException(this);
            }

            PftNode[] arguments = Arguments.ToArray();

            FunctionDescriptor descriptor = context.Functions.FindFunction(name);
            if (!ReferenceEquals(descriptor, null))
            {
                descriptor.Function
                    (
                        context,
                        this,
                        arguments
                    );
            }
            else
            {
                PftProcedure procedure = context.Procedures
                    .FindProcedure(name);

                if (!ReferenceEquals(procedure, null))
                {
                    string expression = context.GetStringArgument(arguments, 0);
                    procedure.Execute
                        (
                            context,
                            expression
                        );
                }
                else
                {
                    PftFunctionManager.ExecuteFunction
                    (
                        name,
                        context,
                        this,
                        arguments
                    );
                }
            }

            OnAfterExecution(context);
        }

        /// <inheritdoc/>
        public override PftNodeInfo GetNodeInfo()
        {
            PftNodeInfo result = new PftNodeInfo
            {
                Node = this,
                Name = SimplifyTypeName(GetType().Name)
            };

            PftNodeInfo name = new PftNodeInfo
            {
                Name = "Name",
                Value = Name
            };
            result.Children.Add(name);

            PftNodeInfo arguments = new PftNodeInfo
            {
                Name = "Arguments"
            };
            arguments.Children.AddRange
                (
                    Arguments.Select(node => node.GetNodeInfo())
                );
            result.Children.Add(arguments);

            return result;
        }

        #endregion
    }
}
