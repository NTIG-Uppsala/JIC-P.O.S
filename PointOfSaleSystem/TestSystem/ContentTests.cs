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
using PointOfSaleSystem.Database;

namespace TestSystem
{

    [TestClass]
    public class ContentTests
    {
        private string firstProductAutomationId = "coffee";
        private string secondProductAutomationId = "scallops";
        private string thirdProductAutomationId = "glass_maison_sans_pareil_sauvignon_blanc";

        private string firstCategoryButtonAutomationId = "starters";
        private string secondCategoryButtonAutomationId = "sauces";
        private string thirdCategoryButtonAutomationId = "warm_drinks";

        private string firstProductName = "Coffee";
        private string secondProductName = "Scallops";
        private string thirdProductName = "Glass Maison Sans Pareil Sauvignon Blanc";

        private int firstProductPrice = 32;
        private int secondProductPrice = 170;
        private int thirdProductPrice = 135;

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

            //Adds two of the same product
            AddItems(window, firstProductAutomationId, 2, firstProductPrice);

            //Adds one product
            AddItems(window, secondProductAutomationId, 1, secondProductPrice);

            //Adds three of the same product
            AddItems(window, thirdProductAutomationId, 3, thirdProductPrice);
            ResetTotalPrice();
        }

        [TestMethod]
        public void CheckProductWindow()
        {
            // Creates a UIA3Automation instance and disposes it after use.
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            AddItems(window, firstProductAutomationId, 1, firstProductPrice);
            AddItems(window, secondProductAutomationId, 1, secondProductPrice);

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

            AddItems(window, firstProductAutomationId, 1, firstProductPrice);
            AddItems(window, secondProductAutomationId, 1, secondProductPrice);

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
                    nextItem = listViewItemsCopy[i + 1];
                }
                else
                {
                    nextItem = null;
                }

                // Get the length of listViewItems before item removal
                int listViewItemsLengthBefore = listViewItems.Length;

                var productNameText = item.FindFirstDescendant(cf => cf.ByAutomationId("ProductNameText"))?.Name;

                // Validate that the increase, decrease and remove buttons work for two products in the product window
                if (productNameText == firstProductName)
                {
                    CheckItemButtonsInListView(firstProductPrice, item, nextItem, listViewItemsLengthBefore);
                }
                else if (productNameText == secondProductName)
                {
                    CheckItemButtonsInListView(secondProductPrice, item, nextItem, listViewItemsLengthBefore);
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

            // Connect to the database (to get the count of rows)
            using (var connection = DatabaseHelper.CreateConnection())
            {
                if (connection != null)
                {
                    // Call the static method from DatabaseHelper to get the count of rows
                    int initialOrderDetailsCount = OrderDetailsTable.GetOrderDetailsCount(connection);

                    AddItems(window, firstProductAutomationId, 1, firstProductPrice);
                    AddItems(window, secondProductAutomationId, 1, secondProductPrice);

                    // Find the pay button and click it
                    var payButton = window.FindFirstDescendant(cf.ByAutomationId("PayButton")).AsButton();
                    payButton.Click();

                    //Check that the OrderConfirmation textblock is visible
                    var orderConfirmation = window.FindFirstDescendant(cf.ByAutomationId("OrderConfirmation")).AsTextBox();
                    Trace.Assert(orderConfirmation.Properties.IsOffscreen.Value == false, "Test failed: Order confirmation textblock is not visible.");

                    // Find the product window's ListView and get the list length
                    int listViewItemsLength = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem)).Length;

                    // Ensure that the ListView is empty
                    Trace.Assert(listViewItemsLength == 0, "Test failed: Items found in the product window after payment.");

                    // Ensure that the price is 0 SEK
                    var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
                    string totalPriceText = totalPrice.Properties.Name.Value;
                    Trace.Assert(totalPriceText == "0 SEK", $"Expected 0 SEK, but got {totalPriceText}");

                    //Check that the database has been updated
                    var updatedOrderDetailsCount = OrderDetailsTable.GetOrderDetailsCount(connection);
                    Trace.Assert(updatedOrderDetailsCount == initialOrderDetailsCount + 2, "Test failed: Order table was not updated.");
                }
            }
        }

        [TestMethod]
        public void VerifyCategoryProductVisibility()
        {
            using var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);

            // Find and click on the warm drinks category button
            var warmDrinksCategoryButton = window.FindFirstDescendant(cf.ByAutomationId(thirdCategoryButtonAutomationId )).AsButton();
            warmDrinksCategoryButton.Click();

            // Try to find two product buttons, where one of them is and one of them is not supposed to be visible
            var secondProduct = window.FindFirstDescendant(cf.ByAutomationId(secondProductAutomationId)).AsButton();
            var firstProduct = window.FindFirstDescendant(cf.ByAutomationId(firstProductAutomationId)).AsButton();

            Trace.Assert(secondProduct == null, $"Test failed: {secondProductName} was found.");
            Trace.Assert(firstProduct != null, $"Test failed: {firstProductName} was not found.");

            // Find and click on the starters category button
            var startersCategoryButton = window.FindFirstDescendant(cf.ByAutomationId(firstCategoryButtonAutomationId)).AsButton();
            startersCategoryButton.Click();

            secondProduct = window.FindFirstDescendant(cf.ByAutomationId(secondProductAutomationId)).AsButton();
            firstProduct = window.FindFirstDescendant(cf.ByAutomationId(firstProductAutomationId)).AsButton();

            Trace.Assert(secondProduct != null, $"Test failed: {secondProductName} was not found.");
            Trace.Assert(firstProduct == null, $"Test failed: {firstProductName} was found.");
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

            Trace.Assert(totalPriceText == "0 SEK", $"Expected 0 SEK, but got {totalPriceText}");
        }

        // Helper method to add items and calculate total price
        private void AddItems(FlaUI.Core.AutomationElements.Window window, string buttonAutomationId, int count, int pricePerItem)
        {
            // Get the current total price
            var totalPrice = window.FindFirstDescendant(cf.ByAutomationId("TotalPrice")).AsTextBox();
            string totalPriceText = totalPrice.Properties.Name.Value;
            int totalPriceValue = int.Parse(totalPriceText.Replace(" SEK", ""));

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
            totalPriceValue = int.Parse(totalPriceText.Replace(" SEK", ""));

            Trace.Assert(totalPriceValue == calculatePrice, $"Expected {calculatePrice}, but got {totalPriceValue}");
        }
    }
}
