# JIC-P.O.S
A point of sale system for a restaurant.

Group members: Carl Eriksson Skogh, Ivar Bjerling, Joel Jansson

## Download and Run the Application
[Click here for download](https://github.com/NTIG-Uppsala/JIC-P.O.S/releases)
- Click the .msi file in the latest release to download it
- If you have a previous version:
  - Go to "Add or remove programs" in the settings app on your computer and remove "Restaurant Point Of Sale System"
- Run the installer from where it was downloaded
- Follow the steps in the installer
- Once having completed the installer the program should open automatically. Otherwise launch the "PointOfSaleSystem.exe" app.

## Create a new installer for the application
- Make sure you have built the solution for the app
- Install the "Advanced Installer" extension:
  - Download "Advanced Installer" in the extensions tab for Visual Studio
  - After pressing install you will be prompted to exit Visual Studio
  - After exiting Visual Studio, it will install the extension and the app automatically
  - Follow the prompts in the installer
- Start the Advanced Installer app
- Double click Visual Studio Application
- Specify product name and organization (If there is one)
- Check MSI setup file and click next
- Click next again
- Select the Visual Studio solution you wish to import and click next
- Select release and click next
- Select the files needed for your installation
  - To find what files to include open your Visual Studio release folder
- After selecting all the necessary files click next and then finish
- Now you should have the Advanced Installer open
- Under the product details tab you can change your desired
  - Product name
  - Product version
  - Organization / Company (If there is one)
- Then navigate to the files and folders tab
- Select the Application Folder
- Now go back to your Visual Studio release folder
- Drag and drop the runtimes folder into the Application Folder in Advanced Installer
- Then build the msi file by either
  - Clicking f7
  - Clicking the second icon in the top left corner
- When the build is complete click the file path located in the terminal at the bottom of the window.
  - This takes you directly to your newly generated msi file

## Technical Documentation
* [Testing System](Documents/testingSystem.md)
* [Adding new products](Documents/AddingNewProducts.md)

## Development Guidelines
* [Programming Languages](Documents/programmingLanguages.md)
* [Coding Standard](Documents/codingStandard.md)
* [Code Analysis](Documents/codeAnalysis.md)
* [Development Environement](Documents/developmentEnvironment.md)
* [Definition of Done](Documents/definitionOfDone.md)