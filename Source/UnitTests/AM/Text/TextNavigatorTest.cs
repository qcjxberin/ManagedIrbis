﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.Text;

namespace UnitTests.AM.Text
{
    [TestClass]
    public class TextNavigatorTest
    {
        [TestMethod]
        public void TextNavigator_SkipWhitespace()
        {
            const string text = " \tABC ";
            TextNavigator navigator = new TextNavigator(text);
            navigator.SkipWhitespace();
            char c = navigator.ReadChar();
            Assert.AreEqual('A', c);
        }

        [TestMethod]
        public void TextNavigator_ReadFrom()
        {
            const string text1 = "'ABC'DEF";
            TextNavigator navigator = new TextNavigator(text1);
            string actual = navigator.ReadFrom('\'', '\'');
            Assert.AreEqual("'ABC'", actual);

            const string text2 = "'ABCDEF";
            navigator = new TextNavigator(text2);
            actual = navigator.ReadFrom('\'', '\'');
            Assert.AreEqual(string.Empty, actual);

            const string text3 = "ABC'DEF";
            navigator = new TextNavigator(text3);
            actual = navigator.ReadFrom('\'', '\'');
            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void TextNavigator_ReadTo_Char()
        {
            const string text1 = "'ABC'DEF";
            TextNavigator navigator = new TextNavigator(text1);
            char open = navigator.ReadChar();
            string actual = navigator.ReadTo(open);
            Assert.AreEqual("ABC'", actual);

            const string text2 = "'ABC";
            navigator = new TextNavigator(text2);
            open = navigator.ReadChar();
            actual = navigator.ReadTo(open);
            Assert.AreEqual("ABC", actual);
        }

        [TestMethod]
        public void TextNavigator_ReadTo_String1()
        {
            TextNavigator navigator = new TextNavigator("314abc>>>hello");
            string actual = navigator.ReadTo(">>>");
            Assert.AreEqual("314abc", actual);
            Assert.AreEqual("hello", navigator.GetRemainingText());
        }

        [TestMethod]
        public void TextNavigator_ReadTo_String2()
        {
            TextNavigator navigator = new TextNavigator("314abc>>hello");
            string actual = navigator.ReadTo(">>>");
            Assert.AreEqual(null, actual);
            Assert.AreEqual("314abc>>hello", navigator.GetRemainingText());
        }

        [TestMethod]
        public void TextNavigator_ReadUntil1()
        {
            const string text = "'ABC'DEF";
            TextNavigator navigator = new TextNavigator(text);
            char open = navigator.ReadChar();
            string actual = navigator.ReadUntil(open);
            Assert.AreEqual("ABC", actual);
        }

        [TestMethod]
        public void TextNavigator_ReadUntil2()
        {
            char[] openChars = { '(' };
            char[] closeChars = { ')' };
            char[] stopChars = { ')' };

            TextNavigator navigator = new TextNavigator("12345)");
            string actual = navigator.ReadUntil(openChars, closeChars, stopChars);
            Assert.AreEqual("12345", actual);

            navigator = new TextNavigator("12(3)(4)5)");
            actual = navigator.ReadUntil(openChars, closeChars, stopChars);
            Assert.AreEqual("12(3)(4)5", actual);

            navigator = new TextNavigator("12(3(4))5)");
            actual = navigator.ReadUntil(openChars, closeChars, stopChars);
            Assert.AreEqual("12(3(4))5", actual);

            navigator = new TextNavigator("12(3(4))5");
            actual = navigator.ReadUntil(openChars, closeChars, stopChars);
            Assert.AreEqual(null, actual);

            navigator = new TextNavigator("12(3(4)5)");
            actual = navigator.ReadUntil(openChars, closeChars, stopChars);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void TextNavigator_ReadWhile()
        {
            const string text1 = "314abc";
            TextNavigator navigator = new TextNavigator(text1);
            string actual = navigator.ReadWhile('0', '1', '2', '3',
                '4', '5', '6', '7', '8', '9');
            Assert.AreEqual("314", actual);
        }

        [TestMethod]
        public void TextNavigator_SkipWhile()
        {
            const string text1 = "314abc";
            TextNavigator navigator = new TextNavigator(text1);
            navigator.SkipWhile('0', '1', '2', '3',
                '4', '5', '6', '7', '8', '9');
            string actual = navigator.GetRemainingText();
            Assert.AreEqual("abc", actual);
        }

        [TestMethod]
        public void TextNavigator_ReadInteger()
        {
            const string text1 = "314abc";
            TextNavigator navigator = new TextNavigator(text1);
            string actual = navigator.ReadInteger();
            Assert.AreEqual("314", actual);

            actual = navigator.ReadInteger();
            Assert.AreEqual(string.Empty, actual);
        }
    }
}
