using System;
using UnityEngine;

/*
 * A simple mod that shoots a ball at the players machine when a key is pressed.
 * This is mostly based on the an earlier tutorial but adapted to a new version of the mod loader.
 * http://forum.spiderlinggames.co.uk/blogs/spaar-s-modding-thoughts/11128-introduction-to-besiege-modding
 * It also contains new features to show off some more advanced Unity features.
 */

namespace Examples.BallShooter
{
    [spaar.Mod("Shooter", version="1.0", author="spaar")]
    public class ShooterMod : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.B))
            {
                // This executes every time B is pressed while left control is held down

                // Create a new ball to shoot
                var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // Get the machine position from AddPiece, a very useful class from the game
                var machinePos = AddPiece.machineMiddle;
                // Move the ball to 50 units above the machine
                ball.transform.position = new Vector3(machinePos.x, machinePos.y + 50.0f, machinePos.z);
                // Enable physics for the ball
                ball.AddComponent<Rigidbody>();
                // Make the ball destroy itself after colliding with the machine so the lag doesn't get too bad
                ball.AddComponent<DestroyOnCollide>(); // See DestroyOnCollide.cs to see how this works
            }
        }
    }
}
