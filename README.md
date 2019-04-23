# CS3505_Spreadsheet
CS3505 Multi-Client Spreadsheet
Authors: Tom Ady, Aaron Carlisle, Cole Jacobs, Joanna Lowry, Pranav Rajan, Bruce Nyguen

SERVER:
	To set up the server you must follow the following steps
	1) Change directory into Server
	2) Change directory into jsoncpp-master
	3) Run the program amalgamate.py by calling "python amalgamate.py"
		(This creates the needed files to make the server executable for JSON)
	4) Change back into the Server directory
	5) Run 'make' to execute the makefile
	6) Execute the server by calling ./Server_Exec

NOTES: Do not modify the sheets.txt file in the Settings directory. This will corrupt the saves users had for sheets
		Error messages will be printed when clients disconnect. Unlike other error messages (i.e. write back or read errors)
		these occur naturally, and cause no issue. Although considered an error, the error source is handled, and the server
		is notified

CLIENT:
	The client is expected to be used within Visual Studio. In order to operate the client, follow the following steps
	1) Select the Spreadsheet Client solution
	2) Build the solution
	3) Ensure that WelcomePage.csproj is selected as the Startup project
	4) Run the program


