using NUnit.Framework;
using ShowingAds.AndroidApp.Core.BusinessCollections;
using ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using static ShowingAds.AndroidApp.Core.Test.ContentsEnumeratorTest;
using ShowingAds.CoreLibrary;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class LinearEnumeratorTest
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
            var enumerator = new LinearEnumerator<int>(new List<int>());
            foreach (var number in _noNumbers)
                enumerator.AddNode(number);
            enumerator.Reset();
            Assert.IsFalse(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current);
        }

        [Test]
        public void OneNumberTest()
        {
            var enumerator = new LinearEnumerator<int>(new List<int>());
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
            var enumerator = new LinearEnumerator<int>(new List<int>());
            foreach (var number in _numbers)
                enumerator.AddNode(number);
            for (int i = 0; enumerator.MoveNext() && i < 200; i++)
                Assert.AreEqual(i % 100, enumerator.Current);
            Assert.AreEqual(0, enumerator.Current);
        }

        [Test]
        public void DynamicTest()
        {
            var clients = new TopLevelCollection<LowLevelCollection<IntComponent>>
                (new FakeStore<List<LowLevelCollection<IntComponent>>>());
            clients.Add(new LowLevelCollection<IntComponent>(1.ToGuid(), new List<IntComponent>()));
            clients.Add(new LowLevelCollection<IntComponent>(2.ToGuid(), new List<IntComponent>()));

            clients.Components[0].Add(new IntComponent(11));
            clients.Components[0].Add(new IntComponent(12));
            clients.Components[0].Add(new IntComponent(13));

            clients.Components[1].Add(new IntComponent(21));
            clients.Components[1].Add(new IntComponent(22));
            clients.Components[1].Add(new IntComponent(23));

            var total = 100;

            while (total --> 0)
            {
                clients.TryGetNext(out var coll);
                coll.TryGetNext(out var item);

                var a = item.Number;
            }
            Assert.Pass();
        }

        private class IntComponent : Component
        {
            public int Number { get; private set; }

            public IntComponent(int number)
            {
                Number = number;
            }

            public override void Accept(BaseVisitor visitor)
            {
            }

            public override void Add(Component component)
            {
            }

            public override Guid GetId() => Guid.Empty;

            public override bool IsValid(BaseFilter filter)
            {
                return true;
            }

            public override void Remove(Component component)
            {
            }
        }
    }
}
