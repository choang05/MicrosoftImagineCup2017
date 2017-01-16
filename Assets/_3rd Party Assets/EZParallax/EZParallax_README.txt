/* 

EZ Parallax v1.3 by TimeFloat
(http://timefloathome.wordpress.com/)

First off, thanks so much for purchasing EZ Parallax! It's great to have your business.

EZ Parallax is contained within two scripts (EZ Parallax itself and then the custom inspector script), and comes with a demo scene, as you may have noticed by now. You should find them in the EZParallax folder installed with this download. Actually, if you're reading this, you probably already did! And remember, the EZParallaxEditor.cs file *must* remain in the "Editor" folder for your project to build properly. That's just where Unity looks for custom inspector files.

(Please be sure to read the info about the demo at the bottom of this readme)

EZ Parallax needs four requirements met for it to work properly:

	1. You need to lay out your parallaxing layers/elements in a 3D scene, which, with Unity, is probably the most intuitive setup anyhow.
	
	2. EZ Parallax is to be used with a camera facing down the positive Z axis. (Orthographic camera most likely, but perspective camera should work too if you're just using EZParallax for manual perspective control.)
	
	3. Your camera actually has to move, ideally with the player. Obviously, a parallax effect only happens when your viewpoint shifts.
	
	4. You need to have your camera in it's correct starting position *before* EZ Parallax initializes itself. If you can't set up your camera position in the scene editor, then you'll need to manually initialize from script. (More info on that below)
	

How it works:
-------------------------
EZ Parallax automatically uses your 3D scene to determine how objects should move, so your moving layers/elements should actually be set up at varying distances along the z axis. Since your camera will be looking down positive Z, you'll need to place your parallaxing elements in front of it, again, moving back away from the camera along the positive z axis.

You can place your elements at any distance from the camera you'd like. EZ Parallax normalizes the range of your parallaxing elements between the camera and the parallaxing element furthest away from the camera, most likely your sky or fog or something like that. Space things however you'd like in the scene, and the relative distances between your layers will automatically define their movement in a "realistic" fashion. You can even place objects in the foreground, between the player and the camera, and have them still move at an appropriate speed based on the parallax effect, fully delivering that feeling of depth in your environment.

EZ Parallax is quite flexible. All it needs is a transform of the element(s) you want moved. You can construct that object any way you'd like. It can have child objects, an animating texture, be animating itself... the sky is really the limit here. Because of this, in theory, you should be able to use it alongside other kits that may not offer parallaxing, depending on how they set up their scene. Users have reported that EZ Parallax works really well with 2D Toolkit.

I bet you're ready to set up your scene now...


"30 second setup":
-------------------------

Setup Option 1 -- Using Tags
1. Place your parallaxing elements and your actual gameplay elements in your scene(not included in the "30 seconds", lol). 

2. Assign the EZParallax script to any game object, so long as it will continue to exist throughout the course of play in your level. I like to create an empty gameobject, name it "ParallaxController", and assign the script to it.

3. Select your parallax control object (I'll be referring to it as the "ParallaxController").

4. The ParallaxController inspector has two fields, "Main Camera" and "Player Obj", for your player object and your main camera. Drag these objects into these fields to assign them, or use the object selector from the inspector.

5. Notice the combo boxes titled "Auto-Parallax Tag Name" and "Auto-WrapX-Parallax Tag Name". Here, you can create tag names to use that will enable you to simply tag elements in your scene to allow EZParallax to add them to the list of parallaxing elements. The first tag will be the tag you should put on items that you want to simply get the basic parallaxing effect. The second tag can be used to make the item automatically duplicate itself to fill the x axis, and wrap around the screen infinitely as the player moves from left to right. Default tags will be added to your project and filled into these slots, but if you want to use your own tag names click on either combobox and select "Add Tag" to add new tag names, and then assign these tag names in the inspector as your default parallax and default wrap tags.

6. Select an object in your scene that you would like to be included in the parallaxing effect. In the inspector for this object, click on the Tag combobox and select the regular parallaxing tag name that you specified or the x wrapping tag name if you'd like it to infinitely loop as the player moves.

7. Repeat step six for each element in your scene that you would like to be part of the parallax effect. **DO NOT include any objects that the player is actually interacting with. This is your focused plane, your "actual" level, and it should move perfectly with the player, so don't assign it to EZParallax.

8. Hit the play button and enjoy!


Setup Option 2 -- Using Drag and Drop

1. Place your parallaxing elements and your actual gameplay elements in your scene(like mentioned above). 

2. Assign the EZParallax script to any game object, so long as it will continue to exist throughout the course of play in your level. I like to create an empty gameobject, name it "ParallaxController", and assign the script to it.(Again like above)

3. Select your object (I'll be referring to it as the "ParallaxController").(It's deja vu all over again)

4. The ParallaxController inspector has two fields, "Main Camera" and "Player Obj", for your player object and your main camera. Drag these objects into these fields to assign them, or use the object selector from the inspector. (After this is when it gets different...)

5. Expand "Parallax Elements", if it isn't already, and set the "size". This is the number of items that you want to be controlled by the parallax effect. As you change this number, the number of elements beneath this field will grow. Alternatively, you can also click on the buttons beneath this field to add remove parallax items.

6. Each item beneath the "size" field is an expandable element that represents each parallaxing element controlled by EZ Parallax in your scene. Expanding one of these items will show a multitude of options that you can use to control how EZP handles the element. For now, just drag one of your parallaxing elements from your scene heirarchy into the "parallaxing object" slot. Notice that the name of the element updates automatically when you give it a new object. Repeat this step for all your parallaxing elements, dropping them into their allotted slots. **DO NOT include any objects that the player is actually interacting with. This is your focused plane, your "actual" level, and it should move perfectly with the player, so don't assign it to EZParallax.

7. Hit the play button and enjoy!

NOTE: These are the easiest and fastest ways to set up your scene, but EZ Parallax v1.3 has many more special options that can be set on each individual parallaxing element, as mentioned above. For any objects that you would like to use advanced options on, you must add the objects to the element list, like in "Setup Option 2", so that you can expand the element in the inspector and adjust its properties. Alternatively, objects can be added and manipulated from script, for those with special runtime needs. Also, tagged objects can be used together with objects that were dragged into the EZ Parallax element list. Just make sure that you don't tag objects AND add them to the list. It won't break anything, but that'd just be a waste of your time. :)


Options and Advanced Use:  
-------------------------

EZ Parallax offers many additional options for users who wish to tweak the parallax effect, or control how various elements used by the script are added, removed, and sequenced.

Parallax Speed Scalars:
EZ Parallax offers speed scaling for both the x and y axis. Generally, you want to keep these between 0-1, setting it to 1 if you want the automatic parallaxing speed. 1 is the default. If you set this number above 1, then things in the background will begin to move faster than they should. This generally has an effect that makes it feel like the camera is actually rotating around the player. I'd avoid this, but if you like the effect, keep it that way. Remember, any changes to these values will affect ALL of your parallaxing elements.

Dolly Zoom:
With dolly zoom turned on, changing the orthographic size of your camera will actually create the effect of dollying instead of zoom. The result is a much more cinematic and visually pleasing "zoom" effect, in my own opinion. If you can't visualize what that looks like, be sure to test it out! With this option enabled, parallaxing elements will position and scale themselves based on the ratio of the current orthographic camera size to the orthographic camera size at the start of the level. Changing the orthographic size in a single frame will result in an instant pop to the new size, so if you would like to see the gradual zoom effect, you'll need to manually animate your orthographic size over time.

Auto Initialize:
This flag is enabled by default, as this is probably what people will want for general use. Users that aren't going to be able to assign their player, camera, or parallaxing objects to EZ Parallax from the inspector (i.e. you are creating these objects at runtime), may want to turn off autoinitialize and then manually start initialization once they have assigned all the necessary items from their scripts. See the functions below for script manipulation of EZ Parallax.

Private Speed Scalars:
In the same way that you can change the x and y speeds of all parallaxing elements controlled by EZP, you also have the option of controlling scalars that affect the individual movement of each element. Expand a parallaxing element in the EZP inspector to view its internal properties. Here, you'll see the "Private Speed Scalar" for the x and y axis. Changing these values will only affect the element that you are currently changing the value for.

Motorization:
You can now make things move on their own with EZ Parallax! Motorization will let you have your clouds float by, birds fly by, boats sail by, etc. Anything that you can put in your scene as a 3D object and that would make sense moving along the x axis can now be given even more life with this option. To motorize one of your objects, simply expand its element in the EZP inspector's ParallaxElements list, check the "Is Motorized" box, and then enter a speed value. This speed is in Unity units, so keep that in mind. A positive value here will move to the right, while a negative value will move to the left.

Infinite Wrapping/Duplicate Spawning on the X axis:
You can now enable the "Spawn Duplicates on X" option on a parallaxing element and EZParallax will automatically detect the necessary number of copies to fill the screen as the camera moves from right to left, and will wrap them around the screen infinitely as the player progresses through your game. The "Units Between Dupes" value will let you set the number of units that you would like between each of your elements. If you would like the units between your duplicates to be random, let's say for maybe random plant placement, or random clouds, etc., you can use the next option.

Randomized Distance Duplicate Spawning
In addition to the regular wrapping and static distance spawning, you can now check the "Randomize Distance" checkbox and enable random spacing between your elements as they spawn and wrap infinitely. Once you check the "Randomize Distance" checkbox, two more fields will appear, "Min Units Between" and "Max Units Between". Here, you can set your minimum and maximum for your random range,and EZ Parallax will randomly choose a distance between these numbers when wrapping. EZP will keep a history of the distance between every item it locates as the the player travels along the x acis, so this way, if the player walks backwards, he/she sees things in the same location as they were when first passing through. If you notice a pattern, which is verrrry unlikely unless you are spawning tons of little elements, you can raise the number of random offsets stored in memory by changing the "Random Offset History Size", at the top of the EZ Parallax inspector, to a larger number. Also, this number must always  be greater than the number of items in the chain. You'll get an error message in the debug console if you need to add more.

Script control functions:

--InitializeParallax()
This function will cause the EZ Parallax script to recalibrate all it's settings based on the player object, the camera object, and any parallaxing elements that have been assigned to it. If you've turned off "auto initialize" in the inspector, you need to call this function manually once you're ready for EZ Parallax to start running. 

--AssignPlayer(GameObject targetPlayerObj, bool doInit)
Use this function to assign a player object if you weren't able to assign a player object in the inspector, or if your game destroys the player object and you need to add the new player object back to EZ Parallax. The second parameter, doInit, should be set to true if after calling this function you are ready for EZ Parallax to recalibrate the parallax with your new objects. The second parameter of the next two functions works like this as well.

--AssignCamera(GameObject targetCameraObj, bool doInit)
This function is just like AssignPlayer, but assigns the camera you'll be doing the parallaxing from. Make sure that your camera is in the correct starting position before you call InitializeParallax again, or the parallaxing elements will have strange offsets.

--AddNewParallaxingElement(Transform targetElement)
--AddNewParallaxingElement(Transform targetElement, float privateSpeedScalarX, float privateSpeedScalarY)
Use this function to add elements to your list of objects you wish to parallax during runtime. This is useful for people who need to spawn objects dynamically but still want them to move properly with the parallax effect. This function can be overloaded to take an x and y private scalar value. This function returns the created EZParallaxObjectElement.

--RemoveParallaxingElement(targetElement : Transform)
Use this to remove objects from the list of elements being controlled by EZ Parallax. If you remove an object, make sure that it is no longer visible in your scene, otherwise it may look strange and break your parallax effect. This function will only remove a single object. If you'd like to remove an object and all of its duplicates, use PurgeSingleDupeChain() found below.

--PurgeSingleDupeChain(Transform targetTransform)
Use this function to remove a SINGLE chain of elements spawned from a single EZP element. I.e., maybe you have wrapping clouds, and you want to remove all of them from the scene. Pass in any of your cloud element's transforms, and all the sibling clouds generated by EZP will be removed from the scene, along with the original transform/elemnent being passed. 

--SetElementWrapSettings(EZParallaxObjectElement targetElement, bool xWrapOn,  float spawnDistanceX)
Use this function to set the infinite wrap settings on an object that you have dynamically added to the parallax list at runtime.

--SetElementMotorized(EZParallaxObjectElement targetElement, float speed)
Use this function to set an object to be motorized after dynamically spawning it. If you wish to make the object wrap, be sure to set it to wrap FIRST, before applying the motorspeed.

--ToggleMotorization( Transform targetTransform, bool paused )
This function will toggle motorization for an entire chain of objects if the target is part of a wrapping set of elements.

--GetEZPObjectElementFromTransform(Transform targetTransform)
Pass in a transform of an object in your EZ Parallax parallaxing elements list to get the EZParallaxObject object associated with this transform.

--SetElementRandomSpawn(EZParallaxObjectElement targetElement, float minRange, float maxRange)
Use this function to set an element to infinitely wrap with random distances between its duplicates. It autmatically turns on x wrapping on the target object, so there is no need to manually set wrapping on the target before hand.

--SetElementRandomSpawn(EZParallaxObjectElement targetElement, float minRange, float maxRange, bool useAltSpawn, float minAltRange, float maxAltRange)
This is the overload of the above function, enabling the ability to turn on the randomized alternate axis spawning (currently just Y axis) for an object that is already randomly spawning on the main axis.

--updatePlayerZ()
If the player changes its Z position, call this function to update the depth values for the parallaxing elements. If left alone after changing the position of the player on the Z axis, you may experience strange parallaxing behavior, so be sure to use this. This function doesn't need to be called if a call to InitializeParallax is pending, as it will update the depth values as well.

--ToggleDollyZoom(bool newToggle)
Use this function to manually toggle dolly vs traditional zoom during runtime. Can be used effectively to take advantage of both effects!



A Note About the Demo Scene and Your future Scenes:
-----------------------------
The demo scene is a fairly quick level that I threw together, showcasing a Limbo-esque type scene to get your creative juices flowing. Here are some take-aways of note, so that you familiarize yoursself with EZP properly from the start:

1. Use the EZParallax Script on it's own empty game object to avoid confusion
2. Place your objects in 3D along the Z axis. Do NOT include your player or the ground that the player's avatar will be interacting with, as they need to work as the "base" for the parallax effect, and you don't want movement of those objects affecting your player.
3. Assign all necessary objects to EZ Parallax in the Inspector for EZ Parallax. For objects that don't have specific requirements, such as randomization or motorization, you can use the tag system. "Parallax" for basic parallaxing, "Parallax_Wrap" for infinite wrapping effect.
4. If you have multiple objects all at the same zdepth from the camera, group them together under a parent object and just assign that parent object to the EZParallax script.
5. Use vertex snapping with the manipulator to place your objects directly next to each other, if you're lining up tiling texture objects, so you don't get seams popping in and out as your objects move across the screen. **NOTE** There is currently a Unity bug in the editor with vertex snapping, so you have to zoom in reeeeaallly close to your adjacent edges so that the vertex snapping lines up properly. The closer you are, the more accurate it will be. Unity QA has verified with me that this is an editor bug that have reproduced and are working on a fix for the next Unity version.



BUGS:
-------------------------
If you find any bugs, be sure to email me at TimeFloatUnity@gmail.com and I'll do my best to fix them asap.


Future of EZ Parallax:
-------------------------
This most recent update is the first of a 3 part update to EZP which should bring about quite a bit more features. I'll be adding y axis infinite wrapping in the future, but early probing into what the code will look like for this shows that it'll probably be the biggest feature on the horizon, and will most likely show up in the last of this 3 part update.


Tutorials and additional help:
------------------------------
Tutorials, videos, general help, and more info can be found at the TimeFloat site -- http://timefloathome.wordpress.com/

And that about wraps up the readme. Thanks again for buying this product!
Check out the version notes if you're into that sort of thing and check out the demo if you want to see it working right away.
The camera lags a bit when played in the editor for some reason, but it's smooth as butter when built and running outside the editor, even on mobile.

Enjoy EZ Parallax, and good luck developing!

*/