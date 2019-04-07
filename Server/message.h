/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: April 6, 2019
 *
 * Declarations for message objects for sending between servers and clients
 */

#include <string>

#ifndef MESSAGE
#define MESSAGE

class message {
public:
message(std::string type);
~message();

std::string code;
std::string source;

std::string * spreadsheets;

std::string cell;
std::string value;
std::string * dependencies;

std::string name;
std::string username;
std::string password;

};

#endif
