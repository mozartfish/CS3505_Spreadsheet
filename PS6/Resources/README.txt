Design decisions:
make a form that looks like Excel, maily because Excel has already done all the research behind good design
Process cmd was used for arrow key events, because pressed and down were inconsistent


Future work:
-add another stack to make forward and backward undoing possible
-add random cell coloring function such that the whole screen is colored random colors automatically
-add a game where a cell clicked produces a random tone
-change fonts in UI
-less boring colored GUI



external code resources:
https://stackoverflow.com/
https://docs.microsoft.com/en-us/dotnet/csharp/

Summary of Additional Features:
Carina and Aaron added an "art game" where the user can fill selected cells with a random color
Aaron added a background worker for gui usability while large computations are made, and an undo function, and added multi color functionality to the visualize menu

In Additiion we also added many spice of life changes
-the contents bar is always selected such that if you press annything other than the arrow keys it will be added to the text box
-pressing enter will submit the contents to the spreadsheet, and also move the selected cell down one
-the far right of the contents bar is always selected upon selection for ease of writing
-Arrow keys along with mouse clicking can nagigate to new cells
-closing via the x at the top right of the form with unsaved work prompts user to go save their work
-opening via the open file dialog with unsaved work alows user to throw away data, cancel, or it opens the save file dialog
-the help menu is its own form, so it can be used while working in the spreadsheet, so you dont have to close the help form to do work in the spreadsheet

Special instructions:
-read the popups when you are doing opperations on a spreadsheet that has unsaved data, they are pretty self explanetory
-the undo function holds previous states in a stack, press enter  to put a cell and its contents onto the stack, to remove them and revert the spreadsheet to a previous version press the undo in the top right of the screen
-Additional Feature instructions:
  - See "help" menu
-There is a background worker, it will evaluate the expression when enter is pressed, and will not allow other 
work to be done on other cells while that work is going on


Time Work Log:

Date: 10/11/2018
Authors: Carina Imburgia and Aaron Carlisle

Project Summary:

	This project will bring together the logical compontents of a spreadsheet
and implement them into a functional user interface. Multiple spreadsheets may
be opened and run in concurrance, and saved for later use. The spreadsheet app
will terminate once the last spreadsheet is closed. Some features include:

	- Formula evaluation
	- Read/Write capabilities
	- Versioning
	- Editable cells
	- Error-checking on formulas
	- Cell variable use and dependency caluculation


First meeting:

	- Initialization of the project.
	- We read over and understood project
	- Solution was set up and DLLs were copied over

Initial division of labor:

Carina: Begin file menu with read/write features, write instructions, fix errors
		in PS5.

Aaron: Work on toggle through cells methods, and displaying values in the spreadsheet


Date: 10/12/2018
Authors: Carina Imburgia and Aaron Carlisle

To Do:

- File menu
- Load spreadsheet and check for circulars
- Is valid range for variable, do we need to throw?
- Set cell location


Completed today: 

- File menu: New, Close, Open (partial)
- Wrote drawing psuedo
- Dictionsry map cell name to column and row
- Arrow keys to toggle
- Instantiated backend spreadsheet
- Content writes to cell on enter

Carina: File extension, help menu, save method
Aaron: Open, and displaying backend spreadsheet, update



Date: 10/15/2018
Authors: Aaron Carlisle, Carina Imburgia

ToDo:
AARON
- add trycatch around open and save
- make help menu pop out as own window

CARINA
- need to do extra feature, fonts, size , whatever
- type out better help menu
- add extra feature explaination to README
- XML

EITHER
- Open should REPLACE current spreadsheet
- LOOK OVER GRADING CRITERIA
- TESTING (should be done after everything else)

Completed today
- Now working spreadsheet values and contents along with formula interaction
- fixed may small bugs in the enter method
- added ease of life modifications to enter method
- added clicking on cell functionality
- when you close out of a spreadsheet via close ot xing, the warning dialog pops up if there is unsaved changes
- changed code so that there are no longer magic buttons
- fixed trycatch in enter
- Need to make the clicking do what the arrow keys do, almost same logic too
- made textbox aesthetics changes
- contents is autoselected to the far right always
-


Question:
- How do you open a spreadsheet, and use the path without creating a new constructor
	- So I asked Danny about this. Basically we do use a new instance of SS and create a new constructor. We then replace the old with new,,
	but first we need to save the old. Or at least be prompted to save.

date: 10/19/2018
Completed:
-made it so that you have option of saving work when opening rather than having to close open dialog then save then reopen open dialog
-added SpreadsheetUtilities.FormulaError shows just FormulaError, user doesnt need to see the SpreadsheetUtilities part
-made the click save method return a boolean, should not affect previous code, but allows me to check if the user hit cancel in the save help bar after trying to open 
a sprd while having non saved work. If they did, added a bit of reccursion so then it will return them to the open dialog they came from
-added lots of good logic to the open helper, now works like a real program would


Date: 10/21/2018
Completed:
-Fixed some close related recursion