using AppSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppSettingsTests
{
    [TestClass]
    public class TypeParserTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TypeParser_GetTypedValue_Bool()
        {
            bool expected = true;
            string input = expected.ToString();

            bool actual = TypeParser.GetTypedValue(typeof(bool), input);
            Assert.AreEqual(expected, actual);
        }
    }
}
