using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;

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
            app = FlaUI.Core.Application.Launch("C:\\Users\\carl.erikssonskogh\\Documents\\GitHub\\JIC-P.O.S\\PointOfSaleSystem\\PointOfSaleSystem\\bin\\Release\\net8.0-windows\\PointOfSaleSystem.exe");
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
            // Create an automation object that is disposed when done
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);
            float calculatePrice = 0;
            FlaUI.Core.AutomationElements.Button coffeeButton = window.FindFirstDescendant(cf.ByAutomationId("CoffeeButton")).AsButton();
            // Click the button twice
            for (int i = 2; i-- > 0;)
            {
                coffeeButton.Click();
                calculatePrice += 25.99f;
            }

            // Check that the total price is correct
            FlaUI.Core.AutomationElements.TextBox totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();

            // Extract the Name property, which contains the displayed text
            string totalPriceText = totalPrice.Properties.Name.Value;

            // Remove the "kr" and parse it to float
            float totalPriceValue = float.Parse(totalPriceText.Replace(" kr", ""));
            Trace.Assert(totalPriceValue == calculatePrice, $"Expected {calculatePrice}, but got {totalPriceValue}");

            // Find the reset button and click it
            FlaUI.Core.AutomationElements.Button resetTotalPriceButton = window.FindFirstDescendant(cf.ByAutomationId("ResetTotalPriceButton")).AsButton();
            resetTotalPriceButton.Click();

            // Check that the total price is 0
            totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            // Extract the Name property, which contains the displayed text
            totalPriceText = totalPrice.Properties.Name.Value;

            // Remove the "kr" and parse it to float
            totalPriceValue = float.Parse(totalPriceText.Replace(" kr", ""));
            Trace.Assert(totalPriceValue == 0, $"Expected {calculatePrice}, but got {totalPriceValue}");
        }
    }
}