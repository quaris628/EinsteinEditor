using Bibyte.functional.background.booleans;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BibyteTests.booleans
{
    [TestClass]
    public class NotBoolTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            FakeInputBool fakeBool = new FakeInputBool();
            NotBool boolean = new NotBool(fakeBool);
            // TODO idk what this whole thing will be
        }
    }
}
