# Indivual assesment report: Quan Le

**Name:** Quan Le
**Username:** lequan1 
**Animal role:** Cat
**Primary Project responsibility:** Checkpoint & respawn interactions, Auto Saving system and Load Menu.

# Code discussion

- [All] **Autosave system**: I had created a autosave system in the game where upon player interaction with a 'checkpoint' in the game, a save-file will be automatically **created/appended/overwritten** which stores the serialized **GameData** objects up to a maximum of three. This save file can then be parsed during the execution of any future loading functions, in the same or complete seperate instance of the running application to load the load the Data and launch to the gameplay with the different data sets.
- [All] **GameData object**: I created a class that meant to be a variable storage, meant to store different variables representing different aspects of the gameData. Currently the data only holds which checkpoint and it's corresponding level, was the checkpoint that the player last interacted with. This represents the player's saved progression in the game, and when 'loaded' the character will start at the saved-file's checkpoint rather than the start of the game. <br>
While the data stored currently is very native and holds the bare necessity, more data could be added with little to no effort, such as death counter or time-elapsed during the gameplay. Also if the player decides to progress backwards and go to a previous checkpoint, it will still override his progress as normal.
- [Half] **Load menu**: using the MenuManager interface that was created by Cameron McDougall(mcdougcame), i followed up with my own implementation of a menu screen, and also provided my own methods which were to deserialize the Load-file into *GameData* objects, parse them and display them as text on buttons that users can click to load the game with data from said objects.
- [Some] **Player Controller**: i modified the player controller to change it's starting location upon level loading, based on whether the game was 'started' or 'loaded'. I also added interactions with the 'checkpoints' and 'killbox' prefabs which sets the most recent checkpoint, and forces a respawn action respectively.

### Interesting code

The most interesting part of my code in my opinion would be the the Awake() method inside [playerController](https://github.com/CameronMcDougall/ShapeGameProject/blob/master/Assets/_Scripts/PlayerController.cs#L121-L131) where prior to the execution of the start method, will override the spawn position of the player. As part of the loading function, it was tricky as the PlayerController script would have needed to access variables from the loadMenu script, which was in a completely different scenes. 

Therefore I had to find some form of persistent data transfering between scenes, and researched and found that static classes or variables made in a script were persistent throughout the whole game, and i was able to make a workaround by assigning the chosen save-file that was being loaded, to a a static variable before transitioning scenes and upon a new scene, access the variable and assign it to the newly initialized playerController script (demonstrated in the GameData object and the PlayerController class on line 123 - 124).

### Proud code
I'm proud of the save and load functions in the PlayerController and LoadMenu respectively, as i had to research on different concepts such as how to create a new file in C#, and store data which can be loaded, which required 'serialization'. Furthermore to produce 1 autosave file which can be overwritten and actually store multiple save-datas, instead of having multiple physical 'files' on the computer was a cleaner solution that i was proud of, as it made the savefiles easier to sort in order of the most recent save.

# reflection:

### What I've learnt from this project
Whilst I have had experience with using GitLab and working in an team-based Agile environment prior, it was still nonetheless good experience for developing in a team. However what was fully fresh for me was working with Designers for a game, where tasks were fully segregated and to coporate with each other with completely different skill sets to produce a good outcome. Also, experience with Unity and C# was also a highlight, as Unity is a powerful engine with vast amounts of features and enhanced skills with another C# will always be a good benefit.

### Future project use
Definitely the communication and task assignment when it comes to people of complete different skill sets on a team.


