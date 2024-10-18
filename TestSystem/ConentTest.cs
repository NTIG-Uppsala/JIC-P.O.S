using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting; // Ensure this is included
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
            // Launch the application
            app = FlaUI.Core.Application.Launch("C:\\Users\\carl.erikssonskogh\\Documents\\WpfApp1\\WpfApp1\\bin\\Release\\net8.0-windows\\WpfApp1.exe");
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

            FlaUI.Core.AutomationElements.Button coffeeButton = window.FindFirstDescendant(cf.ByName("CoffeeButton")).AsButton();
            // Click the button twice
            for (int i = 2; i-- > 0;)
            {
                coffeeButton.Click();
                calculatePrice += 20;
            }

            // Check that the total price is correct
            FlaUI.Core.AutomationElements.TextBox totalPrice = window.FindFirstDescendant(cf.ByName("totalPrice")).AsTextBox();
            float totalPriceValue = float.Parse(totalPrice.Text);
            Trace.Assert(totalPriceValue == calculatePrice, $"Expected {calculatePrice}, but got {totalPriceValue}");

            // Find the reset button and click it
            FlaUI.Core.AutomationElements.Button resetTotalPriceButton = window.FindFirstDescendant(cf.ByName("resetTotalPriceButton")).AsButton();
            resetTotalPriceButton.Click();

            // Check that the total price is 0
            totalPrice = window.FindFirstDescendant(cf.ByName("totalPrice")).AsTextBox();
            totalPriceValue = float.Parse(totalPrice.Text);
            Trace.Assert(totalPriceValue == 0, $"Expected {calculatePrice}, but got {totalPriceValue}");
        }
    }
}