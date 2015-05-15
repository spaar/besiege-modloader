using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace spaar
{

    /// <summary>
    /// Interface for being notified of game status changes.
    /// </summary>
    public interface IGameStateObserver
    {
        /// <summary>
        /// Called when the simulation is started, either via the play button or hitting spacebar.
        /// </summary>
        void SimulationStarted();
    }

    /// <summary>
    /// Internal class used for notifying of IGameStateObservers.
    /// The observers are registered through ModLoader, which in turn calls this.
    /// </summary>
    internal class GameObserver : MonoBehaviour
    {
        private static List<IGameStateObserver> observers;
        // Whether the observers were already notified for the current simulation
        private static bool notifiedObservers;

        void Start()
        {
            observers = new List<IGameStateObserver>();
            notifiedObservers = false;
        }

        void Update()
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

    public class ModLoader : MonoBehaviour
    {
        /// <summary>
        /// A reference to the AddPiece component currently in use.
        /// AddPiece is used for a variety of purposes in the game and it is often necessary to access it.
        /// </summary>
        public static AddPiece AddPiece { get; private set; } 

        private static GameObserver observer;
        
        public static Font guiFont;

        void Start()
        {
            ModLoaderStats stats = ModLoaderStats.Instance;
            if (stats.WasLoaded)
            {
                return;
            }

            AddPiece = null;

            var modObject = new GameObject("Mods");
            modObject.AddComponent<DontDestroyOnLoady>();

            modObject.AddComponent<Console>();
#if DEV_BUILD
            modObject.AddComponent<ObjectExplorer>();
#endif
            observer = modObject.AddComponent<GameObserver>();
            stats.WasLoaded = true;

            FileInfo[] files = (new DirectoryInfo(Application.dataPath + "/Mods")).GetFiles("*.dll");
            for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileInfo = files[i];
                if (!fileInfo.Name.Contains(".no.dll") && fileInfo.Name != "SpaarModLoader.dll")
				{
					Debug.Log(string.Concat("Trying to load ", fileInfo.FullName));
					try
					{
					    Type type = null;
						Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
					    foreach (Type t in assembly.GetTypes())
					    {
					        if (t.Name == "Mod")
					        {
					            type = t;
					        }
					    }
                        modObject.AddComponent(type);
						Debug.Log(string.Concat("Attached and loaded ", fileInfo.Name));
					}
					catch (Exception exception)
					{
						Debug.Log(string.Concat("Could not load mod ", fileInfo.Name, ":"));
						Debug.LogException(exception);
					}
				}
			}
	    FontFind();
        }

        public static void FontFind()
        {
            UnityEngine.Object[] fontObjectArray = Resources.FindObjectsOfTypeAll(typeof(Font));
            foreach (UnityEngine.Object fontObject in fontObjectArray)
            {
                Font font = (Font)fontObject;
                if (font.name == "GOST Common")
                {
                    guiFont = font;
                }
            }
        }
        
        void OnGUI()
        {
            GUI.skin.font = guiFont;
        }

        public void Update()
        {
            if (AddPiece == null)
            {
                try
                {
                    AddPiece = GameObject.Find("BUILDER").GetComponent<AddPiece>();
                }
                catch (NullReferenceException e)
                {
                    // Probably in menu, no AddPiece there, just fall through
                }
            }
        }

        /// <summary>
        /// Register an IGameStateObserver. Every registered observer is always
        /// notified of the supported status changes that occur in the game.
        /// </summary>
        /// <param name="observer">The observer to be registered.</param>
        public static void RegisterGameStateObserver(IGameStateObserver observer)
        {
            GameObserver.RegisterGameStateObserver(observer);
        }

    }

    /// <summary>
    /// Basic datastore (singleton) for mod loader statistics.
    /// </summary>
    internal class ModLoaderStats
    {
        private static readonly ModLoaderStats instance = new ModLoaderStats();

        public static ModLoaderStats Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Whether or not the loader was already loaded this game session and in turn loaded all mods.
        /// Due to the way the internal mod loader works, the ModLoader component can be added more than once,
        /// however the mods and mod loader components should only be added once.
        /// </summary>
        public bool WasLoaded { get; set; }

        private ModLoaderStats() { }
    }
}
