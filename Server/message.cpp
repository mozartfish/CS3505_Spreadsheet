/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: 04/07/19
 *
 * Definitions for the message class
 */

#include <string>
#include "message.h"

/*
 * Constructor for an empty message with the defined type
 */
message::message(std::string type)
{
  this->type = type;
  this->code = -1;
  this->spreadsheets = NULL;
  this->dependencies = NULL;
  this->updates = NULL;
}

/*
 * Destructor for a message
 */
message::~message()
{
}
