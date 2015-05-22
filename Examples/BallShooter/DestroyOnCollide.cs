using System;
using UnityEngine;

namespace Examples.BallShooter
{
    /*
     * This will destroy any game object that it is attached to (using AddComponent)
     * a couple of seconds after colliding with anything.
     */
    public class DestroyOnCollide : MonoBehaviour
    {
        // See http://docs.unity3d.com/ScriptReference/Collider.OnCollisionEnter.html for documentation about this method
        // It is called when a collision involving the parent game object happens.
        public void OnCollisionEnter()
        {
            // gameObject is the game object this is attached to, 5 the number of seconds before it is destroyed
            Destroy(gameObject, 5);
        }
    }
}
