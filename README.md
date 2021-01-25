# Rigidbody-FPS-Controller
Rigidbody FPS Controller for Unity3D. 

Modified version of DaniDev's Rigidbody FPS Controller.

Original tutorial : https://www.youtube.com/watch?v=XAC8U9-dTZU&t=1s

DANI YOU ABSOLUTE MONSTER!!

Controller:
 - Movement
 - Sliding
 - Jumping

I've added the elements that i wanted over the basic one that dani made.
Added:
 - Allowed movement while crouching.
 - Wall Bouncing.(Kinda Cool)
 - Wall Climbing.

Updates?
 - Animations.(Depending on if i continue using the capsule)
 - More Customizability.(Parameters are already customisable)
 - Grapple?(maybe not)
 
Steps:
1. Make a capsule or a 3D object of your choice.
2. Add a rigidbody component to it and freeze all rotations.
3. Make 3 child obejects under it called 
       - Orientation
       - Head
       - Camera
4. Place the Main Camera under the camera object.(DUH)
5. Place the head near the top of the capsule.
6. Attach the PLAYERMOVEMENTMODIFIED script to the player capsule.
7. Drag the camera object(not the actual camera) to the player cam slot.
8. Drag the ORIENTATION object to its slot.
9. Add the Move Camera script to the Camera object and drag in the HEAD object into the slot available.
10. Make a new layer called GROUND.(user layers)
11. In the script choose GROUND in the field WHAT IS GROUND?.
12. Add all of the ground elements(the ones you want to walk on) on the GROUND layer.
13. ENJOY THE AWESOMENESS!!
(optional) but recommended.
14. Create a Physics material in the project panel and set the friction or drag to 0.
15. Apply this material to all the mesh renderer of all the GROUND elements.


Also included is a file that allows for a better feeling and more responsive and tactile jumping from the tutorial by BOARD TO BITS GAMES

Link: https://www.youtube.com/watch?v=7KiK0Aqtmzc

I really suggest you use this, it makes jumping feel so much more video game like and organic.

Cheers,
trip

