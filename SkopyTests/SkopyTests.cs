using NUnit.Framework;
using Skopy;
using System;

namespace SkopyTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAngleDifference()
        {
            var delta = (45 - 135 + 540) % 360 - 180;
            Assert.AreEqual(-90, delta);
            delta = (135 - 45 + 540) % 360 - 180;
            Assert.AreEqual(90, delta);
        }

        [Test]
        public void TestVectorToAngle()
        {
            Assert.AreEqual(90, GetAngle(0, 1));
            Assert.AreEqual(225, GetAngle(-10, -10));
        }

        private double GetAngle(int x, int y)
        {
            var angle = Math.Atan2(y, x);   //radians
                                            // you need to devide by PI, and MULTIPLY by 180:
            var degrees = 180 * angle / Math.PI;  //degrees
            return (360 + Math.Round(degrees)) % 360; //round number, avoid decimal fragments
        }
    }
}