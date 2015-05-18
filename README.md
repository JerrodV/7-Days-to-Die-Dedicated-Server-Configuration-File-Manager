# 7-Days-to-Die-Dedicated-Server-Configuration-File-Manager
A WPF utility application for managing the XML files associated with the 7 Days to Die dedicated server


DISCLAIMER-
Neither this application or the developer is in any way associated with 7 Days to Die. 
While the application has been designed to deal with your sensitive server files reliably,
it is USE AT YOUR OWN RISK and I cannot be held responsible for any ill effects that come from using it.
This would likely be most notable if the developers were to remove settings that if present cause the server to crash,
or were to alter the allowed values of a setting to where presenting the older value causes the server to crash.
In either case, manually editing the setting should restore the server functionality.
I use the program myself and will update it if I see any oddities as future server versions come out.


Instructions:

Using Seven Days Configuration UI is meant to be simple. The idea is that you load up your configuration file and edit your settings through an informative UI, then save when it is all correct. The XML write is done in such a way that any regrouping or reordering you might have done should be fine and preserved when the file is rewritten.

If at any time you have questions on what an item does, hover your mouse over the it. A tool tip will show with the original developer text as well as any (mostly) helpful information I had to offer. Note that some of the tool tip descriptions may describe settings that I have improved upon for the UI. For instance, you may see something like 0 = easy, but also see 'Easy' in the drop down list and not '0'. This is because I am doing the interpretation behind the scenes, but wanted to stay true to the idea of showing the original developer text.

Note that the application will automatically remember the last files you've looked up. If 'n/a' appears, no file path has yet been set. If a message other than the file path or 'n/a' appears, the application was not able to resolve the file path you are saving, or the one you have saved.

The UI controls are mostly self validating. In the case where free text is allowed, and the value is required, a dialog is shown warning you, the field will be highlighted in red, and you will not be able to save until the error is corrected.

Any time a file is saved, it is then reloaded into the UI. The message in the status does not reflect the reload. This is intentional.

Configuration:
This set of tabs is pretty straight forward. The UI elements coorespond to a property of the selected configuration file. Direct binding of the elements to the ui ensures validation.

The tab has two sub-tabs that group the configuration file properties into two sections. Those that apply to the server, and those that apply to the game.

Administration:
The operation of this section is a little less straight forward. The system is simple, but requires some knowledge of how the systems works.

First, there is a finite list of commands that are allowed to be executed in the in-game console. These permissions can be assigned an integer value. If the player has a permission level value that is equal to or less that the value of the permission, the player may execute the command.

Second, the system allow for groups. I am not sure if the permission levels are really any different. Placing a player in the Admin role, then giving them a permission level of 999 (basicaly nothing) would make them a poor admin, but is probably allowed.

So, I think you will need to apply your own ranking system and set ranges according to how you think they will best apply.

For example, you may allow admins to do everything, so they can havea narrow range. We will say they are level 0. Admins might have greater range. So you may have a chief moderator who can also do everything. You would want to give him a 1, and set all permissions to level 1. Then admins aand the chief are both covered. Then you can begin allowing less dangerous commands to be run by others who have a higher moderator rank(which is actually less permissive).

The admin tab is divided again into two sub-tabs. 

The Permissions tab holds two lists. 
Default commands:
The default commands offered in the admin file when the dedicated server is installed (by default), and the commands that are currently active in the loaded admin file. Items can be removed from and added to the default list. The default list will be persistant and new commands found in an admin file will be added automatically. The list will have a filter to hide items that are already in the current command list.

Current Commands:
In the list of current commands, you can assign their permssion level though an integer updown control.  Items not already in the current list can be placed there by drag/drop. 


Users Tab:
A list of default users is maintained by the system (user setting). It will be populated by any new users encountered in an admin file, or by adding a user through th UI. Users can also be removed. 




