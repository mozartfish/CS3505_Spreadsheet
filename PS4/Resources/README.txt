Aaron Goh
10/5/2018

Design Doc

NOTICE: Running with latest version of PS2/PS3 Libraries committed on Github in the PS5 Branch (10/5/2018).

Projects:
	References:
		References contains the DLL files from PS2/PS3 libraries 
			DependencyGraph.dll & DependencyGraph.xml from PS2
			Formula.dll & fomrula.xml from PS3
		References library stores theses dlls which are referenced in ALL other projects in the solution.
		Note: References is not built when solution is run
		
	Spreadsheet:
		Classes:
		Cell is a nested class inside of the class Spreadsheet that contains two member variables:
			Content,Type, and Lookup
				-Content is a generic object that can become a double, string, or Formula
					-Due to the limited ways to access cell, the abstract generalization of the object is safe
				-The Type is storing the typeof the object to ensure that the return type of the Cell is correct with its format i.e. Formula, double, string
				-Lookup is the delegate method that can track variable values

		Spreadsheet is a packaged class backed by class variables: Dependencies & Cells
		SpreadSheet is also implementing AbstractSpreadsheet
			-Cells is a Dictionary<String, Cell> that takes the Cell name and links it to the Cell
			-Dependencies stores the Cell relationships 

			Implementation: 
				Many of the checks are used off of Formula.dll's StringExtension class
					which gives a String the extension to check if it is a valid variable.
				Formula.dll is used to call the Formula object and its associated functionalities.
				DependencyGraph.dll is used to check relationships between variables

			Helper Methods:
				Spreadsheet has a protected helper method called RemoveDependencies(String name). This method is used to generically 
					remove the targeted cell as a dependency from its dependees.
				ReturnStringCellContent is a private helper method to return the raw content as a string for writing out the XML File.
				Lookup method takes in a string and attempts to link it to a double otherwise it returns null
				IsVariable adjusted to match new definition of variable				

			Note: The Circular Dependency check was removed from the Visit method in the AbstractSpreadsheet class. The circular dependency check is now down in the SetCellContents so that it is easier to reset the values.
				The check for null is no longer in the individual methods of SetCellContents and is now done a single time in SetCellOfContents

		Project has referenced dlls stored in the References project. 


	SpreadsheetTest:
		Tests Spreadsheet class
			Helper Method: VerifyCellValues - Takes Cell names and checks them against value. Simplifies testing significantly

		Project has referenced dlls stored in the References project. 




