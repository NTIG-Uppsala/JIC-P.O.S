# JIC-P.O.S
A Point of Sale System for a restaurant.

Group members: Carl Eriksson Skogh, Ivar Bjerling, Joel Jansson

## Download and Run the Application
[Click here for download](https://github.com/NTIG-Uppsala/JIC-P.O.S/releases)
- Click the .msi file in the latest release to download it
- If you have a previous version:
  - Go to "Add or remove programs" in the settings app on your computer and remove "Restaurant Point of Sale System"
- Run the installer from where it was downloaded
- Follow the steps in the installer
- Once having completed the installer the program should open automatically. Otherwise launch the "PointOfSaleSystem.exe" app.

### Fixing Possible Errors when Running the Program
#### Problems with the Database
If products are not displayed correctly or you get an SQL error or similar, it is probably caused by having an older database version after new database features have been added.

- To reset the database, try deleting the database folder: `C\:Users\<username>\AppData\Local\Restaurant Point of Sale System`
- **WARNING:** Be aware that this will delete all the data present in the database. For viewing the contents of the database tables and exporting table data if needed, read the [database system documentation](Documents/databaseSystem.md).

## Technical Documentation
* [Testing System](Documents/testingSystem.md)
* [Creating a New Installer](Documents/creatingInstaller.md)
* [Database System](Documents/databaseSystem.md)
### API Documentation
* [API Database Setup](Documents/ApiDatabaseSetup.md)
* [API Setup](Documents/ApiSetup.md)
* [API Endpoints](Documents/ApiEndpoints.md)

## Development Guidelines
* [Programming Languages](Documents/programmingLanguages.md)
* [Coding Standard](Documents/codingStandard.md)
* [Code Analysis](Documents/codeAnalysis.md)
* [Development Environement](Documents/developmentEnvironment.md)
* [Definition of Done](Documents/definitionOfDone.md)
