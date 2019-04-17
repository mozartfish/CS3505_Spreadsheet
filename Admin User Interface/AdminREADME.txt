TODO:

ON USER CLICK THE EXIT BUTTON AT TOP RIGHT:
- the form must contact the server, and tell it that it is disconnecting then
  inform the welcome page via event that the form is closing, so that welcom page can call use KillProgram();

WHEN THE USER CLICKS THE CONNECT BUTTON IN THE WELCOME PAGE:
- Must make a connection with the server, and THEN should open a FORM1 by using ConnectSuccessful();

WHEN FORM1 IS OPENED:
- The lists should be populated with all the models info

SHUT DOWN BUTTON CLICKED:
- notify the server that it must shut down. Once the server echos the shut down message
  FORM1 informs the welcomePage to KillProgram();  (Event may be reused from the Form1 clicking the exit button at top right)




