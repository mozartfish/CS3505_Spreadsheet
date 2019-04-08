/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last Revision: April 6, 2019
 *
 * Declarations for message objects for sending between servers and clients
 */

#include <string>
#include <vector>
#include <unordered_map>


#ifndef MESSAGE
#define MESSAGE

class message {
 public:
  message(std::string type);
  ~message();
  
  std::string type;
  
  int code;
  std::string source;
  
  std::vector<std::string> * spreadsheets;
  
  std::string cell;
  std::string value;
  std::vector<std::string> * dependencies;
  
  std::string name;
  std::string username;
  std::string password;

  std::unordered_map<std::string, std::string> * updates;
  
};

#endif
