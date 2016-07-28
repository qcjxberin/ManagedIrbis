#if FW40

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from IrbisSearchQuery.g4 by ANTLR 4.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591

// Ambiguous reference in cref attribute
#pragma warning disable 0419

// XML comment has cref attribute that could not be resolved
#pragma warning disable 1574

namespace ManagedClient.Search {
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.3")]
[System.CLSCompliant(false)]
public partial class IrbisSearchQueryParser : Parser {
	public const int
		ENTRY=1, QUOTED=2, NONQUOTED=3, REFERENCE=4, TAGNUMBER=5, PLUS=6, STAR=7, 
		HAT=8, G=9, F=10, DOT=11, LPAREN=12, RPAREN=13, SLASH=14, COMMA=15, WS=16;
	public static readonly string[] tokenNames = {
		"<INVALID>", "ENTRY", "QUOTED", "NONQUOTED", "REFERENCE", "TAGNUMBER", 
		"'+'", "'*'", "'^'", "'(G)'", "'(F)'", "'.'", "'('", "')'", "'/'", "','", 
		"WS"
	};
	public const int
		RULE_program = 0, RULE_levelThree = 1, RULE_levelTwo = 2, RULE_levelOne = 3;
	public static readonly string[] ruleNames = {
		"program", "levelThree", "levelTwo", "levelOne"
	};

	public override string GrammarFileName { get { return "IrbisSearchQuery.g4"; } }

	public override string[] TokenNames { get { return tokenNames; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return _serializedATN; } }

	public IrbisSearchQueryParser(ITokenStream input)
		: base(input)
	{
		_interp = new ParserATNSimulator(this,_ATN);
	}
	public partial class ProgramContext : ParserRuleContext {
		public ITerminalNode Eof() { return GetToken(IrbisSearchQueryParser.Eof, 0); }
		public LevelThreeContext levelThree() {
			return GetRuleContext<LevelThreeContext>(0);
		}
		public ProgramContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_program; } }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterProgram(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitProgram(this);
		}
	}

	[RuleVersion(0)]
	public ProgramContext program() {
		ProgramContext _localctx = new ProgramContext(_ctx, State);
		EnterRule(_localctx, 0, RULE_program);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 8; levelThree(0);
			State = 9; Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.ReportError(this, re);
			_errHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LevelThreeContext : ParserRuleContext {
		public LevelThreeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_levelThree; } }
	 
		public LevelThreeContext() { }
		public virtual void CopyFrom(LevelThreeContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class StarOperator3Context : LevelThreeContext {
		public LevelThreeContext left;
		public IToken op;
		public LevelThreeContext right;
		public LevelThreeContext[] levelThree() {
			return GetRuleContexts<LevelThreeContext>();
		}
		public ITerminalNode STAR() { return GetToken(IrbisSearchQueryParser.STAR, 0); }
		public ITerminalNode HAT() { return GetToken(IrbisSearchQueryParser.HAT, 0); }
		public LevelThreeContext levelThree(int i) {
			return GetRuleContext<LevelThreeContext>(i);
		}
		public StarOperator3Context(LevelThreeContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterStarOperator3(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitStarOperator3(this);
		}
	}
	public partial class PlusOperator3Context : LevelThreeContext {
		public LevelThreeContext left;
		public LevelThreeContext right;
		public LevelThreeContext[] levelThree() {
			return GetRuleContexts<LevelThreeContext>();
		}
		public ITerminalNode PLUS() { return GetToken(IrbisSearchQueryParser.PLUS, 0); }
		public LevelThreeContext levelThree(int i) {
			return GetRuleContext<LevelThreeContext>(i);
		}
		public PlusOperator3Context(LevelThreeContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterPlusOperator3(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitPlusOperator3(this);
		}
	}
	public partial class LevelTwoOuterContext : LevelThreeContext {
		public LevelTwoContext levelTwo() {
			return GetRuleContext<LevelTwoContext>(0);
		}
		public LevelTwoOuterContext(LevelThreeContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterLevelTwoOuter(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitLevelTwoOuter(this);
		}
	}
	public partial class ReferenceContext : LevelThreeContext {
		public ITerminalNode REFERENCE() { return GetToken(IrbisSearchQueryParser.REFERENCE, 0); }
		public ReferenceContext(LevelThreeContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterReference(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitReference(this);
		}
	}

	[RuleVersion(0)]
	public LevelThreeContext levelThree() {
		return levelThree(0);
	}

	private LevelThreeContext levelThree(int _p) {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = State;
		LevelThreeContext _localctx = new LevelThreeContext(_ctx, _parentState);
		LevelThreeContext _prevctx = _localctx;
		int _startState = 2;
		EnterRecursionRule(_localctx, 2, RULE_levelThree, _p);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 14;
			switch (_input.La(1)) {
			case ENTRY:
			case LPAREN:
				{
				_localctx = new LevelTwoOuterContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				State = 12; levelTwo(0);
				}
				break;
			case REFERENCE:
				{
				_localctx = new ReferenceContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				State = 13; Match(REFERENCE);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.Lt(-1);
			State = 24;
			_errHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(_input,2,_ctx);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.InvalidAltNumber ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					State = 22;
					switch ( Interpreter.AdaptivePredict(_input,1,_ctx) ) {
					case 1:
						{
						_localctx = new StarOperator3Context(new LevelThreeContext(_parentctx, _parentState));
						((StarOperator3Context)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelThree);
						State = 16;
						if (!(Precpred(_ctx, 2))) throw new FailedPredicateException(this, "Precpred(_ctx, 2)");
						State = 17;
						((StarOperator3Context)_localctx).op = _input.Lt(1);
						_la = _input.La(1);
						if ( !(_la==STAR || _la==HAT) ) {
							((StarOperator3Context)_localctx).op = _errHandler.RecoverInline(this);
						}
						Consume();
						State = 18; ((StarOperator3Context)_localctx).right = levelThree(3);
						}
						break;

					case 2:
						{
						_localctx = new PlusOperator3Context(new LevelThreeContext(_parentctx, _parentState));
						((PlusOperator3Context)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelThree);
						State = 19;
						if (!(Precpred(_ctx, 1))) throw new FailedPredicateException(this, "Precpred(_ctx, 1)");
						State = 20; Match(PLUS);
						State = 21; ((PlusOperator3Context)_localctx).right = levelThree(2);
						}
						break;
					}
					} 
				}
				State = 26;
				_errHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(_input,2,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.ReportError(this, re);
			_errHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public partial class LevelTwoContext : ParserRuleContext {
		public LevelTwoContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_levelTwo; } }
	 
		public LevelTwoContext() { }
		public virtual void CopyFrom(LevelTwoContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class StarOperator2Context : LevelTwoContext {
		public LevelTwoContext left;
		public IToken op;
		public LevelTwoContext right;
		public LevelTwoContext levelTwo(int i) {
			return GetRuleContext<LevelTwoContext>(i);
		}
		public ITerminalNode STAR() { return GetToken(IrbisSearchQueryParser.STAR, 0); }
		public ITerminalNode HAT() { return GetToken(IrbisSearchQueryParser.HAT, 0); }
		public LevelTwoContext[] levelTwo() {
			return GetRuleContexts<LevelTwoContext>();
		}
		public StarOperator2Context(LevelTwoContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterStarOperator2(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitStarOperator2(this);
		}
	}
	public partial class PlusOperator2Context : LevelTwoContext {
		public LevelTwoContext left;
		public LevelTwoContext right;
		public LevelTwoContext levelTwo(int i) {
			return GetRuleContext<LevelTwoContext>(i);
		}
		public ITerminalNode PLUS() { return GetToken(IrbisSearchQueryParser.PLUS, 0); }
		public LevelTwoContext[] levelTwo() {
			return GetRuleContexts<LevelTwoContext>();
		}
		public PlusOperator2Context(LevelTwoContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterPlusOperator2(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitPlusOperator2(this);
		}
	}
	public partial class ParenOuterContext : LevelTwoContext {
		public ITerminalNode RPAREN() { return GetToken(IrbisSearchQueryParser.RPAREN, 0); }
		public ITerminalNode LPAREN() { return GetToken(IrbisSearchQueryParser.LPAREN, 0); }
		public LevelTwoContext levelTwo() {
			return GetRuleContext<LevelTwoContext>(0);
		}
		public ParenOuterContext(LevelTwoContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterParenOuter(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitParenOuter(this);
		}
	}
	public partial class LevelOneOuterContext : LevelTwoContext {
		public LevelOneContext levelOne() {
			return GetRuleContext<LevelOneContext>(0);
		}
		public LevelOneOuterContext(LevelTwoContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterLevelOneOuter(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitLevelOneOuter(this);
		}
	}

	[RuleVersion(0)]
	public LevelTwoContext levelTwo() {
		return levelTwo(0);
	}

	private LevelTwoContext levelTwo(int _p) {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = State;
		LevelTwoContext _localctx = new LevelTwoContext(_ctx, _parentState);
		LevelTwoContext _prevctx = _localctx;
		int _startState = 4;
		EnterRecursionRule(_localctx, 4, RULE_levelTwo, _p);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 33;
			switch (_input.La(1)) {
			case ENTRY:
				{
				_localctx = new LevelOneOuterContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				State = 28; levelOne(0);
				}
				break;
			case LPAREN:
				{
				_localctx = new ParenOuterContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				State = 29; Match(LPAREN);
				State = 30; levelTwo(0);
				State = 31; Match(RPAREN);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.Lt(-1);
			State = 43;
			_errHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(_input,5,_ctx);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.InvalidAltNumber ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					State = 41;
					switch ( Interpreter.AdaptivePredict(_input,4,_ctx) ) {
					case 1:
						{
						_localctx = new StarOperator2Context(new LevelTwoContext(_parentctx, _parentState));
						((StarOperator2Context)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelTwo);
						State = 35;
						if (!(Precpred(_ctx, 2))) throw new FailedPredicateException(this, "Precpred(_ctx, 2)");
						State = 36;
						((StarOperator2Context)_localctx).op = _input.Lt(1);
						_la = _input.La(1);
						if ( !(_la==STAR || _la==HAT) ) {
							((StarOperator2Context)_localctx).op = _errHandler.RecoverInline(this);
						}
						Consume();
						State = 37; ((StarOperator2Context)_localctx).right = levelTwo(3);
						}
						break;

					case 2:
						{
						_localctx = new PlusOperator2Context(new LevelTwoContext(_parentctx, _parentState));
						((PlusOperator2Context)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelTwo);
						State = 38;
						if (!(Precpred(_ctx, 1))) throw new FailedPredicateException(this, "Precpred(_ctx, 1)");
						State = 39; Match(PLUS);
						State = 40; ((PlusOperator2Context)_localctx).right = levelTwo(2);
						}
						break;
					}
					} 
				}
				State = 45;
				_errHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(_input,5,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.ReportError(this, re);
			_errHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public partial class LevelOneContext : ParserRuleContext {
		public LevelOneContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_levelOne; } }
	 
		public LevelOneContext() { }
		public virtual void CopyFrom(LevelOneContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class StarOperator1Context : LevelOneContext {
		public LevelOneContext left;
		public IToken op;
		public LevelOneContext right;
		public LevelOneContext levelOne(int i) {
			return GetRuleContext<LevelOneContext>(i);
		}
		public ITerminalNode STAR() { return GetToken(IrbisSearchQueryParser.STAR, 0); }
		public LevelOneContext[] levelOne() {
			return GetRuleContexts<LevelOneContext>();
		}
		public ITerminalNode HAT() { return GetToken(IrbisSearchQueryParser.HAT, 0); }
		public StarOperator1Context(LevelOneContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterStarOperator1(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitStarOperator1(this);
		}
	}
	public partial class EntryContext : LevelOneContext {
		public ITerminalNode ENTRY() { return GetToken(IrbisSearchQueryParser.ENTRY, 0); }
		public EntryContext(LevelOneContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterEntry(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitEntry(this);
		}
	}
	public partial class FOperatorContext : LevelOneContext {
		public LevelOneContext left;
		public LevelOneContext right;
		public ITerminalNode F() { return GetToken(IrbisSearchQueryParser.F, 0); }
		public LevelOneContext levelOne(int i) {
			return GetRuleContext<LevelOneContext>(i);
		}
		public LevelOneContext[] levelOne() {
			return GetRuleContexts<LevelOneContext>();
		}
		public FOperatorContext(LevelOneContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterFOperator(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitFOperator(this);
		}
	}
	public partial class DotOperatorContext : LevelOneContext {
		public LevelOneContext left;
		public LevelOneContext right;
		public LevelOneContext levelOne(int i) {
			return GetRuleContext<LevelOneContext>(i);
		}
		public ITerminalNode DOT() { return GetToken(IrbisSearchQueryParser.DOT, 0); }
		public LevelOneContext[] levelOne() {
			return GetRuleContexts<LevelOneContext>();
		}
		public DotOperatorContext(LevelOneContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterDotOperator(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitDotOperator(this);
		}
	}
	public partial class PlusOperator1Context : LevelOneContext {
		public LevelOneContext left;
		public LevelOneContext right;
		public LevelOneContext levelOne(int i) {
			return GetRuleContext<LevelOneContext>(i);
		}
		public ITerminalNode PLUS() { return GetToken(IrbisSearchQueryParser.PLUS, 0); }
		public LevelOneContext[] levelOne() {
			return GetRuleContexts<LevelOneContext>();
		}
		public PlusOperator1Context(LevelOneContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterPlusOperator1(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitPlusOperator1(this);
		}
	}
	public partial class GOperatorContext : LevelOneContext {
		public LevelOneContext left;
		public LevelOneContext right;
		public ITerminalNode G() { return GetToken(IrbisSearchQueryParser.G, 0); }
		public LevelOneContext levelOne(int i) {
			return GetRuleContext<LevelOneContext>(i);
		}
		public LevelOneContext[] levelOne() {
			return GetRuleContexts<LevelOneContext>();
		}
		public GOperatorContext(LevelOneContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.EnterGOperator(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IIrbisSearchQueryListener typedListener = listener as IIrbisSearchQueryListener;
			if (typedListener != null) typedListener.ExitGOperator(this);
		}
	}

	[RuleVersion(0)]
	public LevelOneContext levelOne() {
		return levelOne(0);
	}

	private LevelOneContext levelOne(int _p) {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = State;
		LevelOneContext _localctx = new LevelOneContext(_ctx, _parentState);
		LevelOneContext _prevctx = _localctx;
		int _startState = 6;
		EnterRecursionRule(_localctx, 6, RULE_levelOne, _p);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			{
			_localctx = new EntryContext(_localctx);
			_ctx = _localctx;
			_prevctx = _localctx;

			State = 47; Match(ENTRY);
			}
			_ctx.stop = _input.Lt(-1);
			State = 66;
			_errHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(_input,7,_ctx);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.InvalidAltNumber ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					State = 64;
					switch ( Interpreter.AdaptivePredict(_input,6,_ctx) ) {
					case 1:
						{
						_localctx = new DotOperatorContext(new LevelOneContext(_parentctx, _parentState));
						((DotOperatorContext)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelOne);
						State = 49;
						if (!(Precpred(_ctx, 5))) throw new FailedPredicateException(this, "Precpred(_ctx, 5)");
						State = 50; Match(DOT);
						State = 51; ((DotOperatorContext)_localctx).right = levelOne(6);
						}
						break;

					case 2:
						{
						_localctx = new FOperatorContext(new LevelOneContext(_parentctx, _parentState));
						((FOperatorContext)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelOne);
						State = 52;
						if (!(Precpred(_ctx, 4))) throw new FailedPredicateException(this, "Precpred(_ctx, 4)");
						State = 53; Match(F);
						State = 54; ((FOperatorContext)_localctx).right = levelOne(5);
						}
						break;

					case 3:
						{
						_localctx = new GOperatorContext(new LevelOneContext(_parentctx, _parentState));
						((GOperatorContext)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelOne);
						State = 55;
						if (!(Precpred(_ctx, 3))) throw new FailedPredicateException(this, "Precpred(_ctx, 3)");
						State = 56; Match(G);
						State = 57; ((GOperatorContext)_localctx).right = levelOne(4);
						}
						break;

					case 4:
						{
						_localctx = new StarOperator1Context(new LevelOneContext(_parentctx, _parentState));
						((StarOperator1Context)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelOne);
						State = 58;
						if (!(Precpred(_ctx, 2))) throw new FailedPredicateException(this, "Precpred(_ctx, 2)");
						State = 59;
						((StarOperator1Context)_localctx).op = _input.Lt(1);
						_la = _input.La(1);
						if ( !(_la==STAR || _la==HAT) ) {
							((StarOperator1Context)_localctx).op = _errHandler.RecoverInline(this);
						}
						Consume();
						State = 60; ((StarOperator1Context)_localctx).right = levelOne(3);
						}
						break;

					case 5:
						{
						_localctx = new PlusOperator1Context(new LevelOneContext(_parentctx, _parentState));
						((PlusOperator1Context)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_levelOne);
						State = 61;
						if (!(Precpred(_ctx, 1))) throw new FailedPredicateException(this, "Precpred(_ctx, 1)");
						State = 62; Match(PLUS);
						State = 63; ((PlusOperator1Context)_localctx).right = levelOne(2);
						}
						break;
					}
					} 
				}
				State = 68;
				_errHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(_input,7,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.ReportError(this, re);
			_errHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 1: return levelThree_sempred((LevelThreeContext)_localctx, predIndex);

		case 2: return levelTwo_sempred((LevelTwoContext)_localctx, predIndex);

		case 3: return levelOne_sempred((LevelOneContext)_localctx, predIndex);
		}
		return true;
	}
	private bool levelOne_sempred(LevelOneContext _localctx, int predIndex) {
		switch (predIndex) {
		case 4: return Precpred(_ctx, 5);

		case 5: return Precpred(_ctx, 4);

		case 6: return Precpred(_ctx, 3);

		case 7: return Precpred(_ctx, 2);

		case 8: return Precpred(_ctx, 1);
		}
		return true;
	}
	private bool levelTwo_sempred(LevelTwoContext _localctx, int predIndex) {
		switch (predIndex) {
		case 2: return Precpred(_ctx, 2);

		case 3: return Precpred(_ctx, 1);
		}
		return true;
	}
	private bool levelThree_sempred(LevelThreeContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return Precpred(_ctx, 2);

		case 1: return Precpred(_ctx, 1);
		}
		return true;
	}

	public static readonly string _serializedATN =
		"\x3\xAF6F\x8320\x479D\xB75C\x4880\x1605\x191C\xAB37\x3\x12H\x4\x2\t\x2"+
		"\x4\x3\t\x3\x4\x4\t\x4\x4\x5\t\x5\x3\x2\x3\x2\x3\x2\x3\x3\x3\x3\x3\x3"+
		"\x5\x3\x11\n\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\a\x3\x19\n\x3\f\x3"+
		"\xE\x3\x1C\v\x3\x3\x4\x3\x4\x3\x4\x3\x4\x3\x4\x3\x4\x5\x4$\n\x4\x3\x4"+
		"\x3\x4\x3\x4\x3\x4\x3\x4\x3\x4\a\x4,\n\x4\f\x4\xE\x4/\v\x4\x3\x5\x3\x5"+
		"\x3\x5\x3\x5\x3\x5\x3\x5\x3\x5\x3\x5\x3\x5\x3\x5\x3\x5\x3\x5\x3\x5\x3"+
		"\x5\x3\x5\x3\x5\x3\x5\x3\x5\a\x5\x43\n\x5\f\x5\xE\x5\x46\v\x5\x3\x5\x2"+
		"\x2\x5\x4\x6\b\x6\x2\x2\x4\x2\x6\x2\b\x2\x2\x3\x3\x2\t\nN\x2\n\x3\x2\x2"+
		"\x2\x4\x10\x3\x2\x2\x2\x6#\x3\x2\x2\x2\b\x30\x3\x2\x2\x2\n\v\x5\x4\x3"+
		"\x2\v\f\a\x2\x2\x3\f\x3\x3\x2\x2\x2\r\xE\b\x3\x1\x2\xE\x11\x5\x6\x4\x2"+
		"\xF\x11\a\x6\x2\x2\x10\r\x3\x2\x2\x2\x10\xF\x3\x2\x2\x2\x11\x1A\x3\x2"+
		"\x2\x2\x12\x13\f\x4\x2\x2\x13\x14\t\x2\x2\x2\x14\x19\x5\x4\x3\x5\x15\x16"+
		"\f\x3\x2\x2\x16\x17\a\b\x2\x2\x17\x19\x5\x4\x3\x4\x18\x12\x3\x2\x2\x2"+
		"\x18\x15\x3\x2\x2\x2\x19\x1C\x3\x2\x2\x2\x1A\x18\x3\x2\x2\x2\x1A\x1B\x3"+
		"\x2\x2\x2\x1B\x5\x3\x2\x2\x2\x1C\x1A\x3\x2\x2\x2\x1D\x1E\b\x4\x1\x2\x1E"+
		"$\x5\b\x5\x2\x1F \a\xE\x2\x2 !\x5\x6\x4\x2!\"\a\xF\x2\x2\"$\x3\x2\x2\x2"+
		"#\x1D\x3\x2\x2\x2#\x1F\x3\x2\x2\x2$-\x3\x2\x2\x2%&\f\x4\x2\x2&\'\t\x2"+
		"\x2\x2\',\x5\x6\x4\x5()\f\x3\x2\x2)*\a\b\x2\x2*,\x5\x6\x4\x4+%\x3\x2\x2"+
		"\x2+(\x3\x2\x2\x2,/\x3\x2\x2\x2-+\x3\x2\x2\x2-.\x3\x2\x2\x2.\a\x3\x2\x2"+
		"\x2/-\x3\x2\x2\x2\x30\x31\b\x5\x1\x2\x31\x32\a\x3\x2\x2\x32\x44\x3\x2"+
		"\x2\x2\x33\x34\f\a\x2\x2\x34\x35\a\r\x2\x2\x35\x43\x5\b\x5\b\x36\x37\f"+
		"\x6\x2\x2\x37\x38\a\f\x2\x2\x38\x43\x5\b\x5\a\x39:\f\x5\x2\x2:;\a\v\x2"+
		"\x2;\x43\x5\b\x5\x6<=\f\x4\x2\x2=>\t\x2\x2\x2>\x43\x5\b\x5\x5?@\f\x3\x2"+
		"\x2@\x41\a\b\x2\x2\x41\x43\x5\b\x5\x4\x42\x33\x3\x2\x2\x2\x42\x36\x3\x2"+
		"\x2\x2\x42\x39\x3\x2\x2\x2\x42<\x3\x2\x2\x2\x42?\x3\x2\x2\x2\x43\x46\x3"+
		"\x2\x2\x2\x44\x42\x3\x2\x2\x2\x44\x45\x3\x2\x2\x2\x45\t\x3\x2\x2\x2\x46"+
		"\x44\x3\x2\x2\x2\n\x10\x18\x1A#+-\x42\x44";
	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
}
} // namespace ManagedClient.Search

#endif
