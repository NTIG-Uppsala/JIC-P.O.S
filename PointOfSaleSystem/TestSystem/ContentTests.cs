using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows;
using PointOfSaleSystem;
using static PointOfSaleSystem.MainWindow;
using System.Windows.Controls;

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

            // Wait for the window to be fully loaded
            System.Threading.Thread.Sleep(1000); // Wait for 1 second (adjust as necessary)
    }

        [TestCleanup]
        // Runs after each test
        public void TestCleanup()
        {
            app.Close();
        }

        [TestMethod]
        public void CheckButtons()
        {
            // Creates a UIA3Automation instance and disposes it after use.
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            //Adds two coffees
            AddItems(window, "CoffeeButton", 2 ,25);
            ResetTotalPrice();

            //Adds one Pasta Carbonara
            AddItems(window, "PastaCarbonaraButton", 1, 170);
            ResetTotalPrice();

            //Adds three Meatballs and Mashed Potatoes
            AddItems(window, "MeatballsMashedPotatoesButton", 3, 100);
            ResetTotalPrice();
        }

        [TestMethod]
        public void ResetTotalPrice()
        {
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            // Reset the total price
            var resetTotalPriceButton = window.FindFirstDescendant(cf.ByAutomationId("ResetTotalPriceButton")).AsButton();
            resetTotalPriceButton.Click();

            // Check that the total price is now zero
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            string totalPriceText = totalPrice.Properties.Name.Value;

            Trace.Assert(totalPriceText == "0 kr", $"Expected 0, but got {totalPriceText}");
        }

        // Helper method to add items and calculate total price
        private void AddItems(FlaUI.Core.AutomationElements.Window window, string buttonAutomationId, int count, int pricePerItem)
        {
            int calculatePrice = 0;
            var itemButton = window.FindFirstDescendant(cf.ByAutomationId(buttonAutomationId)).AsButton();

            // Clicks the specified button 'count' times to add items, 
            // and updates the calculated total price by adding the pricePerItem.
            for (int i = 0; i < count; i++)
            {
                itemButton.Click();
                calculatePrice += pricePerItem;
            }

            // Verify the total price after adding items
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            string totalPriceText = totalPrice.Properties.Name.Value;
            int totalPriceValue = int.Parse(totalPriceText.Replace(" kr", ""));
            Trace.Assert(totalPriceValue == calculatePrice, $"Expected {calculatePrice}, but got {totalPriceValue}");
        }
    }
}

