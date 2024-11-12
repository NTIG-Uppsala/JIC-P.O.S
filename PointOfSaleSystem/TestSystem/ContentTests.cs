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

            //Adds one Pasta Carbonara
            AddItems(window, "PastaCarbonaraButton", 1, 170);

            //Adds three Meatballs and Mashed Potatoes
            AddItems(window, "MeatballsMashedPotatoesButton", 3, 100);
            ResetTotalPrice();
        }

        [TestMethod]
        public void CheckProductWindow()
        {
            // Creates a UIA3Automation instance and disposes it after use.
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            //Adds two coffees
            AddItems(window, "CoffeeButton", 2, 25);
            //Adds two Pasta Carbonara
            AddItems(window, "PastaCarbonaraButton", 2, 170);

            // Find all the elements that match the criteria (e.g., ListView rows)
            var listViewItems = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            // Ensure that listViewItems contains elements
            Trace.Assert(listViewItems.Length > 0, "Test failed: No items found in the product window.");

            // Loop through each item (AutomationElement) in the array
            foreach (var item in listViewItems)
            {
                // Access elements using AutomationId
                var productNameText = item.FindFirstDescendant(cf => cf.ByAutomationId("ProductNameText"))?.Name;
                var priceText = item.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name;
                var amountText = item.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;
                
                // Validate that two coffees and pasta carbonaras are present in the product window
                if (productNameText == "Coffee")
                {
                    Trace.Assert(priceText == "50", $"Expected '50 kr' but got {priceText}");
                    Trace.Assert(amountText == "2", $"Expected '2' but got {amountText}");
                }

                else if (productNameText == "Pasta carbonara")
                {
                    Trace.Assert(priceText == "340", $"Expected '340 kr' but got {priceText}");
                    Trace.Assert(amountText == "2", $"Expected '2' but got {amountText}");
                }
            }
        }

        [TestMethod]
        public void AjdustAndVerifyProducts()
        {
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            //Adds one coffee
            AddItems(window, "CoffeeButton", 1, 25);
            //Adds one Pasta Carbonara
            AddItems(window, "PastaCarbonaraButton", 1, 170);

            // Find all the elements that match the criteria (e.g., ListView rows)
            var listViewItems = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
            // Make a copy of the array to loop through instead of the original to avoid a runtime error
            var listViewItemsCopy = listViewItems;

            // Ensure that listViewItems contains elements
            Trace.Assert(listViewItems.Length > 0, "Test failed: No items found in the product window.");

            void CheckItemButtonsInListView(int productPrice, FlaUI.Core.AutomationElements.AutomationElement item, int listViewItemsLengthBefore)
            {
                // Access elements using AutomationId
                var productNameText = item.FindFirstDescendant(cf => cf.ByAutomationId("ProductNameText"))?.Name;
                var priceText = item.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name;
                var amountText = item.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;

                var increaseButton = item.FindFirstDescendant(cf => cf.ByAutomationId("IncreaseButton")).AsButton();
                var decreaseButton = item.FindFirstDescendant(cf => cf.ByAutomationId("DecreaseButton")).AsButton();
                var removeButton = item.FindFirstDescendant(cf => cf.ByAutomationId("RemoveButton")).AsButton();

                // Increase the amount of the product by 1
                increaseButton.Click();
                amountText = item.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;
                Trace.Assert(amountText == "2", $"Expected '2' but got {amountText}");
                Trace.Assert(priceText == (productPrice * 2).ToString(), $"Expected {(productPrice * 2).ToString()} but got {priceText}");

                // Decrease the amount of the product by 1
                decreaseButton.Click();
                amountText = item.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;
                Trace.Assert(amountText == "1", $"Expected '1' but got {amountText}");
                Trace.Assert(priceText == (productPrice).ToString(), $"Expected {(productPrice).ToString()} but got {priceText}");

                // Delete the product
                removeButton.Click();
                // listViewItems length is decreased by one after item remove
                Trace.Assert(listViewItemsLengthBefore == listViewItems.Length + 1);
            }
            // Loop through each item (AutomationElement) in the array
            foreach (var item in listViewItemsCopy)
            {
                // Get the length of listViewItems before item removal
                int listViewItemsLengthBefore = listViewItems.Length;

                var productNameText = item.FindFirstDescendant(cf => cf.ByAutomationId("ProductNameText"))?.Name;

                // Validate that the increase, decrease and remove buttons work for two products in the product window
                if (productNameText == "Coffee")
                {
                    CheckItemButtonsInListView(25, item, listViewItemsLengthBefore);

                }
                else if (productNameText == "Pasta carbonara")
                {
                    CheckItemButtonsInListView(170, item, listViewItemsLengthBefore);
                }
            }
        }
   
        // Helper method to reset the total price
        private void ResetTotalPrice()
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
            // Get the current total price
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            string totalPriceText = totalPrice.Properties.Name.Value;
            int totalPriceValue = int.Parse(totalPriceText.Replace(" kr", ""));

            int calculatePrice = totalPriceValue;
            var itemButton = window.FindFirstDescendant(cf.ByAutomationId(buttonAutomationId)).AsButton();

            // Clicks the specified button 'count' times to add items, 
            // and updates the calculated total price by adding the pricePerItem.
            for (int i = 0; i < count; i++)
            {
                itemButton.Click();
                calculatePrice += pricePerItem;
            }

            //Gets the new total price
            totalPriceText = totalPrice.Properties.Name.Value;
            totalPriceValue = int.Parse(totalPriceText.Replace(" kr", ""));

            Trace.Assert(totalPriceValue == calculatePrice, $"Expected {calculatePrice}, but got {totalPriceValue}");
        }
    }
}

