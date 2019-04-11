Cole Jacobs
CS 3500
10/05/2018

PS5

This solution contains the "internals" of a spreadsheet program. Previous code has been enhanced and extended 
in order to more fully perform like a spreadsheet application. The core of this program is achieved using two 
classes: the Cell class, and the Spreadsheet class (which extends AbstractSpreadsheet). A spreadsheet contains 
an infite number of cells, but it is only important to keep track of nonempty cells. We can achieve this using 
a dictionary as the underlying data structure in the Spreadsheet class. The key-value pair will store the cell's 
name (string) as the key, and the value will be a Cell object (where a Cell represents the most basic storage
unit available in a spreadsheet.) 

New functionality allows for a spreadsheet to be saved. This is accomplished by writing all of the information
contained in the solution internals to a .xml file. This file can then be parsed and re-loaded for further use.

We make use of pre-existing classes contained in two other solutions: 
   - PS2: commit 2685b42
   - PS3: commit 4b2f0fd
These give us access to a DependencyGraph class which allows us to track the relationships among cells, and
a Formula class to represent formulas stored in a cell. Formulas can now be evaluated within the spreadsheet.
The evaluation is stored and can be accessed by the user.
