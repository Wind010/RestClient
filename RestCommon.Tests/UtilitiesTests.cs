//-----------------------------------------------------------------------
// <summary>
//      Unit tests for the RestCommon library.
// </summary>
//-----------------------------------------------------------------------


using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rest.Common.Tests
{
    using System.Text;

    using Common;

    [TestClass]
    public class UtilitiesTests
    {
        private static TestObject _testObj = new TestObject();
        private const string _testObjJson = "{\"Name\":\"Tester.Test\",\"Email\":\"test@test.com\",\"Notes\":\"This is a test.\",\"Ranking\":1}";
        /*
            {
                "Name": "Tester.Test",
                "Email": "test@test.com",
                "Notes": "This is a test."
                "Ranking": 1
            }
        */

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _testObj.Name = "Tester.Test";
            _testObj.Email = "test@test.com";
            _testObj.Notes = "This is a test.";
            _testObj.Ranking = 1;
        }


        [TestInitialize]
        public void TestInitialize()
        {
        }


        [TestMethod]
        public void SerializeToJson_X_Successful()
        {
            var utilities = new Utilities();
            string strTest = utilities.SerializeToJson(_testObj);

            Assert.IsFalse(string.IsNullOrEmpty(strTest));
            Assert.IsTrue(strTest == _testObjJson);
        }


        [TestMethod]
        public void DeserializeToObject_X_Successful()
        {
            var testObj = new TestObject();
            var utilities = new Utilities();
            testObj = utilities.DeserializeToObject<TestObject>(_testObjJson, Encoding.UTF8);

            Assert.IsNotNull(testObj);
            Assert.IsTrue(testObj.Equals(_testObj, StringComparison.OrdinalIgnoreCase));
        }


    }
}


