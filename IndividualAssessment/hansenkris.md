# Header Information

**Name:** Kristian Hansen
**Username:** hansenkris, arbiter91268 (github)
**Animal Role:** Bear
**Primary Project Responsibility:** Programming, bugfixing


# Code Discussion

+ **Camera (Most)**: Camera movement and controls. Camera follows the player around, but is also controllable and zooms in when an object is between the camera and the player. I got the camera working for the most part apart from a few bug fixes and the initial camera setup.
+ **Player abilities/movement (Some-Half)**: How the player moves around and interacts with the levels. Also includes the various abilities (different shapes and their bonuses) that the player has at their disposal. I worked on mainly the cube ability of the player and giving the cube an ability to break objects within a level. The cube will plummmet and break an object with a certain tag when it hits it at a certain momentum. I also worked on general code cleanup and bugfixes for the player, including bugfixing player deformation on moving platforms (our biggest bug we identified) and cylinder launch direction problems. 
+ **World Interaction (Some)**: Various objects/entities in the world that the player interacts with. I programmed water currents for this, where the player is propelled at a set direction with a set velocity along a water current object. The script I wrote for this is attached to any GameObject within the levels that will have moving water interactions. 

#### Interesting Code

`FixedUpdate()` in [CameraController.cs](https://github.com/CameronMcDougall/ShapeGameProject/blob/master/Assets/_Scripts/CameraController.cs) is code that I wrote which I personally find interesting. This code is responsible for managing the camera position and facing direction for the player. It is programmed to follow the player but also be movable as the cursor is moved around the screen, and thus the facing direction will be updated. This code is interesting because of its mathematical complexity and the amount of time I spent on this code to get it right. A raycast is also used in this code to ensure the camera has an unobstructed view of the player and thus will move the camera in front of any obstructing objects, and return back to normal when the preferred camera position is no longer obstructed. This took me multiple attempts to get right along with multiple online tutorials read. Fellow group members also did some minor tweaking/bugfixing of the camera after I had a go at it. 

#### Proud Code

The code I am particularly proud of is the code which I wrote which breaks an object on collision in heavy cube plummet mode:
```java
if (mor == ShapeVar.CUBE && !shrunk && col.collider.CompareTag("Breakable"))
{
    Vector3 momentum = rb.velocity * rb.mass; // p=mv
    if (momentum.magnitude >= breakingMomentum)
    {
        var exp = GetComponent<ParticleSystem>();
        Destroy(col.gameObject);
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        rb.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    }
}
```
I am proud of this code because of how simple it is, but how much it does for the player interactions with the world. It checks whether the right conditions are present (correct player shape and collided with an appropriate object) and then calculates breaking physics accordingly. It then calculates the momentum of the player using standard physics (Newton's second law: p=mv) and if the momentum is big enough, then it removes this object from the game and sets the player velocity to 0. 


# Learning Reflection

#### What I have Learnt

This course along with this project provided me with useful skills, both technical and personal. For the project I had to learn how to use Unity and how to code in C# from scratch. This was no easy feat, but from my experiences with other languages, I adapted what I learnt to C#. Since I mainly did programming, I did not have to learn much Unity. This project also improved my knowledge of Git and further cemented collarbarative and communicative skills with other group members to work together and develop a solution. My learnt skills of unity and C# will futher expand my toolbelt as a programmer and can thus be used in future projects/careers that require the use of some or all of unity and/or C#. Furthermore, the use of git within this project will prove useful in future group projects because of how powerful Git is for version control, issue tracking, and code backup. Tools which boost efficiency and ease of access are very sought after in programming, and thus having a compresensive understanding of Git is a useful skill to posess.

#### Most Important Thing

In my opinion, the most important thing I will use from this project is the newfound knowledge of the C# language and library. While Unity is a powerful tool, it can only be used to develop video games and similar programs. Whereas C# is an entire language and can thus be used to create anything. Git is also an important tool that I further learnt more about during the development of this projcet because of how useful Git is with group development and within the software development industry. 