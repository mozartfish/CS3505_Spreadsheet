TODO:






TESTING NEEDS DOING ONCE THE SERVER IS UP:
- able to recieve and decode all messages correctly
- make sure model updates correctly when getting information
- Form1, manUsers, SSman need to update when recieve update from network
- test shut down kills server, and allows user to enter new IP
- test server rude disconnecting
- test admin rude disconnectings effect on the server













ON USER CLICK THE EXIT BUTTON AT TOP RIGHT:
- the form must contact the server, and tell it that it is disconnecting then
  inform the welcome page to show, and kill form1

WHEN THE USER CLICKS THE CONNECT BUTTON IN THE WELCOME PAGE:
- Must make a connection with the server, and THEN should open a FORM1 by using ConnectSuccessful();

WHEN FORM1 IS OPENED:
- The lists should be populated with all the models info

SHUT DOWN BUTTON CLICKED:
- notify the server that it must shut down. Once the server echos the shut down message
  FORM1 informs the welcomePage to show itself, and kill Form1 to allow new server to be selected














HELP MENU!!! Last Thing Done!
- Aaron will do this :) Bruce just needs to do a confirmation near the end

















DONE TODAY:
First part of the shut down, now the network event should inform the form when shutdown, so that form can tell the welcome to kill form



































Algorithms problem 3:
G' should be the same G, but all the weights should be negative, making the shortest path, the longest.