/*
 *Tester for check whether json objects can be properly serialized and deserialized
 *Authors: Tom Ady, Pranav Rajan
 *Last Updated: April 13, 2019
 */

#include <iostream>
#include "message.h"
#include "helpers.h"
#include <string.h>
#include <string>
#include <vector>
#include <unordered_map>

using namespace std;

int main()
{
 string foo =  "{\"type\": \"open\", \"name\": \"cool.sprd\", \"username\": \"pajensen\", \"password\": \"Doofus\"}";
 server_helpers::message_to_json(foo);
 
  
  return 0;
}
