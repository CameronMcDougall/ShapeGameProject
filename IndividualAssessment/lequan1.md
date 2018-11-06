# Indivual assesment report: Quan Le

**Name:** Quan Le
**Username:** lequan1 
**Animal role:** Cat
**Primary Project responsibility:** Checkpoint & respawn interactions, Auto Saving system and Load Menu.

# Code discussion

- [All] **Autosave system**: I had created a autosave system in the game where upon player interaction with a 'checkpoint' in the game, a save-file will be automatically **created/appended/overwritten** which stores the serialized **GameData** objects up to a maximum of three. This save file can then be parsed during the execution of any future loading functions, in the same or complete seperate instance of the running application to load the load the Data and launch to the gameplay with the different data sets.
- [All] **GameData object**: I created a class that meant to be a variable storage, meant to store different variables representing different aspects of the gameData. Currently the data only holds which checkpoint and it's corresponding level, was the checkpoint that the player last interacted with. This represents the player's saved progression in the game, and when 'loaded' the character will start at the saved-file's checkpoint rather than the start of the game. 

While the data stored currently is very native and holds the bare necessity, more data could be added with little to no effort, such as death counter or time-elapsed during the gameplay. Also if the player decides to progress backwards and go to a previous checkpoint, it will still override his progress as normal.
- [Half] **Load menu**: using the MenuManager interface that was created by Cameron McDougall(mcdougcame), i followed up with my own implementation of a menu screen, and also provided my own methods which were to deserialize the Load-file into *GameData* objects, parse them and display them as text on buttons that users can click to load the game with data from said objects.
- [Some] **Player Controller**: i modified the player controller to change it's starting location upon level loading, based on whether the game was 'started' or 'loaded'. I also added interactions with the 'checkpoints' and 'killbox' prefabs which sets the most recent checkpoint, and forces a respawn action respectively.

### Interesting code

