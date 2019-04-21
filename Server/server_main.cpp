/*
 * Authors: Thomas Ady and Pranav Rajan
 * Last revision: 4/15/19
 *
 * The entry point for the server spreadsheet application
 */

#include <iostream>
#include <vector>
#include <future>
#include <thread>
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
unordered_map<int, string> * socket_usermap;
unordered_map<int, string> * socket_sprdmap;
unordered_set<int> * admins;
queue<string> * updates;
const string * SHEET_FILEPATH = new string ("./Settings/sheets.txt");

bool continue_to_run = true;


// Function declarations
void close(volatile socks & socket_info);
int process_spreadsheets_from_file();
int write_sheets_to_file();
void process_updates(volatile socks * socks_list);
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

	  curr_sheet->add_direct_sheet_history(spd_hist);

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
	      curr_sheet->add_direct_cell_history(cell_num, cell_hist);
	      
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
	  for (auto sheet_hist : *(sheet_it->second->get_sheet_history()))
	    {
	      file << sheet_hist << '\t';
	    }
	  
	  
	  //Write cells, and their dependencies, and their history
	  file << "Cell_Information:" << '\t';
	  for (int i = 0; i < DEFAULT_CELL_COUNT; i++)
	    {
	      // Write individual cell history
	      string cell = ":" + to_string(i) + ":";
	      
	      vector<string> * cell_hist = sheet_it->second->get_cell_history(i);

	      if (cell_hist->size() == 0)
		continue;

	      file << cell;

	      for (auto cell_contents : *cell_hist)
		file  << '\t' << cell_contents;

	      // If not the last cell
	      if (i < DEFAULT_CELL_COUNT - 1)
		file << '\t';
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
void process_updates(volatile socks * socks_list)
{
  

  Json::CharReaderBuilder des_builder;
  Json::CharReader * deserializer = des_builder.newCharReader();

  while (updates->size() != 0)
    {

      // Get individual message
      string update = updates->front();
      cout << "update: " << update << endl;
      updates->pop();

      vector<char *> message_split;
      
      // Get client ID
      char * token = strtok(&update[0], "\t");
      while (token != NULL)
	{
	  message_split.push_back(token);
	  token = strtok(NULL, "\t");
	}

      // Message should always have the socket at 0
      int fd = atoi(message_split[0]);
      cout << fd << endl;
      
      // Get the spreadsheet for the update (if there is one)
      string spread_name;
      spreadsheet * sheet;

      // Only get sheet if a name was specified
      if (message_split.size() > 2)
	{
	  spread_name += message_split[1];
	  
	  // Don't modify nonexistant servers
	  if (sheets->find(spread_name) == sheets->end())
	    continue;

	  sheet = (*sheets)[spread_name];
	  
	}
      
      // Get the JSON Serialized update
      string serialized_update(message_split[message_split.size() - 1]);
      cout << serialized_update << endl;
      
      //Deserialize
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
	  send_back["type"] = "full send";
	  
	  // Send the full spreadsheet to the client
	  if (check_sprd(deserialized["name"].asString(), deserialized["username"].asString(), deserialized["password"].asString(), fd))
	    {

	      
	      // Get every cell with data
	      for (char c = 'A'; c <= 'Z'; c++)
		for (int i = 1; i <= 99; i++)
		  {
		    // Get current cell
		    string cell(1, c);
		    cell += to_string(i);
		    
		    string contents = (*sheets)[deserialized["name"].asString()]->get_cell_contents(cell);

		    // Otherwise add contents to the list
		    send_back["spreadsheet"][cell] = contents;
		  }
	      
	      string send_back_str = send_back.toStyledString()  + "\n\n";

	      //Send error back only to client that sent the bad request
	      socket_connections::SendData(fd, send_back_str.c_str(), send_back_str.size());
	      continue;
	    }

	  // Send error message for bad login
	  else
	    {
	      send_back["type"] = "error";
	      send_back["code"] = 1;
	      send_back["source"] = "";

	      string send_back_str = send_back.toStyledString()  + "\n\n";

	      //Send error back only to client that sent the bad request
	      socket_connections::SendData(fd, send_back_str.c_str(), send_back_str.size());
	      continue;
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
	      
	      send_back["type"] = "full send";
	      send_back["spreadsheet"][deserialized["cell"].asString()] = deserialized["value"].asString();

	      // Let all admins know of the update
	      if (admins->size() > 0)
		{
		  Json::Value admin_mess;
		  admin_mess["type"] = "SS";
		  admin_mess["name"] = spread_name;
		  admin_mess["status"] = 0;

		  string admin_str = admin_mess.toStyledString()  + "\n\n";
		  const char * admin_c = admin_str.c_str();
		  
		  for (int admin : *admins)
		    socket_connections::SendData(admin, admin_c, admin_str.size());
		}
	    }
	  
	  // If there is a failure
	  else
	    {
	      cout << "circ dep" << endl;
	      send_back["type"] = "error";
	      send_back["code"] = 2;
	      send_back["source"] = deserialized["cell"].asString();

	      string send_back_str = send_back.toStyledString()  + "\n\n";

	      //Send error back only to client that sent the bad request
	      socket_connections::SendData(fd, send_back_str.c_str(), send_back_str.size());
	      continue;
	    }
	}
      
      // Undo
      else if (deserialized["type"].asString() == "undo")
	{
	  string result = sheet->undo();
	  // Nothing needed for an empty undo
	  if (result == "")
	    continue;

	  int idx = result.find('\t');
	  string cell = result.substr(0, idx);
	  string contents = result.substr(idx + 1);

	  send_back["type"] = "full send";
	  send_back["spreadsheet"][cell] = contents;
	  
	  // Let all admins know of the update
	      if (admins->size() > 0)
		{
		  Json::Value admin_mess;
		  admin_mess["type"] = "SS";
		  admin_mess["name"] = spread_name;
		  admin_mess["status"] = 0;

		  string admin_str = admin_mess.toStyledString()  + "\n\n";
		  const char * admin_c = admin_str.c_str();
		  
		  for (int admin : *admins)
		    socket_connections::SendData(admin, admin_c, admin_str.size());
		}
	}
      
      // Revert
      else if (deserialized["type"].asString() == "revert")
	{
	  string result = sheet->revert(deserialized["cell"].asString());

	  // Good revert
	  if (result != "\t")
	    {
	      send_back["type"] = "full send";
	      send_back["spreadsheet"][deserialized["cell"].asString()] = result;

	      // Let all admins know of the update
	      if (admins->size() > 0)
		{
		  Json::Value admin_mess;
		  admin_mess["type"] = "SS";
		  admin_mess["name"] = spread_name;
		  admin_mess["status"] = 0;

		  string admin_str = admin_mess.toStyledString()  + "\n\n";
		  const char * admin_c = admin_str.c_str();
		  
		  for (int admin : *admins)
		    socket_connections::SendData(admin, admin_c, admin_str.size());
		}
	    }

	  // Result returned \t, error character (choice was arbitrary)
	  else
	    {
	      cout << "circ dep" << endl;
	      send_back["type"] = "error";
	      send_back["code"] = 2;
	      send_back["source"] = deserialized["cell"].asString();

	      string send_back_str = send_back.toStyledString() + "\n\n";

	      //Send error back only to client that sent the bad request
	      socket_connections::SendData(fd, send_back_str.c_str(), send_back_str.size());
	      continue;
	    }
	}
      
      // Admin connect message
      else if (deserialized["type"].asString() == "admin")
	{
	  // Add the socket to the list of admins
	  admins->insert(fd);

	  //Send the admin all sheets with all of their users
	  for (auto sheet_pair : *sheets)
	    {
	      Json::Value ad_mess;
	      ad_mess["type"] = "SS";
	      ad_mess["SSname"] = sheet_pair.second->get_name();

	      for (auto user_pair : sheet_pair.second->get_users())
		{
		  ad_mess["users"][user_pair.first] = user_pair.second;
		}

	      string send_back_str = ad_mess.toStyledString() + "\n\n";
	      const char * admin_c = send_back_str.c_str();

	      socket_connections::SendData(fd, admin_c, send_back_str.size());
	    }

	  continue;
	}

      // Admin shutdown
      else if (deserialized["type"].asString() == "shutdown")
	{
	  // Just let the server know to shutdown on the next loop, and echo the message back to the admin
	  continue_to_run = false;

	  string send_back_str = deserialized.toStyledString() + "\n\n";
	  const char * admin_c = send_back_str.c_str();

	  // Let all admins know of the shutdown
	  for (int admin : *admins)
	    socket_connections::SendData(admin, admin_c, send_back_str.size());

	  continue;
	}


      // Admin spreadsheet message (request create, status, or delete)
      else if (deserialized["type"].asString() == "SS")
	{
	  cout << "sheet mess" << endl;
	  string sheet_name = deserialized["ssName"].asString();
	  int status = deserialized["status"].asInt();
	  cout << status << endl;

	  // Delete sheet
	  if (status == -1 && sheets->find(sheet_name) != sheets->end())
	    {
	      //For now force close all clients, should improve to use the bool they have
	      for (int listener : (*sheets)[sheet_name]->get_listeners())
		(*(socks_list->needs_removed))[listener] = true;
	      
	      delete (*sheets)[sheet_name];
	      sheets->erase(sheet_name);
	    }

	  // Add sheet
	  else if (status == 1 && sheets->find(sheet_name) == sheets->end())
	    {
	      (*sheets)[sheet_name] = new spreadsheet(sheet_name);
	      cout << "sheet made" << sheets->size() << endl;
	    }

	  string send_back_str = deserialized.toStyledString() + "\n\n";
	  const char * admin_c = send_back_str.c_str();

	  cout << send_back_str << endl;

	  // Let all admins know of the shutdown
	  for (int admin : *admins)
	    socket_connections::SendData(admin, admin_c, send_back_str.size());

	  continue;
	}

      // Admin spreadsheet message (request create, change, or delete)
      else if (deserialized["type"].asString() == "User")
	{
	  cout << "User message" << endl;
	  int status = deserialized["status"].asInt();
	  string user = deserialized["username"].asString();
	  string pass = deserialized["pass"].asString();
	  string sheet_name = deserialized["workingOn"].asString();

	  // Don't do anything for user message with nonexistant sheet
	  if (sheets->find(sheet_name) == sheets->end())
	    continue;

	  spreadsheet * sheet = (*sheets)[sheet_name];

	  // Delete user
	  if (status == -1)
	    {
	      sheet->remove_user(user);
	    }

	  // Change user password
	  else if (status == 0)
	    {
	      sheet->change_user(user, pass);
	    }

	  // Add user
	  else if (status == 1)
	    {
	      sheet->add_user(user, pass);
	    }

	  string send_back_str = deserialized.toStyledString() + "\n\n";
	  const char * admin_c = send_back_str.c_str();

	  // Let all admins know of the shutdown
	  for (int admin : *admins)
	    socket_connections::SendData(admin, admin_c, send_back_str.size());

	  continue;
	}


      string send_back_str = send_back.toStyledString()  + "\n\n";
      const char * send_back_c = send_back_str.c_str();
	     
      // Send updates to all that should be notified if successful
      for (int client : (*sheets)[spread_name]->get_listeners())
	{
	  if (!(*(socks_list->needs_removed))[client])
	    socket_connections::SendData(client, send_back_c, send_back_str.size());
	}
    }

  delete deserializer;
}

/*
 * Closes the server, sending all necessary goodbyes, processes the rest of
 * the incoming requests, closes all socket connections, deletes all
 * pointer objects, and writes all spreadsheets back to the file in Settings
 */
void close(volatile socks & socket_info)
{

  cout << "Preparing to Shut Down the Server" << endl;

  //Wait 7 seconds for data to finish coming in
  auto begin = std::chrono::steady_clock::now();
  while (std::chrono::duration_cast<std::chrono::seconds>(std::chrono::steady_clock::now() - begin).count() < 7);

  //Clear queue and process messages
  process_updates(&socket_info);

  //Close sockets
  for (int socket : *socket_info.sockets)
    socket_connections::CloseSocket(socket);

  //Write the sheets to the file
  if (write_sheets_to_file() < 0)
    cout << "Error writing back spreadsheets, last version will be saved instead" << endl;

  //Delete all pointers, and all spreadsheets in the list
  for (unordered_map<string, spreadsheet*>::iterator it = sheets->begin(); it != sheets->end(); it++)
    delete it->second;
  sheets->clear();
  delete sheets;

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
      (*sheets)[spread_name] = new spreadsheet(spread_name);
      (*sheets)[spread_name]->add_user(user, pass);
      (*sheets)[spread_name]->add_listener(fd);
      (*socket_sprdmap)[fd] = spread_name;
      (*socket_usermap)[fd] = user;
      
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
	  (*socket_sprdmap)[fd] = spread_name;
	  (*socket_usermap)[fd] = user;
	  return true;
	}
      else if (sprd_users[user] == pass)
	{
	  (*sheets)[spread_name]->add_listener(fd);
	  (*socket_sprdmap)[fd] = spread_name;
	  (*socket_usermap)[fd] = user;
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
  connections.needs_removed = new unordered_map<int, bool>();

  // Initialize a lock that gets passed around for when updating 
  // the socks struct or accessing its members
  std::mutex lock;

  // Initialize all fields needed for running the server
  sheets = new unordered_map<string, spreadsheet*> ();
  socket_usermap = new unordered_map<int, string>();
  socket_sprdmap = new unordered_map<int, string>();
  admins = new unordered_set<int>();
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
  thread(socket_connections::WaitForClientConnections, &connections, &lock, &continue_to_run).detach();
  
  //Infinite send and receive loop
   while(continue_to_run)
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
	  json_sheets["spreadsheets"] = Json::Value(Json::arrayValue);
	  Json::Value * j_sheets = &json_sheets["spreadsheets"];
	  for (auto it : *sheets)
	    j_sheets->append(it.first);

	  string message = json_sheets.toStyledString()  + "\n\n";
	  const char * mess_c = message.c_str();

	  int size = connections.sockets->size();
	  cout << message << endl;

	  // Iterate over each new client, and send required start data, and wait for data from them
	  for (int idx = connections.size_before_update; idx < size; idx++)
	    {
	      cout << idx << endl;
	      cout << connections.buffers->size() << endl;
	      cout << connections.partial_data->size() << endl;
	      cout << connections.needs_removed->size() << endl;
	      cout << connections.size_before_update << endl;
	      // Add new buffers for getting data
              connections.buffers->push_back(new char[BUF_SIZE]);
	      bzero((*connections.buffers)[idx - 1], BUF_SIZE);
	      connections.partial_data->push_back(new string());
	      (*connections.needs_removed)[(*connections.sockets)[idx]] = false;;

	      
	      // Send list of spreadsheet names
	      socket_connections::SendData((*connections.sockets)[idx], mess_c, message.size());

	      // Wait for data and run the timeout timer
	      thread(socket_connections::WaitForData, (*connections.sockets)[idx],  
		     (*connections.buffers)[idx - 1], BUF_SIZE, &lock, &(*connections.needs_removed)).detach();

	    }
	  
	  connections.new_socket_connected = false;
	  connections.size_before_update = connections.sockets->size();
	 
	  
	  cout << connections.buffers->size() << endl;
	  cout << connections.partial_data->size() << endl;
	  cout << connections.needs_removed->size() << endl;
	  cout << connections.size_before_update << endl;
      	}
       lock.unlock();
       

       /**********************************************************************/
       /*                     CLIENT REMOVAL BLOCK                           */
       /**********************************************************************/
       lock.lock();
       
       if (connections.sockets->size() > 1)
	 {
	   vector<int>::iterator it = connections.sockets->begin() + 1;
	   int it_idx = 0;
	   while (it != connections.sockets->end())
	     {
	       int fd = *it;
	       // If a client needs removed, remove it
	       if ((*connections.needs_removed)[fd])
		 {
		   cout << "Removing client from socket " << fd << endl;

		   cout << connections.buffers->size() << endl;
		   cout << connections.partial_data->size() << endl;
		   cout << connections.needs_removed->size() << endl;
		   cout << connections.size_before_update << endl;

		   // Erase the socket from the usermap and sheet map, and remove it from the spreadsheet listeners 
		   // Remove client if associated with spreadsheet
		   if ((*socket_sprdmap).count(fd) > 0)
		     {
		       string sheet_asso = (*socket_sprdmap)[fd];
		       
		       // Handles case of spreadsheet was deleted mid connect
		       if (sheets->find(sheet_asso) != sheets->end())
			 (*sheets)[sheet_asso]->remove_listener(fd);
		       
		       socket_usermap->erase(fd);
		       socket_sprdmap->erase(fd);
		     }
		   
		   // Close the socket
		   socket_connections::CloseSocket(fd);

		   cout << "socket closed for " << fd << endl;
		   
		   // Erase the socket from the connections struct
		   it = connections.sockets->erase(it);
		   --(connections.size_before_update);
		   connections.buffers->erase(connections.buffers->begin() + it_idx - 1);
		   connections.partial_data->erase(connections.partial_data->begin() + it_idx - 1);

		   cout << "data erased for " << fd << endl;
		   
		   // Make sure double erasure doesn't happen
		   connections.needs_removed->erase(fd);
		   
		   cout << "erased bool " << fd << endl;

		   cout << connections.buffers->size() << endl;
		   cout << connections.partial_data->size() << endl;
		   cout << connections.needs_removed->size() << endl;
		   cout << connections.size_before_update << endl;
		 }
	       else
		 ++it;
	       
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
	       thread(socket_connections::WaitForData, (*connections.sockets)[idx + 1],  (*connections.buffers)[idx], 
		      BUF_SIZE, &lock, &(*connections.needs_removed)).detach();
	     }

	   // Process data
	   string delimiter = "\n\n";
	   int limit = (*connections.partial_data)[idx]->find(delimiter);

	   // If delemiter doesn't exist, data is partial
	   if (limit < 0)
	     continue;

	   //Get new data, erase data
	   string  data = (*connections.partial_data)[idx]->substr(0, limit);
	   (*connections.partial_data)[idx]->erase(0, limit + 2);

	   // Format an update as socket_num, sheet_name, and then the data, separated with \t
	   string * update = new string();
	   *update += to_string((*connections.sockets)[idx + 1]) + '\t';

	   if ((*socket_sprdmap).find((*connections.sockets)[idx + 1]) != socket_sprdmap->end())
	     *update += (*socket_sprdmap)[(*connections.sockets)[idx + 1]];

	   *update += '\t';
	   *update += data;
	   
	   // Add data to the queue
	   updates->push(*update);
	   
	 }

       lock.unlock();

       /**********************************************************************/
       /*                         UPDATE PROCESSING                          */
       /**********************************************************************/

       lock.lock();

       process_updates(&connections);

       lock.unlock();

       
       // Write back to file every x minutes
       if (std::chrono::duration_cast<std::chrono::minutes>(std::chrono::steady_clock::now() - update_timer).count() > 1)
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
