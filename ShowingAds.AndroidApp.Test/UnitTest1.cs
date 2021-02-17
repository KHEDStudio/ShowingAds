using NUnit.Framework;
using ShowingAds.Android.BroadcastReceivers;

namespace ShowingAds.Android.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            new BootCompletedReceiver();
        }
    }
}