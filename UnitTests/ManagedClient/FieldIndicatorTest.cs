﻿using System;
using AM.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ManagedClient;

namespace UnitTests.ManagedClient
{
    [TestClass]
    public class FieldIndicatorTest
    {
        [TestMethod]
        public void TestFieldIndicatorConstructor()
        {
            FieldIndicator indicator = new FieldIndicator();
            Assert.AreEqual(false, indicator.HasValue);
            Assert.AreEqual(FieldIndicator.NoValue, indicator.Value);
            Assert.AreEqual(FieldIndicator.NoValue, indicator.ToString());

            string value = "1";
            indicator = new FieldIndicator(value);
            Assert.AreEqual(true, indicator.HasValue);
            Assert.AreEqual(value, indicator.Value);
            Assert.AreEqual(value, indicator.ToString());

            value = "234";
            string expected = "2";
            indicator = new FieldIndicator(value);
            Assert.AreEqual(true, indicator.HasValue);
            Assert.AreEqual(expected, indicator.Value);
            Assert.AreEqual(expected, indicator.ToString());
        }

        [TestMethod]
        public void TestFieldIndicatorSetValue()
        {
            FieldIndicator indicator = new FieldIndicator();
            Assert.AreEqual(false, indicator.HasValue);
            Assert.AreEqual(FieldIndicator.NoValue, indicator.Value);
            Assert.AreEqual(FieldIndicator.NoValue, indicator.ToString());

            string value = "1";
            indicator.SetValue(value);
            Assert.AreEqual(true, indicator.HasValue);
            Assert.AreEqual(value, indicator.Value);
            Assert.AreEqual(value, indicator.ToString());

            value = "234";
            string expected = "2";
            indicator.SetValue(value);
            Assert.AreEqual(true, indicator.HasValue);
            Assert.AreEqual(expected, indicator.Value);
            Assert.AreEqual(expected, indicator.ToString());

            indicator.SetValue(null);
            Assert.AreEqual(false, indicator.HasValue);
            Assert.AreEqual(FieldIndicator.NoValue, indicator.Value);
            Assert.AreEqual(FieldIndicator.NoValue, indicator.ToString());
        }

        private void _TestSerialization
            (
                FieldIndicator first
            )
        {
            byte[] bytes = first.SaveToMemory();

            FieldIndicator second 
                = bytes.RestoreObjectFromMemory<FieldIndicator> ();
            Assert.AreEqual(first.HasValue, second.HasValue);
            Assert.AreEqual(first.Value, second.Value);
            Assert.AreEqual(first.ToString(), second.ToString());
        }

        [TestMethod]
        public void TestFieldIndicatorSerialization()
        {
            FieldIndicator indicator = new FieldIndicator();
            _TestSerialization(indicator);

            indicator = new FieldIndicator("1");
            _TestSerialization(indicator);

            indicator = new FieldIndicator("234");
            _TestSerialization(indicator);
        }
    }
}