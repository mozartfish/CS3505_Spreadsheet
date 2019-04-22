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



ADMIN:
- Skeleton of Events (2 hrs)


DONE
March 21-March 23, 2019
Everyone - Voted for the Send It Protocol
Pranav - Set up Discord and the github repository for the assignment
- Join the Discord Group and get everyone connected to the github repository 
Aaron - Decide on the 3500 spreadsheet project that we are going to use for the multi-client spreadsheet
- Decide who is going to work on which part of the spreadsheet SERVER: Tom & Pranav CLIENT: Cole & Joanna ADMIN: Aaron & Bruce
- Using what we know about the Protocol begin thinking about design for the client and the server

April 1:
- Rough draft of GUI (4 hrs)
- Design top protocol, for messages to server (1 hr)
- Use document (1 hr)
