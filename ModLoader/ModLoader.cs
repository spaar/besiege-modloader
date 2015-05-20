using System;
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

    public class ModLoader : MonoBehaviour
    {
        /// <summary>
        /// A reference to the AddPiece component currently in use.
        /// AddPiece is used for a variety of purposes in the game and it is often necessary to access it.
        /// </summary>
        public static AddPiece AddPiece { get; private set; }

        private static GameObserver observer;

        internal static Configuration Configuration { get; set; }

         private void Start()
        {
            var stats = ModLoaderStats.Instance;
            if (stats.WasLoaded)
            {
                return;
            }

            AddPiece = null;

            //KeyManager.LoadKeys();
            modObject.AddComponent<DontDestroyOnLoady>();
            modObject.AddComponent<Console>();
            modObject.AddComponent<KeySettings>();
#if DEV_BUILD
            modObject.AddComponent<ObjectExplorer>();
#endif
            observer = modObject.AddComponent<GameObserver>();
            stats.WasLoaded = true;

            var files = (new DirectoryInfo(Application.dataPath + "/Mods")).GetFiles("*.dll");
            SearchAttributes(files);
        }

        public void SearchAttributes(FileInfo[] Fileinfo)
        {
            foreach (var fileInfo in Fileinfo)
            {
                if (!fileInfo.Name.EndsWith(".no.dll") && fileInfo.Name != "SpaarModLoader.dll" &&
                    fileInfo.Name != "Mono.Reflection.dll"
                    && fileInfo.Name != "Mono.Cecil.dll")
                {
                    Debug.Log("Trying to load " + fileInfo.FullName);
                    try
                    {
                        var assembly = Assembly.LoadFrom(fileInfo.FullName);
                        var types = assembly.GetTypes();

                        var foundAttrib = false;

                        foreach (var type in types)
                        {
                            var attrib = Attribute.GetCustomAttribute(type, typeof(Mod)) as Mod;
                            if (attrib != null)
                            {
                                modObject.AddComponent(type);
                                Debug.Log("Successfully loaded " + attrib.Name() + " (" + attrib.version + ") by " + attrib.author);
                                foundAttrib = true;
                            }
                        }

                        if (!foundAttrib)
                        {
                            // Continue to load a Mod class if not Mod attribute is found for now.
                            // Otherwise all current mods would break and require an update.
                            // TODO: Remove this fall-back after most mods have updated
                            foreach (var type in types)
                            {
                                if (type.Name == "Mod")
                                {
                                    modObject.AddComponent(type);
                                    break;
                                }
                            }
                        }

                        Debug.Log("Successfully loaded " + fileInfo.Name);
                    }
                    catch (Exception exception)
                    {
                        Debug.Log("Could not load mod " + fileInfo.Name + ":");
                        Debug.LogException(exception);
                    }
                }
            }
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
}
