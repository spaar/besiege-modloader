using System;
using System.Collections.Generic;
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

        public static List<Mod> LoadedMods { get; private set; }

        private static GameObserver observer;

        internal static Configuration Configuration { get; set; }

        void Start()
        {
            ModLoaderStats stats = ModLoaderStats.Instance;
            if (stats.WasLoaded)
            {
                return;
            }

            AddPiece = null;

            LoadedMods = new List<Mod>();
            LoadedMods.Add(new Mod("ModLoader")); // Needed so the mod loader can actually register commands itself
            LoadedMods[0].assembly = Assembly.GetExecutingAssembly();

            var console = gameObject.AddComponent<Console>(); // Attach the console before loading the config so it can display possible errors

            Configuration = Configuration.LoadOrCreateDefault(Configuration.CONFIG_FILE_NAME);
            Keys.LoadKeys();

            gameObject.AddComponent<KeySettings>();
            observer = gameObject.AddComponent<GameObserver>();
#if DEV_BUILD
            gameObject.AddComponent<ObjectExplorer>();
#endif
            console.EnableInterface(); // Enable the console interface since it can now ask the configuration for the correct keys to use
            stats.WasLoaded = true;

            FileInfo[] files = (new DirectoryInfo(Application.dataPath + "/Mods")).GetFiles("*.dll");
            foreach (FileInfo fileInfo in files)
            {
                if (!fileInfo.Name.EndsWith(".no.dll") && fileInfo.Name != "SpaarModLoader.dll")
                {
                    Debug.Log("Trying to load " + fileInfo.FullName);
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
                        var types = assembly.GetExportedTypes();

                        bool foundAttrib = false;

                        foreach (var type in types)
                        {
                            var attrib = Attribute.GetCustomAttribute(type, typeof(Mod)) as Mod;
                            if (attrib != null)
                            {
                                gameObject.AddComponent(type);
                                attrib.assembly = assembly;
                                LoadedMods.Add(attrib);
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
                                    gameObject.AddComponent(type);
									Debug.Log("Successfully loaded " + fileInfo.Name);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.Log("Could not load " + fileInfo.Name + ":");
                        Debug.LogException(exception);
                    }
                }
            }

            Console.RegisterCommand("listMods", (args, namedArgs) =>
            {
                var result = "Loaded mods: ";
                for (int i = 1; i < LoadedMods.Count; i++)
                {
                    var mod = LoadedMods[i];
                    result += "\n " + mod.Name() + " (" + mod.version + ") by " + mod.author;
                }
                return result;
            });
            Console.RegisterCommand("version", (args, namedArgs) => { return "spaar's Mod Loader version 0.2.2, Besiege v0.09"; });
        }

        public void Update()
        {
            if (AddPiece == null)
            {
                try
                {
                    AddPiece = GameObject.Find("BUILDER").GetComponent<AddPiece>();
                }
                catch (NullReferenceException)
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
