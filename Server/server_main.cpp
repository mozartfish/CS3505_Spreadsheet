/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last revision: 4/15/19
 *
 * The entry point for the server spreadsheet application
 */

#include <iostream>
#include <vector>
#include <future>
#include <mutex>
#include <string>
#include <unordered_map>
#include <queue>
#include <cstring>
#include <strings.h>
#include <fstream>
#include <chrono>
#include "jsoncpp-master/dist/json/json.h"
#include "spreadsheet.h"
#include "../Connection/socket_connections.h"

using namespace std;

// All necessary fields for running the server
unordered_map<string, spreadsheet*> * sheets;
vector<string> * sheet_names;
unordered_map<int, string> * socket_usermap;
unordered_map<int, string> * socket_sprdmap;
queue<string> * updates;
const string * SHEET_FILEPATH = new string ("./Settings/sheets.txt");


// Function declarations
void close(volatile socks & socket_info);
int process_spreadsheets_from_file();
int write_sheets_to_file();
void process_updates();
bool check_sprd(string spread_name, string user, string pass, int fd);



/*
 * Processes all spreadsheets from the Settings directory that should be
 * adjacent to this file, for use on server startup, assumes histories are well formed
 * Returns a non-zero number on an error
 */
int process_spreadsheets_from_file()
{
  ifstream file((*SHEET_FILEPATH));
  string line;

  vector<string> read_partitions;
  read_partitions.push_back("Name:");
  read_partitions.push_back("Usermap:");
  read_partitions.push_back("Spreadsheet_History:");
  read_partitions.push_back("Cell_Information:");

  // Read in spreadsheets
  if (file.is_open())
      while (getline(file, line))
	{
	  spreadsheet* curr_sheet = NULL;

	  // Pull out the first token
	  char * line_pointer = &line[0];
	  char * token = strtok(line_pointer, "\t");

	  // If no data after line
	  if (!token)
	    continue;

	  string * token_str = new string(token);

	  int part_idx = 0;

       /**********************************************************************/
       /*                       READ SHEET NAME                              */
       /**********************************************************************/

	  // Bad name error
	  if (*token_str != read_partitions[part_idx])
	    return -1;
	  

	  // Try to get name, make spreadsheet if valid, otherwise return err
	  token = strtok(NULL, "\t");
	  if (token == NULL)
	    return -1;
	  else
	    {
	      delete token_str;
	      token_str = new string (token);
	      curr_sheet = new spreadsheet(*token_str);
	      sheet_names->push_back(*token_str);
	    }
	  
	  ++part_idx;
	  token = strtok(NULL, "\t");

       /**********************************************************************/
       /*                       USERMAP READ BLOCK                           */
       /**********************************************************************/


	  // Try to parse Usermap (Can be empty)
	  if (token == NULL)
	    return -1;
	  else
	    {
	      token_str = new string (token);

	      // Bad partition
	      if (*token_str != read_partitions[part_idx])
		return -1;

	      part_idx++;

	      // Iterate over usermap until no tokens (error) or until next partition
	      while (token != NULL )
		{
		  // Get user
		  token = strtok(NULL, "\t");
		  if (token == NULL)
		    return -1;

		  string user(token);

		  // If 'user' is actually the next partition in the spreadsheet, break loop
		  if (user == read_partitions[part_idx])
		      break;

		  // Get pass
		  token = strtok(NULL, "\t");
		  if (token == NULL)
		    return -1;

		  string pass(token);

		  //Add user password pair to sheet
		  curr_sheet->add_user(user, pass);
		}
	    }

       /**********************************************************************/
       /*                       SHEET HISTORY READ                           */
       /**********************************************************************/

	  // Never reached next partition (aka Spreadsheet history)
	  if(token == NULL)
	    return -1;

	  part_idx++;

	  vector<string> * spd_hist = new vector<string>();

	  // Iterate over spreadsheet history
	  token = strtok(NULL, "\t");
	  while (token != NULL)
	    {
	      
	      token_str = new string (token);

	      // Make sure we have not reached next partition
	      if (*token_str == read_partitions[part_idx])
		break;

	      // add directly to spread history
	      spd_hist->push_back(*token_str);

	      token = strtok(NULL, "\t");
	    }

	  curr_sheet->add_direct_sheet_history(*spd_hist);

       /**********************************************************************/
       /*                       CELL HISTORY READ                            */
       /**********************************************************************/
	  
	  // Never reached next partition (aka Cell history)
	  if (token == NULL)
	    return -1;

	  token = strtok(NULL, "\t");

	  // Iterate over each token
	  while (token)
	    {
	      token_str = new string (token);
	     
	      //Parse cell num
	      int cell_num = stoi(token_str->substr(1, token_str->length() - 1));
	      vector<string> * cell_hist = new vector<string>();

	      token = strtok(NULL, "\t");

	      // Iterate over full history of the cell
	      while (token)
		{
		  token_str = new string(token);

		  // If found next cell
		  if ((*token_str)[0] == ':' && (*token_str)[token_str->length() - 1] == ':')
		    break;

		  // Add histories to a vector
		  cell_hist->push_back(*token_str);

		  token = strtok(NULL, "\t");
		}
	      
	      // Add vector of histories to spreadsheet
	      curr_sheet->add_direct_cell_history(cell_num, *cell_hist);
	      
	      } 

	  // Add finished sheet 
	  (*sheets)[curr_sheet->get_name()] = curr_sheet;
	}

  std::cout << "Finishing file read" << std::endl;
  
  // If could not open assumes there was no sheet to read
  file.close();
  return 0;
}






/*
 * Writes the state of all of the current spreadsheets to the specified SHEET_FILEPATH under a .txt file
 */
int write_sheets_to_file()
{
  ofstream file;
  file.open((*SHEET_FILEPATH), ofstream::out | ofstream::trunc);

  // Write all values, separated with tab characters (similar to tsv)
  if (file.is_open())
    {

      cout << "Writing spreadsheets back to file" << endl;

      unordered_map<string, spreadsheet*>::iterator sheet_it = sheets->begin();

      for (sheet_it; sheet_it != sheets->end(); sheet_it++)
	{
	  //Write name
	  file << "Name:" << '\t';
	  file << sheet_it->first << '\t';
	  
	  //Write users and passwords
	  file << "Usermap:" << '\t';
	  for (auto usermap : sheet_it->second->get_users())
	    file << usermap.first << '\t' << usermap.second << '\t';
	  
	  file << "Spreadsheet_History:" << '\t';
	  //Write spreadsheet history
	  for (auto sheet_hist : sheet_it->second->get_sheet_history())
	    {
	      file << sheet_hist << '\t';
	    }
	  
	  
	  //Write cells, and their dependencies, and their history
	  file << "Cell_Information:" << '\t';
	  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
	    {
	      // Write individual cell history
	      string cell = ":" + to_string(i) + ":";
	      file << cell;
	      vector<string> cell_hist = sheet_it->second->get_cell_history(i);
	      for (auto cell_contents : cell_hist)
		file  << '\t' << cell_contents;
	    }

	  //Separate spreadsheets by newline
	  file << '\n';
	}
    }
  //error opening file
  else
    return -1;

  file.close();
  return 0;
}








/*
 * Function that processes all messages contained in the queue field updates, and sends them out to all
 * necessary users
 */
void process_updates()
{
  while (updates->size() != 0)
    {
      // Get individual message
      string update = updates->front();
      updates->pop();
      
      // Get client ID
      char * token = strtok(&update[0], "\t");
      int fd = atoi(token);
      
      // Get the spreadsheet for the update
      token = strtok(NULL, "\t");
      string spread_name(token);
      spreadsheet * sheet = (*sheets)[spread_name];
      
      
      // Get the JSON Serialized update
      token = strtok(NULL, "\t");
      string serialized_update(token);
      
      //Deserialize
      Json::CharReaderBuilder des_builder;
      Json::CharReader * deserializer = des_builder.newCharReader();
      Json::Value deserialized;

      Json::Value send_back;
      
      // Try to read message
      if (!deserializer->parse(serialized_update.c_str(), serialized_update.c_str() + serialized_update.size(), &deserialized, NULL))
	{
	  cout << "Bad message received from socket " << fd << endl;
	  continue;
	}
      
      
      //Process update
      // Open
      if (deserialized["type"].asString() == "open")
	{
	  
	  
	  // Send the full spreadsheet to the client
	  if (check_sprd(deserialized["name"].asString(), deserialized["username"].asString(), deserialized["password"].asString(), fd))
	    {
	      
	    }

	  // Send error message for bad login
	  else
	    {
	      send_back["type"] = "error";
	      send_back["code"] = 1;
	      send_back["source"] = "";

	      //TODO SEND
	    }
	  
	}
      
      // Edit
      else if (deserialized["type"].asString() == "edit")
	{
	  vector<string> * dependencies = new vector<string>();

	  // Add each dependency from json to a string vector
	  for (int i = 0; i < deserialized["dependencies"].size(); i++)
	    dependencies->push_back(deserialized["dependencies"][i].asString());
	  
	  // Try to change cell
	  if(sheet->change_cell(deserialized["cell"].asString(), deserialized["value"].asString(), dependencies))
	    {
	      
	    }
	  
	  // If there is a failure
	  else
	    {
	      send_back["type"] = "error";
	      send_back["code"] = 2;
	      send_back["source"] = deserialized["cell"].asString();

	      //TODO SEND
	    }
	}
      
      // Undo
      else if (deserialized["type"].asString() == "undo")
	{
	  string result = sheet->undo();
	  // Nothing needed for an empty undo
	  if (result == "")
	    continue;

	  
	}
      
      // Revert
      else if (deserialized["type"].asString() == "revert")
	{
	  string result = sheet->revert(deserialized["cell"].asString());

	  // Good revert
	  if (result != "\t")
	    {
	      

	    }

	  // Result returned \t, error character (choice was arbitrary)
	  else
	    {
	      send_back["type"] = "error";
	      send_back["code"] = 2;
	      send_back["source"] = deserialized["cell"].asString();

	      //TODO SEND
	    }
	}
	     
      // Send updates to all that should be notified if successful
    }
}

/*
 * Closes the server, sending all necessary goodbyes, processes the rest of
 * the incoming requests, closes all socket connections, deletes all
 * pointer objects, and writes all spreadsheets back to the file in Settings
 */
void close(volatile socks & socket_info)
{

  //Wait 7 seconds for data to finish coming in
  auto begin = std::chrono::steady_clock::now();
  while (std::chrono::duration_cast<std::chrono::seconds>(std::chrono::steady_clock::now() - begin).count() < 7);

  //Clear queue and process messages
  process_updates();

  //Close sockets
  for (int socket : *socket_info.sockets)
    socket_connections::CloseSocket(socket);

  //Write the sheets to the file
  if (!write_sheets_to_file())
    cout << "Error writing back spreadsheets, last version will be saved instead" << endl;

  //Delete all pointers, and all spreadsheets in the list
  for (unordered_map<string, spreadsheet*>::iterator it = sheets->begin(); it != sheets->end(); it++)
    delete it->second;
  delete sheets;
  delete sheet_names;
  delete socket_usermap;
  delete socket_sprdmap;
  delete updates;
  delete SHEET_FILEPATH;
}


/*
 * Opens/creates a spreadsheet with the given spreadsheet name
 * username and password, returns true if the spreadsheet did not
 * exist or if the user password combo was correct, or if there
 * was no user of the provided name associated with the spreadsheet
 */
bool check_sprd(string spread_name, string user, string pass, int fd)
{
  //Create new spreadsheet if nonexistant
  if (sheets->find(spread_name) == sheets->end())
    {
      sheet_names->push_back(spread_name);
      (*sheets)[spread_name] = new spreadsheet(spread_name);
      (*sheets)[spread_name]->add_user(user, pass);
      (*sheets)[spread_name]->add_listener(fd);
      return true;
    }
  
  // Access spreadsheet
  else
    {
      unordered_map<string,string> sprd_users = (*sheets)[spread_name]->get_users();
      
      // User didn't exist
      if (sprd_users.find(user) == sprd_users.end())
	{
	  (*sheets)[spread_name]->add_user(user, pass);
	  (*sheets)[spread_name]->add_listener(fd);
	  return true;
	}
      else if (sprd_users[user] == pass)
	{
	  (*sheets)[spread_name]->add_listener(fd);
	  return true;
	}
      else
	return false;
    }
}

/*
 * The main entry point for the program, launches the Server
 */
int main(int argc, char ** argv)
{
  std::cout << "Launching server" << std::endl;
  
  // Initialize the struct of sockets
  volatile  socks connections;
  connections.sockets = new std::vector<int>();
  connections.continue_listening = true;
  connections.size_before_update = 0;
  connections.new_socket_connected = false;
  connections.buffers = new std::vector<char*>();
  connections.partial_data = new vector<string*>();
  connections.needs_removed = new vector<bool>();

  // Initialize a lock that gets passed around for when updating 
  // the socks struct or accessing its members
  std::mutex lock;

  // Initialize all fields needed for running the server
  sheets = new unordered_map<string, spreadsheet*> ();
  sheet_names = new vector<string>();
  socket_usermap = new unordered_map<int, string>();
  socket_sprdmap = new unordered_map<int, string>();
  updates = new queue<string>();

  //Start by reading the file of spreadsheets into the program
  if (process_spreadsheets_from_file() < 0)
    {
      cout << "Error reading spreadsheet file. Check for syntax errors." << endl;
      return -1;
    }

  // Start the "timer" (just initializes value to current time) for updating
  auto update_timer = std::chrono::steady_clock::now();

  //Listen for clients async
  auto x = std::async(std::launch::async, socket_connections::WaitForClientConnections, &connections, &lock);
  
  //Infinite send and receive loop
   while(true)
    {

      /**********************************************************************/
      /*                         NEW CLIENT BLOCK                           */
      /**********************************************************************/
      lock.lock();
       if (connections.new_socket_connected)
      	{

      	  std::cout << "A new client has connected" << std::endl;
	  
	  // Get all spreadsheets as a json object
	  Json::Value json_sheets;
	  string type("list");
	  json_sheets["type"] = type;
	  json_sheets.append("spreadsheets");
	  for (string ind_sheet : *sheet_names)
	    json_sheets["spreadsheets"].append(ind_sheet);


	  // Iterate over each new client, and send required start data, and wait for data from them
	  for (int idx = connections.size_before_update; idx < connections.sockets->size(); idx++)
	    {
	      cout << "Buffer section" << endl;
	      // Add new buffers for getting data
              connections.buffers->push_back(new char[BUF_SIZE]);
	      bzero((*connections.buffers)[idx - 1], BUF_SIZE);
	      connections.partial_data->push_back(new string());
	      connections.needs_removed->push_back(false);

	      
	      // Send list of spreadsheet names
	      socket_connections::SendData((*connections.sockets)[idx], json_sheets.asCString(), (int)json_sheets.asString().size());

	      cout << "Data wait section" << endl;
	      std::async(std::launch::async, socket_connections::WaitForData,
			 (*connections.sockets)[idx],  (*connections.buffers)[idx - 1], BUF_SIZE);

	      cout << "Timer section" << endl;
	      std::async(std::launch::async, socket_connections::WaitForDataTimer,
			 (*connections.buffers)[idx - 1], idx - 1, &lock, connections.needs_removed);
			 
	    }
	  connections.new_socket_connected = false;
	  connections.size_before_update = connections.sockets->size();
      	}
       lock.unlock();

       /**********************************************************************/
       /*                     CLIENT REMOVAL BLOCK                           */
       /**********************************************************************/
       lock.lock();

       int it_idx = 0;
       vector<bool>::iterator it = connections.needs_removed->begin();
       while (it != connections.needs_removed->end())
	 {
	   cout << "Client removal section" << endl;
	   // If a client needs removed, remove it
	   if (*it)
	     {
	       // Make sure double erasure doesn't happen
	       connections.needs_removed->erase(it++);

	       // Erase the socket from the usermap and sheet map, and remove it from the spreadsheet listeners
	       int fd = (*connections.sockets)[it_idx + 1];
	       string sheet_asso = (*socket_sprdmap)[fd];
	       (*sheets)[sheet_asso]->remove_listener(fd);
	       socket_usermap->erase(fd);
	       socket_sprdmap->erase(fd);

	       // Close the socket
	       socket_connections::CloseSocket(fd);
	       
	       // Erase the socket from the connections struct
	       connections.sockets->erase(connections.sockets->begin() + it_idx + 1);
	       --(connections.size_before_update);
	       connections.buffers->erase(connections.buffers->begin() + it_idx);
	       connections.partial_data->erase(connections.partial_data->begin() + it_idx);
	       
	       it_idx++;
	     }
	 }
       lock.unlock();

       
       /**********************************************************************/
       /*                       DATA RECEIVE BLOCK                           */
       /**********************************************************************/
       lock.lock();
       
       // Get data from buffers
       for (int idx = 0; idx < connections.buffers->size(); idx++)
	 {
	   // If data exists, grab and reset buffer (If not chars are null)
	   if ((*connections.buffers)[idx][0] > 0)
	     {
	       (*(*connections.partial_data)[idx]) += (*connections.buffers)[idx];
	       (*connections.buffers)[idx] = new char[BUF_SIZE];
	       bzero((*connections.buffers)[idx], BUF_SIZE);
	     

	       // Resume getting data
	       std::async(std::launch::async, socket_connections::WaitForData,
		      (*connections.sockets)[idx + 1],  (*connections.buffers)[idx], BUF_SIZE);

	       std::async(std::launch::async, socket_connections::WaitForDataTimer,
			 (*connections.buffers)[idx], idx, &lock, connections.needs_removed);
	     }

	   // Process data
	   string delimiter = "\n\n";
	   int limit = (*connections.partial_data)[idx]->find(delimiter);

	   // If delemiter doesn't exist, data is partial
	   if (limit < 0)
	     continue;

	   //Get new data, erase data
	   string  data = (*connections.partial_data)[idx]->substr(0, limit);
	   (*connections.partial_data)[idx]->erase(0, limit + 1);

	   // Format an update as socket_num, sheet_name, and then the data, separated with \t
	   string * update = new string();
	   *update += to_string((*connections.sockets)[idx + 1]) + '\t';
	   *update += (*socket_sprdmap)[(*connections.sockets)[idx + 1]] + '\t';
	   *update += data;
	   
	   // Add data to the queue
	   updates->push(*update);
	   
	 }

       lock.unlock();

       /**********************************************************************/
       /*                         UPDATE PROCESSING                          */
       /**********************************************************************/

       lock.lock();

       process_updates();

       lock.unlock();

       
       // Write back to file every x minutes
       if (std::chrono::duration_cast<std::chrono::minutes>(std::chrono::steady_clock::now() - update_timer).count() > 10)
	 {
	   lock.lock();
	   if (write_sheets_to_file() < 0)
	     cout << "Error writing sheets back to file" << endl;
	   lock.unlock();
	   update_timer = std::chrono::steady_clock::now();
	 }
       

    }

   close(connections);
   return 0;
}
