# Cameron McDougall Individual Report 
**Name**: Cameron McDougall  
**Username**: mcdougcame  
**Animal role**: Owl  
**Primary project responsibility**: In terms of the code, it would of been the menu. In terms of within the project; I dealt with documentation. For example, I put up added issues to the sprint board on github which were discussed in our group chat or talked about in our lab. I also created the readme off of the designers report and completed issues that were assigned to me.  
## Code discussion   
**A description of the parts of the project you worked on, including a level of input: [ All | Most | Half | Some | Touched ]. Only mentioned touched if it is important to the project overall.**  
	- [Most] **Menu**: I did the MenuManager, PauseMenuManager, StartMenuManager classes which were used for the in-game and before the game has started menus. I worked on most of the LoadMenuManager, being that I needed some of the code from Quan Le that he implemented for loading in the PlayerController class to load game files.  
	- [Some] **Saving And Loading**(PlayerController class): I implemented some code to stop the saving taking duplicate checkpoints as the checkpoints could be triggered more than once while on it previously. I also added in code that stopped saves from a previous level in the same game as it created bugs. (findWithAttr and getCorrectLevel functions)  
	- [Some] **CameraController** : I fixed some code in the camera so it didn't go underneath the plane as it was annoying to the gameplay.  
	- [touched] **PlayerController class** : I fixed some code in the PlayerController Class to stop the player from getting the cube ability for all characters when swapping.  
**A description and link to the most interesting part of code written by you**
[Actions in parent class](https://github.com/CameronMcDougall/ShapeGameProject/blob/master/Assets/_Scripts/Menu/MenuManager.cs#L25-L42)
[Actions in child class](https://github.com/CameronMcDougall/ShapeGameProject/blob/master/Assets/_Scripts/Menu/StartMenuManager.cs#L22-L39)
I would say the most interesting part of my code was the use of the Actions class to make the menus reusable. The Action class allowed me to use functions in the parent class from the child class.  
**Identification of the section of code you are most proud of - and why you feel that this is particularly good code. (this could be the same as above)**  
I'm proud of the MenuManager class as it uses polymorpism and a few things like Actions to make the class more reusable and that makes it less code overall for the menus. I feel the classes code is well structured as well as the functions are spread out nicely.  
## Learning reflection (can be technical or personal reflections)  
- **Reflection on what you have learnt and how you have developed as a programmer in this project**  
I learn't how to work in a agile environment with a team. In previous courses, I never used issues or a scrum board before so it was a good experience learning how programming teams operate.
I also learnt the importance of sprint planning as it helps planning out the issues needed in a timespan. I've also learnt how to deal with merge conflicts which I didn't have much experience with in the past.
I also learnt that unity is very annoying with git as you could get merge conflicts but some files would be in bytecode being unable to fix those conflicts without downloading files.
- **A description of the most important thing you will use from this project in future projects**
I will definitely use the knowledge learnt from the agile methodology as I'm starting my graduate job at the end of this trimester. This would include my knowledge learnt of dealing with merge conflicts and planning scrum sprints to achieve project goals more effectively. Also linking commits to issues would also help figuring out what developers have already done in trying to fix a certain issues as I wouldn't have to look through a whole lot of commits. 