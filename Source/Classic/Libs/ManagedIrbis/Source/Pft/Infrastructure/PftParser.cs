﻿/* PftParser.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.IO;
using AM.Text;

using CodeJam;

using JetBrains.Annotations;
using ManagedIrbis.Pft.Infrastructure.Ast;
using ManagedIrbis.Source.Pft.Infrastructure.Ast;
using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Pft.Infrastructure
{
    /// <summary>
    /// Parser for PFT language.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class PftParser
    {
        #region Properties

        /// <summary>
        /// Token list.
        /// </summary>
        [NotNull]
        public PftTokenList Tokens { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PftParser
            (
                [NotNull] PftTokenList tokens
            )
        {
            Code.NotNull(tokens, "tokens");

            Tokens = tokens;
        }

        #endregion

        #region Private members

        private PftNode ParseGroup()
        {
            PftGroup result = new PftGroup();

            PftToken token = Tokens.Current;

            token.MustBe(PftTokenKind.LeftParenthesis);
            Tokens.RequireNext();
            while (true)
            {
                token = Tokens.Current;

                if (token.Kind == PftTokenKind.RightParenthesis)
                {
                    Tokens.MoveNext();
                    break;
                }

                if (token.Kind == PftTokenKind.LeftParenthesis)
                {
                    throw new PftSyntaxException(token);
                }

                PftNode node = ParseSimple();
                result.Children.Add(node);
            }

            return result;
        }

        [NotNull]
        private PftNode ParseSimple()
        {
            PftNode result = null;
            bool moveNext = true;
            PftToken token = Tokens.Current;

            switch (token.Kind)
            {
                case PftTokenKind.Comma:
                    result = new PftComma();
                    break;

                case PftTokenKind.C:
                    result = new PftC(token);
                    break;

                case PftTokenKind.ConditionalLiteral:
                case PftTokenKind.V:
                case PftTokenKind.RepeatableLiteral:
                    result = ParseField();
                    moveNext = false;
                    break;

                case PftTokenKind.Hash:
                    result = new PftHash();
                    break;

                case PftTokenKind.Mfn:
                    result = new PftMfn(token);
                    break;

                case PftTokenKind.Mpl:
                    result = new PftMode(token);
                    break;

                case PftTokenKind.Percent:
                    result = new PftPercent();
                    break;

                case PftTokenKind.Slash:
                    result = new PftSlash();
                    break;

                case PftTokenKind.UnconditionalLiteral:
                    result = new PftUnconditionalLiteral(token);
                    break;

                case PftTokenKind.X:
                    result = new PftX(token);
                    break;

                case PftTokenKind.Unifor:
                    result = ParseUnifor(token);
                    break;

                default:
                    throw new PftSyntaxException(token);
            }

            if (ReferenceEquals(result, null))
            {
                throw new PftSyntaxException(token);
            }
            if (moveNext)
            {
                Tokens.MoveNext();
            }

            return result;
        }

        private PftNode ParseField()
        {
            List<PftNode> leftHand = new List<PftNode>();
            PftField result = new PftV();
            PftToken token;
            PftNode node;
            PftRepeatableLiteral literal;

            // Gather left hand of the field: conditional literal and friends
            while (!Tokens.IsEof)
            {
                node = null;
                token = Tokens.Current;
                switch (token.Kind)
                {
                    case PftTokenKind.ConditionalLiteral:
                        node = new PftConditionalLiteral(token);
                        break;

                    case PftTokenKind.C:
                        node = new PftC(token);
                        break;

                    case PftTokenKind.Comma:
                        node = new PftComma();
                        break;

                    case PftTokenKind.Comment:
                        node = new PftComment(token);
                        break;

                    case PftTokenKind.Hash:
                        node = new PftHash();
                        break;

                    case PftTokenKind.Mpl:
                        node = new PftMode(token);
                        break;

                    case PftTokenKind.Percent:
                        node = new PftPercent();
                        break;

                    case PftTokenKind.RepeatableLiteral:
                        // goto next step
                        break;

                    case PftTokenKind.Slash:
                        node = new PftSlash();
                        break;

                    case PftTokenKind.V:
                        // goto next step
                        break;

                    case PftTokenKind.X:
                        node = new PftX(token);
                        break;
                }

                if (node == null)
                {
                    break;
                }

                leftHand.Add(node);
                Tokens.MoveNext();
            } // Tokens.IsEof

            // Gather left hand of the field: repeatable literal
            if (!Tokens.IsEof)
            {
                token = Tokens.Current;
                if (token.Kind == PftTokenKind.RepeatableLiteral)
                {
                    literal = new PftRepeatableLiteral(token)
                    {
                        IsPrefix = true
                    };
                    leftHand.Add(literal);

                    if (Tokens.Peek() == PftTokenKind.Plus)
                    {
                        literal.Plus = true;
                        Tokens.MoveNext();
                    }

                    Tokens.MoveNext();
                }
            } // Tokens.IsEof

            // Orphaned left hand?
            if (Tokens.IsEof)
            {
                result = new PftOrphan();
                result.LeftHand.AddRange(leftHand);
                goto DONE;
            }

            // Parse field itself
            if (!Tokens.IsEof)
            {
                token = Tokens.Current;

                // Orphaned?
                if (token.Kind != PftTokenKind.V)
                {
                    result = new PftOrphan();
                    result.LeftHand.AddRange(leftHand);
                    goto DONE;
                }
                if (string.IsNullOrEmpty(token.Text))
                {
                    throw new PftSyntaxException(token);
                }

                FieldSpecification specification = (FieldSpecification)token.UserData;
                if (specification == null)
                {
                    throw new PftSyntaxException(token);
                }

                // Check for command code
                switch (specification.Command)
                {
                    case 'v':
                    case 'V':
                        // Already V
                        break;

                    case 'd':
                    case 'D':
                        result = new PftD();
                        break;

                    case 'n':
                    case 'N':
                        result = new PftN();
                        break;

                    case 'g':
                    case 'G':
                        result = new PftG();
                        break;

                    default:
                        throw new PftSyntaxException(token);
                }

                result.LeftHand.AddRange(leftHand);
                result.Apply(specification);
                Tokens.MoveNext();
            } // Tokens.IsEof

            // Gather right hand (for V command only)
            if (result is PftV)
            {
                if (!Tokens.IsEof)
                {
                    bool plus = false;
                    token = Tokens.Current;
                    if (token.Kind == PftTokenKind.Plus)
                    {
                        plus = true;
                        Tokens.RequireNext();
                        token = Tokens.Current;
                    }
                    if (token.Kind == PftTokenKind.RepeatableLiteral)
                    {
                        literal = new PftRepeatableLiteral(token)
                        {
                            Plus = plus
                        };
                        result.RightHand.Add(literal);
                        Tokens.MoveNext();
                    }
                    else
                    {
                        if (plus)
                        {
                            throw new PftSyntaxException(token);
                        }
                    }
                } // Tokens.IsEof

                if (!Tokens.IsEof)
                {
                    token = Tokens.Current;
                    if (token.Kind == PftTokenKind.ConditionalLiteral)
                    {
                        node = new PftConditionalLiteral(token);
                        result.RightHand.Add(node);
                        Tokens.MoveNext();
                    }
                }
            } // result is PftV

            DONE: return result;
        }

        private PftUnifor ParseUnifor
            (
                PftToken token
            )
        {
            PftUnifor result = new PftUnifor(token);

            Tokens.RequireNext();
            token = Tokens.Current;
            token.MustBe(PftTokenKind.LeftParenthesis);
            Tokens.RequireNext();

            while (true)
            {
                token = Tokens.Current;

                if (token.Kind == PftTokenKind.RightParenthesis)
                {
                    Tokens.MoveNext();
                    break;
                }

                if (token.Kind == PftTokenKind.LeftParenthesis)
                {
                    throw new PftSyntaxException(token);
                }

                PftNode node = ParseSimple();
                result.Children.Add(node);
            }

            return result;
        }

        private PftNode ParseComposite()
        {
            if (Tokens.Current.Kind == PftTokenKind.LeftParenthesis)
            {
                return ParseGroup();
            }

            return ParseSimple();
        }

        private PftProgram ParseProgram()
        {
            PftProgram result = new PftProgram();

            while (!Tokens.IsEof)
            {
                PftNode node = ParseComposite();
                result.Children.Add(node);
            }

            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Parse the tokens.
        /// </summary>
        [NotNull]
        public PftProgram Parse()
        {
            PftProgram result = ParseProgram();

            return result;
        }

        #endregion

        #region Object members

        #endregion
    }
}