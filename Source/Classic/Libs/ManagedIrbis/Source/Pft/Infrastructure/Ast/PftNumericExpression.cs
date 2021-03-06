﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PftNumericExpression.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;

using AM;

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
    public sealed class PftNumericExpression
        : PftNumeric
    {
        #region Properties

        /// <summary>
        /// Left operand.
        /// </summary>
        [CanBeNull]
        public PftNumeric LeftOperand { get; set; }

        /// <summary>
        /// Operation.
        /// </summary>
        [CanBeNull]
        public string Operation { get; set; }

        /// <summary>
        /// Right operand.
        /// </summary>
        [CanBeNull]
        public PftNumeric RightOperand { get; set; }

        /// <inheritdoc />
        public override IList<PftNode> Children
        {
            get
            {
                if (ReferenceEquals(_virtualChildren, null))
                {
                    _virtualChildren = new VirtualChildren();
                    List<PftNode> nodes = new List<PftNode>();
                    if (!ReferenceEquals(LeftOperand, null))
                    {
                        nodes.Add(LeftOperand);
                    }
                    if (!ReferenceEquals(RightOperand, null))
                    {
                        nodes.Add(RightOperand);
                    }
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
        public PftNumericExpression()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftNumericExpression
            (
                [NotNull] PftToken token
            )
            : base(token)
        {
            Code.NotNull(token, "token");
        }

        #endregion

        #region Private members

        private VirtualChildren _virtualChildren;

        #endregion

        #region Public methods

        /// <summary>
        /// Do the operation.
        /// </summary>
        public double DoOperation
            (
                [NotNull] PftContext context,
                double leftValue,
                [NotNull] string operation,
                double rightValue
            )
        {
            Code.NotNull(context, "context");
            Code.NotNullNorEmpty(operation, "operation");

            operation = operation.ToLowerInvariant();
            double result;
            switch (operation)
            {
                case "+":
                    result = leftValue + rightValue;
                    break;

                case "-":
                    result = leftValue - rightValue;
                    break;

                case "*":
                    result = leftValue * rightValue;
                    break;

                case "/":
                    result = leftValue / rightValue;
                    break;

                case "%":
                    // ReSharper disable once PossibleLossOfFraction
                    result = (int)leftValue % (int)rightValue;
                    break;

                case "div":
                    // ReSharper disable once PossibleLossOfFraction
                    result = (int)leftValue / (int)rightValue;
                    break;

                default:
                    throw new PftSyntaxException(this);
            }

            return result;
        }


        #endregion

        #region ICloneable members

        /// <inheritdoc />
        public override object Clone()
        {
            PftNumericExpression result = (PftNumericExpression) base.Clone();

            if (!ReferenceEquals(LeftOperand, null))
            {
                result.LeftOperand = (PftNumeric) LeftOperand.Clone();
            }

            if (!ReferenceEquals(RightOperand, null))
            {
                result.RightOperand = (PftNumeric) RightOperand.Clone();
            }

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

            if (ReferenceEquals(LeftOperand, null))
            {
                throw new PftSyntaxException(this);
            }
            if (string.IsNullOrEmpty(Operation))
            {
                throw new PftSyntaxException(this);
            }
            if (ReferenceEquals(RightOperand, null))
            {
                throw new PftSyntaxException(this);
            }

            PftContext clone = context.Push();
            clone.Evaluate(LeftOperand);
            double leftValue = LeftOperand.Value;
            clone.Evaluate(RightOperand);
            context.Pop();
            double rightValue = RightOperand.Value;
            Value = DoOperation
                (
                    context,
                    leftValue,
                    Operation.ThrowIfNull(),
                    rightValue
                );

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

            if (!ReferenceEquals(LeftOperand, null))
            {
                PftNodeInfo leftNode = new PftNodeInfo
                {
                    Node = LeftOperand,
                    Name = "Left"
                };
                result.Children.Add(leftNode);
                leftNode.Children.Add(LeftOperand.GetNodeInfo());
            }

            if (!string.IsNullOrEmpty(Operation))
            {
                PftNodeInfo operationNode = new PftNodeInfo
                {
                    Name = "Operation",
                    Value = Operation
                };
                result.Children.Add(operationNode);
            }

            if (!ReferenceEquals(RightOperand, null))
            {
                PftNodeInfo rightNode = new PftNodeInfo
                {
                    Node = RightOperand,
                    Name = "Right"
                };
                result.Children.Add(rightNode);
                rightNode.Children.Add(RightOperand.GetNodeInfo());
            }

            return result;
        }

        #endregion
    }
}
