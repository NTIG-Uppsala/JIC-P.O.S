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
  - After exiting Visual Studio, it will install the extension automatically
  - Follow the prompts in the installer
- Create an Advanced Installer project in Visual Studio:
  - Navigate to File->New->Project
  - Select "Advanced Installer Project"
  - In the solution field, select "Add to solution"
  - Choose a name for the installer project in the name field
- After having created the project, a pop-up should appear to install the Advanced Installer program to your computer. When installed, it should launch automatically.
- When launched, choose "Visual Studio Application" and follow the setup wizard
- When prompted to select a solution, choose the main project solution for the application
- In the "Detected files" part of the setup wizard, choose the necessary files for the WPF app, namely the .exe file and other files created from building the solution
- Once having completed the setup wizard, enter the information for the app in the "Product Details" section of the Advanced Installer interface
- While remaining in the "Product Details" section, build the installer by selecting "Build" in the upper left
- After the installer is built, a link appears in the output at the bottom of the window that leads to the .msi file installer

## Documents
* [Programming Languages](Documents/programmingLanguages.md)
* [Coding Standard](Documents/codingStandard.md)
* [Code Analysis](Documents/codeAnalysis.md)
* [Development Environement](Documents/developmentEnvironment.md)
* [Definition of Done](Documents/definitionOfDone.md)
* [Testing System](Documents/testingSystem.md)
