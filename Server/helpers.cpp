/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: April 6, 2019
 *
 * A list of static helper functions for the Server
 */

#include <string>
#include "helpers.h"
#include "message.h"
#include <string.h>
#include <vector>
#include <iostream> // for debugging purposes

using namespace std;

/*
 * Converts a message object into a sendable JSON string
 */
string server_helpers::message_to_json( const message & mess)
{
  string m_json;
  int idx;

  // All messages for comms have a type associated \
  m_json += '{';
  m_json += "\"type\": \"";
  m_json += mess.type;
  m_json += "\",";

  // Error message case
  if (mess.code >= 0)
    {
      m_json += "\"code\": " ;
      m_json += mess.code;
      m_json += ",";
      m_json += "\"source\": \"" + mess.source + "\"";
    }

  // Send spreadsheets case
  else if (mess.spreadsheets != NULL)
    {
      // Append all spreadsheet names in the message
      m_json += "\"spreadsheets\": [";
      for (idx = 0; idx < mess.spreadsheets->size() - 1; idx++)
        {
	  m_json += "\"" + (*mess.spreadsheets)[idx] + "\"";
	  if (idx < mess.spreadsheets->size() - 1)
	    m_json += ",";
	    
	}
    }

  // Cell update message
  else if (mess.updates != NULL)
    {
      m_json += "\"spreadsheet\": {";
      
      std::unordered_map<std::string, std::string>::iterator it = mess.updates->begin();
      for (it; it != mess.updates->end(); it++)
	{
	  m_json += "\"" + it->first + "\": ";
	  m_json += "\"" + it->second + "\"";
	  m_json += ",";
	}

      m_json[m_json.length()]  = '}';
    }
  
  m_json += '}';
  return m_json;
}

/*
 * Converts a JSON serialized string into a readable message object
 */
message & server_helpers::json_to_message(std::string & mess)
{
  
  char *pch;
  message *m;
  pch = strtok (&mess[0], "\""); // split on quotations

  //cout << "message separated into tokens" << endl;
  std::vector<std::string> message_tokens = std::vector<std::string>(); // vector for storing all the tokens in the message for processing
 
  while (pch != NULL)
  {
    std::string message_contents(pch);
    cout << message_contents << endl;
    message_tokens.push_back(message_contents);
    pch = strtok(NULL, "\"");
  }
  
  if (message_tokens[1] == "type")
  {
    //cout << "type index is correct" << endl;
    if (message_tokens[3] == "open")
    {
      // cout << "type keyword index is correct" << endl;
      m = new message("open");
      //cout << "created a new message" << endl;
      m->name = message_tokens[8];
      //cout << "set the name" << endl;
      m->username = message_tokens[13];
      //cout << "set the username" << endl;
      m->password = message_tokens[18];
      //cout << "set the password" << endl;
    }
    else if (message_tokens[4] == "edit")
    {
      m = new message("edit");
      m->cell = message_tokens[10];
      m->value = message_tokens[15];
      m->dependencies->push_back(message_tokens[24]);
      m->dependencies->push_back(message_tokens[27]);
    }
    else if (message_tokens[4] == "undo")
    {
      m = new message("undo");
    }
    else if (message_tokens[4] == "revert")
    {
      m = new message("revert");
      m->cell = message_tokens[10];
    }
  }
  else
  {
    return *new message("");
  }
}
