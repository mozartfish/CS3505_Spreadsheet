/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: April 6, 2019
 *
 * A list of static helper functions for the Server
 */

#include <string>
#include "helpers.h"
#include "message.h"

using namespace std;

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
      
    }
  
  m_json += '}';
  return m_json;
}


message & server_helpers::json_to_message(const std::string & mess)
{

}
