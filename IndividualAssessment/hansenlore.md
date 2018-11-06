Header Information:

Name: Lorens (Leo) Hansen
Username: hansenlore
Animal: Puppy
Primary Project Responsibility: User interface, Spinning Top, Level Manager

Code Discussion:

- Description of the parts of the project I worked on (only the important code), including input level:

User Interface: 
In Unity, I constructed the HUD the player can see when they’re playing, adding various text boxes, and anchoring them so they would work for any size screen. I then added the relevant code to display the text and timers for the level in these UI elements. The elements I coded include updating the attempt number the player is on, the timer, the finishing text and the text to ask if the player wants to restart. Additionally, I put in the floating text in level 1 and 2.

For the UI elements, my level of input is: MOST
The only UI element I did not work on was the menu.

Level Manager:
I constructed a level manager for the assignment so we could switch between levels when the player reaches the end of one. With the level manager, coding changing and restarting levels was a lot easier and just involved one line of code since all the logic was done in this class.

For the level manager, my level of input is: ALL

Spinning Top:
The spinning top is the fourth shape a player can change into, though was cut for timing constraints in level design. I implemented all of the code to handle the shape’s ability to spin, be able to move precisely whilst spinning (rather than having momentum), and to pickup nearby objects labelled with the tag ‘Pickup’. This nearly worked perfectly, only having a few issues with movement and clipping through walls sideways. All the code is in the final program, though the shape is blocked to the player as there are no levels made where it is suitable.

For the spinning top, my level of input is: MOST
I did not design the spinning top nor the animations of spinning / starting up. I did do the animation controller for it however as well as all the code.

Important other parts:
Other features of code I contributed to are:

•	Cylinder boosting (fixing the direction and speed of the boost) Input level = TOUCHED

•	Changing the rigid-body collision detection to continuous when a cylinder object is used so it cannot boost through the walls. Input level = TOUCHED

•	Fixing the variable jumping height of the sphere so it now jumps at the same height every time. Input level = MOST (I moved actionLogic to be called in fixed update and wrote a few extra lines of code where sphere jumping was done to fix this).

- Description and link to the most interesting part of code written by me:

There are a few pieces of code that contend for the most interesting part of code, but having to only choose one, I would choose the code for the spinning top, in particular the part where I alter its rigidbody constraints in order to make it be able to spin and move properly. (I am not sure why the code is all on one line, but it's the part of the line about rb.constraints = FreezeRotation etc.

https://github.com/CameronMcDougall/ShapeGameProject/blob/f9f9d91eab922e85b1e23f7be166551bcaf59908/Assets/_Scripts/PlayerController.cs#L285

Since the spinning top cannot be accessed in the final game, here is an interesting part of code that can be seen in the final product:

https://github.com/CameronMcDougall/ShapeGameProject/blob/f9f9d91eab922e85b1e23f7be166551bcaf59908/Assets/_Scripts/PlayerController.cs#L676

This (above link) is the code I wrote in to handle removing text after a certain amount of time has passed. When first looking at this, I did not know any means of how this could be possible, but after researching, I found it was necessary to start a coroutine when calling the method to deactivate text so pausing deactivation would not affect the rest of the elements in the game, but rather solely affect the attempt text that I passed in.


- Identification of section of code I am most proud of and why:

The section of code I’m most proud of is the cylinder boosting bug fix. This was a major issue with our game where the cylinder would not boost in the direction the player was facing with the camera, but rather in one global direction at first. So one would not be able to beat level 2 and progress through the game. This was altered by another member of the group but now had the issue that the cylinder would only launch in the direction the top face was facing, which still wasn’t what we intended. The first part I fixed was to change the ‘transform.up’ code  to state ‘cam.transform.up’. This allowed the cylinder to move in the direction the camera was facing. However, the cylinder still boosted too far upwards and not far enough forwards. I experimented with some different values to multiply each of the directions by and the overall boost value in the public inspector to get the correct amount of boost to clear the gaps in level 2 and the gap in level 3.

https://github.com/CameronMcDougall/ShapeGameProject/blob/f9f9d91eab922e85b1e23f7be166551bcaf59908/Assets/_Scripts/PlayerController.cs#L236

Learning Reflection:

- Reflect on what I have learnt + how I have developed as a programmer in the project:

From this project, I have learnt more about the process of developing a game with a team of people. This was a new experience to me and I found that working in a team containing both coders and designers has made us able to produce a lot more for our game than I would have been able to produce alone. 

In the technical side of things, I have learnt much more about how to use and modify UI elements in Unity, such as making text appear for a set amount of time only and introducing a timing system which is displayed in minutes and seconds. Additionally, I have learnt about coding in a level management system to handle the various scenes we have in our project and be able to easily switch / restart them. Coding the spinning top, however, has been the greatest source of my learning, since I needed to implement a new movement system for it, create an animation controller to handle its animations and add in a functionality to pickup objects (letting them hover above the player while they spin).

- Description of the most important thing I will use from this project in future projects:

The most important part of this project I will use in the future is the use of Github for overall collaboration between team members with different sets of skills to produce a working game.
