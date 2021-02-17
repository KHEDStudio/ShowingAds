using NUnit.Framework;
using ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class RandomEnumeratorTest
    {
        private List<int> _noNumbers;
        private List<int> _oneNumber;
        private List<int> _numbers;

        [SetUp]
        public void Setup()
        {
            _noNumbers = new List<int>();
            _oneNumber = new List<int>();
            _oneNumber.Add(0);
            _numbers = new List<int>();
            for (int i = 0; i < 100; i++)
                _numbers.Add(i);
        }

        [Test]
        public void NoNumbersTest()
        {
            var enumerator = new RandomEnumerator<int>(new List<int>());
            foreach (var number in _noNumbers)
                enumerator.AddNode(number);
            enumerator.Reset();
            Assert.IsFalse(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current);
        }

        [Test]
        public void OneNumberTest()
        {
            var enumerator = new RandomEnumerator<int>(new List<int>());
            foreach (var number in _oneNumber)
                enumerator.AddNode(number);
            Assert.IsTrue(enumerator.MoveNext());
            var _number = enumerator.Current;
            enumerator.Reset();
            Assert.AreEqual(_number, enumerator.Current);
            enumerator.MoveNext();
            Assert.AreEqual(_number, enumerator.Current);
        }

        [Test]
        public void NumbersTest()
        {
            var enumerator = new RandomEnumerator<int>(new List<int>());
            foreach (var number in _numbers)
                enumerator.AddNode(number);
            for (int i = 0; enumerator.MoveNext() && i < 200; i++)
            {
                var number = (int)enumerator.Current;
                var linearEnumerator = (LinearEnumerator<int>)enumerator;
                linearEnumerator.MoveNext();
                var nextNumber = (int)linearEnumerator.Current;
                Assert.IsTrue(number + 1 == nextNumber || number + 1 == _numbers.Count && nextNumber == 0);
            }

            while (enumerator.MoveNext())
                enumerator.RemoveNode((int)enumerator.Current);
            Assert.AreEqual(0, enumerator.Current);
        }
    }
}
