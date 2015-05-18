# 7-Days-to-Die-Dedicated-Server-Configuration-File-Manager
A WPF utility application for managing the XML files associated with the 7 Days to Die dedicated server


DISCLAIMER-
Neither this application nor the developer is in any way associated with 7 Days to Die. 
While the application has been designed to deal with your sensitive server files reliably,
it is USE AT YOUR OWN RISK and I cannot be held responsible for any ill effects that come from using it.
This would likely be most notable if the developers were to remove settings that if present cause the server to crash,
or were to alter the allowed values of a setting to where presenting the older value causes the server to crash.
In either case, manually editing the setting should restore the server functionality.
I use the program myself and will update it if I see any oddities as future server versions come out.


Instructions:

Using Seven Days Configuration UI is meant to be simple. The idea is that you load up your configuration file and edit your settings through an informative UI, then save when it is all correct. The XML write is done in such a way that any regrouping or reordering you might have done should be fine and preserved when the file is rewritten.

If at any time you have questions on what an item does, hover your mouse over it. A tool tip will show with the original developer text as well as any (mostly) helpful information I had to offer, whenever possible. Note that some of the tool tip descriptions may describe settings that I have improved upon for the UI. For instance, you may see something like 0 = easy, but also see 'Easy' in the drop down list and not '0'. This is because I am doing the interpretation behind the scenes, but wanted to stay true to the idea of showing the original developer text.

Note that the application will automatically remember the last files you've looked up (browse). If 'n/a' appears, no file path has yet been set. If a message other than the file path or 'n/a' appears, the application was not able to resolve the file path you are saving, or the one you have saved.

The UI controls are mostly self-validating. In the case where free text is allowed, and the value is required, a dialog is shown warning you, the field will be highlighted in red, and you will not be able to save until the error is corrected.

Any time a file is saved, it is then reloaded into the UI. The message in the status does not reflect the reload. This is intentional. 

The Tabs

Note - No changes will ever effect the files until the user clicks save at the top of the UI. They may move through the tabs freely, and even view steam profiles (from within the app ;)) without losing your work. However, most of the saves to the user settings are done silently, though if appropriate, will notify the user in the status bar when they occur.

Configuration:
This set of tabs is pretty straight forward. The UI elements correspond to a property of the selected configuration file. Direct binding of the ui elements to the source data ensures validation. In code, all abstracted values, such as 0 = easy are handled by enumeration. These enumerations contain a directive that adds a property 'description', which can be used as either the xml value, or simply the select box display value. This was pretty handy for processing and should be easy to adapt if values change in updates.

The tab has two sub-tabs that group the configuration file properties into two sections. Those that apply to the server such as what your telnet password is, and those that apply to the game, like how long is day.

Administration:
The operation of this section is a little less straight forward. The system is simple, but requires some knowledge of how it works to get the most from it.

First, there is a finite list of commands that are allowed to be executed in the in-game console. To these, permissions can be assigned as an integer value. If the player has a permission level value that is equal to or less that the value of the permission, the player may execute the command.

Second, the system allow for groups. I am not sure if the permission levels are really any different. Placing a player in the Admin role, then giving them a permission level of 999 (basically no permissions) would make them a poor admin, but is probably allowed.

So, I think you will need to apply your own ranking system and set ranges according to how you think they will best apply.

For example, you may allow admins to do everything, so they can have a narrow range. We will say they are level 0. Moderators might have greater range. So you may have a chief moderator who can also do everything. You would want to give him a 1, and set all permissions to level 1. Then admins and the chief are both covered. Then you can begin allowing less dangerous commands to be run by others who have a higher moderator rank (which is actually less permissive).

The admin tab is divided again into two sub-tabs. 

Permissions:
Default commands:
The default commands offered in the admin file when the dedicated server is installed (by default), and the commands that are currently active in the loaded admin file. Items can be removed from and added to the default list. The default list will be persistent and new commands found in an admin file will be added automatically. The list will have a filter to hide items that are already in the current command list.

Current Commands:
In the list of current commands, you can assign their permission level though an integer up/down control.  Items not already in the current list can be placed there by drag/drop. 


Users Tab:
A list of default users is maintained by the system (in user setting). It will be populated by any new users encountered in an admin file, or by adding a user through the UI. Users can also be removed. Only their steam ID is stored, which is public information. Loading data into a 'SteamUser' object is two phase. First, the steamID and read in from user settings, or an admin file. This is done async, so when the operation calls back, the command to get the steam data for the ID on the object is called. This means that this app hits steam quite a bit and I might hit a cap. We will see. I have a fix planned, but I'm dragging it out because this is much more flexible.

The user tab is conditioned in such a way that only one list may have a selection in it at a time. This is done so that delete can be pressed and it not be a nightmare to figure out what they want deleted. Also, I was considering making the list box tiles all selected all the time for appearance, because in this design there is not reason to select an item, other than possibly to delete it. (drag does not require a selection)

Items may only be dragged from the user list to another list. Deleted on lists other than users only effect that collection. Or to say, if I delete from the admin column, I have not deleted him as a user. Hoever, If I delete him from admin, then from users, I have permanantly deleted them. I also prompt before the permanent delete is allowed.

Adding Users:
I really wanted to be able to allow the user to search for a Steam player by name, then select them from a list of candidates, but the API does not allow it. However, admins like myself have been obtaining them from logs or 7DaysToDieServerManagerV2(By FrontRunnerTek), so we get by. What I am able to do is verify the guy for you. Enter the SteamID(64), verify, save. If the id is not in the right format, or errors on request, you will not be allowed to save. This makes the software really web dependant, but it will be running on servers, after all.



Plans for backups:
I have not started coding this yet, but it's a no brainer. I have lost my configs several times due to patches. And again, while it is not hard to keep a backup nearby, meh, it's for me in the first place (fun little tinker project) and the world second. Over-codeing for an already easy task is after all, what I'm doing. A friend at work also suggested that it could be used to toggle the game between two different configurations pretty quickly for running events and such, so it's going to happen for the config file for sure. There is less need to back up the admin files, as they seem to survive the patches better, but I'll already have the system in place and... 'who know?' is why we back up anyway.







