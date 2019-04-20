PS5 Implementation Comments and Ideas
Author: Carina Imburgia
Date: 09/29/2018


Overall summary:

	This solution will create an update to PS3, which sought to handle cells within a spreadsheet
and their dependencies on one another. Rather than adding/updating contents within a cell to one of
three SetCellContents methods, this solution will deptermine what the contents are and pass them 
into the appropriate SetCellContents method (now protected). 
	
	Additionally, this solution will now handle variable validity on two levels; first specified by 
the solution, then further specified by the user's IsValid delegate.

	The spreadsheet will be versioned and saved using xml documentation that saves the state of the
spreadsheet. Error checking will ensure that the file can be opened, read, and closed appropriately.
All contents of the cells should be saved with a toString method.

	Furthermore, this updated version will handle calculation of the value of a cell. If a cell
depends on another cell for its value, then it will caluculate the value apprpriately.

Summaries of other requirments:

1. Variables and validity:
	
	- Must be one or more letters followed by one or more digits
	- Can have further constraints (IsValid)

2. Spreadsheet Versions:

	- Start with default version
	- What to do next?

3. New Constructors:

	- Three constructors:
		a. Zero argument: no extra validity parameters or normalization
		b. Three argument: User will provide a validity, normalization, version
		c. Four argument: Same as three and also a filepath

4. Error checking/handling

	- If the version of the saved spreadsheet does not match the version parameter provided to the 
	  constructor, an exception should be thrown.
	- Names contained in spreadsheet should all be valid
	- No invalid formulas or circular dependencies should be saved
	- File does not exist
	- etc. ???

6. Changed

	- Has the spreadsheet been modified after construction?
	- Once the spreadsheet has been saved it is no longer changed

7. Save
	
	- Save the spreadsheet as an xml file on the disc

Potential Tests By Method:

1. GetSavedVersion(String filename)

	- File does not exist
	- File does not direct to appropriate directory
	- File version must be accurate
	- Overwrite should or should not create a new version??

2. Save(String filename)

	- How would you test for XML contents to be accurate?
	- How are we going to decide the version? 1,2,3,4,5??
	- Make sure all contents are changed to strings and '=' sign is placed in front of formulas
	- Make sure it saves to the appropriate directory

3. GetCellValue(String name)

	- Cell that contains a divide by zero
	- Cell that depends on other cells
	- Do we need to save the values somewhere? Should the values of each of its successors be
	  calculated each time? 
	- Put this into cell class

4. GetNamesOfAllNonemptyCells()

	- Do cells with formula errors count?
	- Update and empty cell with a divide by zero, does it get added?
	- Will this be used to write the xml documentation?

5. GetCellContents(String name)

	- Call on empty cell
	- Call with invalid cell name

6. SetContentsOfCell

	- This method should be short. It is used to determine if the contents of a cell is of type 
	  string, double or formula. Then pass to the appropriate parotected method

Private Methods:

- ReadXML

===================================[Questions for TAs]===============================================

- If cell names must all be normalized, is that done twice (in evaluate and in Spreadsheet??
- What does changed mean
- constructors: can you write the contents once?
- debugg other method
- data structure
- solution name?
- get saved vesion if passed in filepath?
- is the filepath constructor only for loading??
- lookup in getvalue method?
- better to pass in as parameter or compute after construction and set?
