#if FW35

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from NumberRanges.g4 by ANTLR 4.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591

namespace AM.Text.Ranges {
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.3")]
[System.CLSCompliant(false)]
public partial class NumberRangesParser : Parser {
	public const int
		NUMBER=1, MINUS=2, DELIMITER=3;
	public static readonly string[] tokenNames = {
		"<INVALID>", "NUMBER", "'-'", "DELIMITER"
	};
	public const int
		RULE_program = 0, RULE_item = 1, RULE_range = 2, RULE_one = 3;
	public static readonly string[] ruleNames = {
		"program", "item", "range", "one"
	};

	public override string GrammarFileName { get { return "NumberRanges.g4"; } }

	public override string[] TokenNames { get { return tokenNames; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return _serializedATN; } }

	public NumberRangesParser(ITokenStream input)
		: base(input)
	{
		_interp = new ParserATNSimulator(this,_ATN);
	}
	public partial class ProgramContext : ParserRuleContext {
		public ItemContext item(int i) {
			return GetRuleContext<ItemContext>(i);
		}
		public ItemContext[] item() {
			return GetRuleContexts<ItemContext>();
		}
		public ITerminalNode Eof() { return GetToken(NumberRangesParser.Eof, 0); }
		public ProgramContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_program; } }
		public override void EnterRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.EnterProgram(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.ExitProgram(this);
		}
	}

	[RuleVersion(0)]
	public ProgramContext program() {
		ProgramContext _localctx = new ProgramContext(_ctx, State);
		EnterRule(_localctx, 0, RULE_program);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 9;
			_errHandler.Sync(this);
			_la = _input.La(1);
			do {
				{
				{
				State = 8; item();
				}
				}
				State = 11;
				_errHandler.Sync(this);
				_la = _input.La(1);
			} while ( _la==NUMBER );
			State = 13; Match(Eof);
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

	public partial class ItemContext : ParserRuleContext {
		public OneContext one() {
			return GetRuleContext<OneContext>(0);
		}
		public RangeContext range() {
			return GetRuleContext<RangeContext>(0);
		}
		public ItemContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_item; } }
		public override void EnterRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.EnterItem(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.ExitItem(this);
		}
	}

	[RuleVersion(0)]
	public ItemContext item() {
		ItemContext _localctx = new ItemContext(_ctx, State);
		EnterRule(_localctx, 2, RULE_item);
		try {
			State = 17;
			switch ( Interpreter.AdaptivePredict(_input,1,_ctx) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 15; range();
				}
				break;

			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 16; one();
				}
				break;
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

	public partial class RangeContext : ParserRuleContext {
		public IToken start;
		public IToken stop;
		public ITerminalNode MINUS() { return GetToken(NumberRangesParser.MINUS, 0); }
		public ITerminalNode NUMBER(int i) {
			return GetToken(NumberRangesParser.NUMBER, i);
		}
		public ITerminalNode[] NUMBER() { return GetTokens(NumberRangesParser.NUMBER); }
		public RangeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_range; } }
		public override void EnterRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.EnterRange(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.ExitRange(this);
		}
	}

	[RuleVersion(0)]
	public RangeContext range() {
		RangeContext _localctx = new RangeContext(_ctx, State);
		EnterRule(_localctx, 4, RULE_range);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 19; _localctx.start = Match(NUMBER);
			State = 20; Match(MINUS);
			State = 21; _localctx.stop = Match(NUMBER);
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

	public partial class OneContext : ParserRuleContext {
		public ITerminalNode NUMBER() { return GetToken(NumberRangesParser.NUMBER, 0); }
		public OneContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_one; } }
		public override void EnterRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.EnterOne(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			INumberRangesListener typedListener = listener as INumberRangesListener;
			if (typedListener != null) typedListener.ExitOne(this);
		}
	}

	[RuleVersion(0)]
	public OneContext one() {
		OneContext _localctx = new OneContext(_ctx, State);
		EnterRule(_localctx, 6, RULE_one);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 23; Match(NUMBER);
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

	public static readonly string _serializedATN =
		"\x3\xAF6F\x8320\x479D\xB75C\x4880\x1605\x191C\xAB37\x3\x5\x1C\x4\x2\t"+
		"\x2\x4\x3\t\x3\x4\x4\t\x4\x4\x5\t\x5\x3\x2\x6\x2\f\n\x2\r\x2\xE\x2\r\x3"+
		"\x2\x3\x2\x3\x3\x3\x3\x5\x3\x14\n\x3\x3\x4\x3\x4\x3\x4\x3\x4\x3\x5\x3"+
		"\x5\x3\x5\x2\x2\x2\x6\x2\x2\x4\x2\x6\x2\b\x2\x2\x2\x19\x2\v\x3\x2\x2\x2"+
		"\x4\x13\x3\x2\x2\x2\x6\x15\x3\x2\x2\x2\b\x19\x3\x2\x2\x2\n\f\x5\x4\x3"+
		"\x2\v\n\x3\x2\x2\x2\f\r\x3\x2\x2\x2\r\v\x3\x2\x2\x2\r\xE\x3\x2\x2\x2\xE"+
		"\xF\x3\x2\x2\x2\xF\x10\a\x2\x2\x3\x10\x3\x3\x2\x2\x2\x11\x14\x5\x6\x4"+
		"\x2\x12\x14\x5\b\x5\x2\x13\x11\x3\x2\x2\x2\x13\x12\x3\x2\x2\x2\x14\x5"+
		"\x3\x2\x2\x2\x15\x16\a\x3\x2\x2\x16\x17\a\x4\x2\x2\x17\x18\a\x3\x2\x2"+
		"\x18\a\x3\x2\x2\x2\x19\x1A\a\x3\x2\x2\x1A\t\x3\x2\x2\x2\x4\r\x13";
	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
}
} // namespace AM.Text.Ranges

#endif