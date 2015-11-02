using System.Linq;
using AppSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppSettingsTests
{
    [TestClass]
    public class AppSettingsAttributeTests
    {
        [TestMethod]
        public void AppSettingsAttribute_Specified()
        {
            string propertyName = "Override";
            string attributeMemberName = "Name";

            var nameArgument = GetName(propertyName, attributeMemberName);

            Assert.AreEqual("Overriden", nameArgument);
        }

        [TestMethod]
        public void AppSettingsAttribute_NotSpecified()
        {
            string propertyName = "Default";
            string attributeMemberName = "Name";

            var nameArgument = GetName(propertyName, attributeMemberName);

            Assert.IsNull(nameArgument);
        }

        private static string GetName(string propertyName, string attributeMemberName)
        {
            var test = new TestSettings();
            var defaultProperty = test.GetType().GetProperty(propertyName);
            var fckAttribute = defaultProperty.CustomAttributes
                .Where(a => a.AttributeType == typeof(AppSettingAttribute))
                .FirstOrDefault();
            var nameArgument = fckAttribute.NamedArguments
                .Where(arg => arg.MemberName == attributeMemberName)
                .FirstOrDefault();

            return nameArgument.TypedValue.Value != null ?
                nameArgument.TypedValue.Value.ToString() :
                null;
        }

        private class TestSettings
        {
            [AppSetting]
            public int Default { get; set; }

            [AppSetting(Key = "Overriden")]
            public int Override { get; set; }
        }
    }
}
