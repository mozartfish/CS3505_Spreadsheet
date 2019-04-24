Required pre use:
	To use our admin front end the Json must be up to date, to do this, right click the solution, and select manage Nuget Packages.
Then instal and update.


Useage:
type in the IP to the top right text box (under IP), optional port specification defaults to 2112. hit connect, and if the specified IP is correct.
EX: lab1-20.eng.utah.edu
then the two lists will populate with users (left) and all updates coming into the server (right).
	To create, delete, change pass words of users, click the account managment button, and a new window will open. Enter the fields next to each 
button that you want to use, and click the corrisponding button.
	To Create or remove a Spreadsheet click the spreadsheet managment button from the main form and fill in the correlating feilds, and click the button.
Note: all 3 forms are able to be opened simultaniously. Closing the main form will disconnect from the server.

To shut down the server click the bottom right button labeled SHUT DOWN SERVER and click OK, the front end will terminate once the server has aknoledged the shut down request.
And after a short delay, the server will write all spreadsheets to a file, and terminate all connections.