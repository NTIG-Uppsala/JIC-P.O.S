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
using Microsoft.Data.Sqlite;

namespace TestSystem
{

    [TestClass]
    public class ContentTests
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

            //Adds one coffee
            AddItems(window, "CoffeeButton", 1, 25);
            //Adds one pasta carbonara
            AddItems(window, "PastaCarbonaraButton", 1, 170);

            // Find the product window's ListView and get the list length
            int listViewItemsLength = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem)).Length;

            // Ensure that the ListView contains 2 elements
            Trace.Assert(listViewItemsLength == 2, "Test failed: Items not found in the product window.");
        }

        [TestMethod]
        public void AdjustAndVerifyProducts()
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

            void CheckItemButtonsInListView(int productPrice, AutomationElement item, AutomationElement nextItem, int listViewItemsLengthBefore)
            {
                // Access elements using AutomationId
                var productNameText = item.FindFirstDescendant(cf => cf.ByAutomationId("ProductNameText"))?.Name;
                var priceText = item.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name;
                var amountText = item.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;

                var increaseButton = item.FindFirstDescendant(cf => cf.ByAutomationId("IncreaseButton")).AsButton();
                var decreaseButton = item.FindFirstDescendant(cf => cf.ByAutomationId("DecreaseButton")).AsButton();
                var removeButton = item.FindFirstDescendant(cf => cf.ByAutomationId("RemoveButton")).AsButton();

                string nextProductNameText = nextItem?.FindFirstDescendant(cf => cf.ByAutomationId("ProductNameText"))?.Name;
                string nextPriceText = nextItem?.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name;
                string nextAmountText = nextItem?.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;

                // Increase the amount of the product by 1
                increaseButton.Click();
                priceText = item.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name;
                amountText = item.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;
                Trace.Assert(amountText == "2", $"Expected '2' but got {amountText}");
                Trace.Assert(priceText == (productPrice * 2).ToString(), $"Expected {(productPrice * 2).ToString()} but got {priceText}");

                // Check that the next product is unaltered
                if (nextItem != null) // nextItem is null if no next item
                {
                    Trace.Assert(nextAmountText == nextItem.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name,
                                 $"Expected next item amount to be unchanged but was altered for {nextProductNameText}");
                    Trace.Assert(nextPriceText == nextItem.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name,
                                 $"Expected next item amount to be unchanged but was altered for {nextProductNameText}");
                }

                // Decrease the amount of the product by 1
                decreaseButton.Click();
                amountText = item.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name;
                Trace.Assert(amountText == "1", $"Expected '1' but got {amountText}");
                priceText = item.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name;
                Trace.Assert(priceText == (productPrice).ToString(), $"Expected {(productPrice).ToString()} but got {priceText}");

                if (nextItem != null)
                {
                    Trace.Assert(nextAmountText == nextItem.FindFirstDescendant(cf => cf.ByAutomationId("AmountText"))?.Name,
                                 $"Expected next item amount to be unchanged but was altered for {nextProductNameText}");
                    Trace.Assert(nextPriceText == nextItem.FindFirstDescendant(cf => cf.ByAutomationId("PriceText"))?.Name,
                                 $"Expected next item amount to be unchanged but was altered for {nextProductNameText}");
                }

                // Delete the product
                removeButton.Click();
                // listViewItems length is decreased by one after item remove
                listViewItems = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
                Trace.Assert(listViewItemsLengthBefore == listViewItems.Length + 1);
            }

            // Loop through each item (AutomationElement) in the array
            for (int i = 0; i < listViewItemsCopy.Length; i++)
            {
                AutomationElement item = listViewItemsCopy[i];

                // Get the next item in the list if exists
                AutomationElement? nextItem;
                if (i != listViewItemsCopy.Length - 1)
                {
                    nextItem = listViewItemsCopy[i+1];
                }
                else
                {
                    nextItem = null;
                }

                // Get the length of listViewItems before item removal
                int listViewItemsLengthBefore = listViewItems.Length;

                var productNameText = item.FindFirstDescendant(cf => cf.ByAutomationId("ProductNameText"))?.Name;

                // Validate that the increase, decrease and remove buttons work for two products in the product window
                if (productNameText == "Coffee")
                {
                    CheckItemButtonsInListView(25, item, nextItem, listViewItemsLengthBefore);
                }
                else if (productNameText == "Pasta carbonara")
                {
                    CheckItemButtonsInListView(170, item, nextItem, listViewItemsLengthBefore);
                }
            }
        }

        [TestMethod]
        public void VerifyPayment() 
        {
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            // Retrieve the length of the database before clicking the pay button
            string databaseFilePath = @"\database.db";
            string tableName = "order_details";
            var initialOrderTableLength = dataBaseHelper.ReadData(tableName).GetRows();

            // Adds one coffee
            AddItems(window, "CoffeeButton", 1, 25);
            //Adds one Pasta Carbonara
            AddItems(window, "PastaCarbonaraButton", 1, 170);

            // Find the pay button and click it
            var payButton = window.FindFirstDescendant(cf.ByAutomationId("PayButton")).AsButton();
            payButton.Click();

            //Check that the OrderConfirmation textblock is visible
            var orderConfirmation = window.FindFirstDescendant(cf.ByAutomationId("OrderConfirmation")).AsTextBox();
            Trace.Assert(orderConfirmation.Properties.IsOffscreen.Value == false, "Test failed: Order confirmation textblock is not visible.");

            // Find the product window's ListView and get the list length
            int listViewItemsLength = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem)).Length;

            // Ensure that the ListView is empty
            Trace.Assert(listViewItemsLength == 0, "Test failed: Items not found in the product window.");

            // Ensure that the price is 0 kr
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            string totalPriceText = totalPrice.Properties.Name.Value;
            Trace.Assert(totalPriceText == "0 kr", $"Expected 0 kr, but got {totalPriceText}");

            //Check that the database has been updated
            var updatedOrderTableLength = dataBaseHelper.ReadData(tableName).GetRows();
            Trace.Assert(updatedOrderTableLength == initialOrderTableLength + 2, "Test failed: Order table was not updated.");
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

            Trace.Assert(totalPriceText == "0 kr", $"Expected 0 kr, but got {totalPriceText}");
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

