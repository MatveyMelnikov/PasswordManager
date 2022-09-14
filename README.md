# PasswordManager
## Simple password manager with plugin system.

This is a small guide to using this application in the console and what's going on.

### Registration
In the event that the application does not find the login file (LoginDetails.dat) in the Data folder, the registration process will be started. 

You need to enter the first key word and the second one. You can think of it as a username and password. These key words will be used to encrypt all your data and will be required for further login. After entering the program will create a data folder, a login file and a data file (Data.dat).

### Menu
Menu output example:
```
--Menu--
1) Show note;
2) Add new note;
3) Delete note;
4) Change data file;
5) Set plugin;
--Notes (Data | EncryptionPluginExample)--
1) MySecretProfile
-Enter the command number:
```
The menu lists all commands with their numbers. In the line "--Notes (Data | EncryptionPluginExample)--" in parentheses, the first word is the name of the current file (Data), and the second is the current plugin used for hash generation and encryption (EncryptionPluginExample). The next line lists all notes in current file.

To call a command, enter its number.

### Commands
#### 1. Show note
Shows the content of the note you want.
Arguments:
* Note number
#### 2. Add new note
Creates a new note in the current file. If the file does not exist, it will be created and all current notes will be saved.
Arguments:
* Target of note - In fact, the title of the note. This may be the site for which these inputs are intended.
* First key word
* Second key word
#### 3. Delete note
Deletes a note by number.
Arguments:
* Note number
#### 4. Change data file
Gives a list of files (with .dat extension) located in the data folder. After selecting a file - sets it as current.
* File number
#### 5. Set plugin
Gives a list of plugins (files with .dll extension) located in the plugins folder. After selecting a plugin - sets it as current.
If any of the algorithms (hash algorithm or encryption algorithm) was not found, then it replaces it with the default algorithm. The name of this plugin, as the current one, will be displayed in any case.
* Plugin number
