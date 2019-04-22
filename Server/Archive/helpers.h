/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: April 6, 2019
 *
 * A list of static helper functions for the Server (declarations only)
 */

#include <string>
#include "message.h"

#ifndef HELP
#define HELP

class server_helpers 
{

 public:
  static std::string message_to_json(const message & mess);
  static message & json_to_message(std::string & mess);
};

#endif
