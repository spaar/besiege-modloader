using System.Collections.Generic;
using UnityEngine;

namespace spaar
{
    /// <summary>
    /// Internal class used for notifying of IGameStateObservers.
    /// The observers are registered through ModLoader, which in turn calls this.
    /// </summary>
    internal class GameObserver : MonoBehaviour
    {
        private static List<IGameStateObserver> observers;
        // Whether the observers were already notified for the current simulation
        private static bool notifiedObservers;

        private void Start()
        {
            observers = new List<IGameStateObserver>();
            notifiedObservers = false;
        }

        private void Update()
        {
            if (AddPiece.isSimulating && !notifiedObservers)
            {
                SimulationStarted();
                notifiedObservers = true;
            }
            else if (!AddPiece.isSimulating && notifiedObservers)
            {
                notifiedObservers = false;
            }
        }

        public static void RegisterGameStateObserver(IGameStateObserver observer)
        {
            observers.Add(observer);
        }

        public void SimulationStarted()
        {
            foreach (var observer in observers)
            {
                observer.SimulationStarted();
            }
        }
    }
}
