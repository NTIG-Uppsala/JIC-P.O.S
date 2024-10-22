using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TestSystem
{
    [TestClass]
    public class CoffeeTest
    {
        private ConditionFactory? cf;
        private FlaUI.Core.Application? app; // Declare app at the class level

        [TestInitialize]
        // Runs before each test
        public void TestInitialize()
        {
            cf = new ConditionFactory(new UIA3PropertyLibrary());
            app = FlaUI.Core.Application.Launch(@"..\..\..\..\PointOfSaleSystem\bin\Release\net8.0-windows\PointOfSaleSystem.exe");
        }

        [TestCleanup]
        // Runs after each test
        public void TestCleanup()
        {
            app.Close();
        }

        [TestMethod]
        public void CheckCoffee()
        {
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            AddItems(window, "CoffeeButton", 2, 25.99f);
        }

        [TestMethod]
        public void ResetTotalPrice()
        {
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            // Add some items to set the total price
            AddItems(window, "CoffeeButton", 5, 25.99f);

            // Reset the total price
            var resetTotalPriceButton = window.FindFirstDescendant(cf.ByAutomationId("ResetTotalPriceButton")).AsButton();
            resetTotalPriceButton.Click();

            // Check that the total price is now zero
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            string totalPriceText = totalPrice.Properties.Name.Value;
            float totalPriceValue = float.Parse(totalPriceText.Replace(" kr", ""));
            Trace.Assert(totalPriceValue == 0, $"Expected 0, but got {totalPriceValue}");
        }

        // Helper method to add items and calculate total price
        private void AddItems(FlaUI.Core.AutomationElements.Window window, string buttonAutomationId, int count, float pricePerItem)
        {
            float calculatePrice = 0;
            var itemButton = window.FindFirstDescendant(cf.ByAutomationId(buttonAutomationId)).AsButton();

            for (int i = 0; i < count; i++)
            {
                itemButton.Click();
                calculatePrice += pricePerItem;
            }

            // Verify the total price after adding items
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            string totalPriceText = totalPrice.Properties.Name.Value;
            float totalPriceValue = float.Parse(totalPriceText.Replace(" kr", ""));
            Trace.Assert(totalPriceValue == calculatePrice, $"Expected {calculatePrice}, but got {totalPriceValue}");
        }
    }
}

