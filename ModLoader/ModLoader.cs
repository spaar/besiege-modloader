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

    public interface IGameStateObserver
    {
        void SimulationStarted();
    }

    internal class GameObserver : MonoBehaviour
    {
        private static List<IGameStateObserver> observers;
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

        private static AddPiece addPiece;
        public static AddPiece AddPiece
        {
            get
            {
                return addPiece;
            }
            internal set
            {
                addPiece = value;
            }
        }

        private static GUISkin guiSkin;
        public static GUISkin GUISkin
        {
            get
            {
                return guiSkin;
            }
            private set
            {
                guiSkin = value;
            }
        }

        private static GameObserver observer;

        void Start()
        {
            ModLoaderStats stats = ModLoaderStats.Instance;
            if (stats.WasLoaded)
            {
                return;
            }

            GameObject go = GameObject.FindObjectOfType<GameObject>();
            GameObject root = go.transform.root.gameObject;
            root.AddComponent<Console>();
            root.AddComponent<ObjectExplorer>();
            observer = root.AddComponent<GameObserver>();
            stats.WasLoaded = true;

            //StartCoroutine(LoadEditorBundle());

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
					        if (t.FullName.Contains("Mod"))
					        {
					            type = t;
					        }
					    }
                        go.AddComponent(type);
						Debug.Log(string.Concat("Attached and loaded ", fileInfo.Name));
					}
					catch (Exception exception)
					{
						Debug.Log(string.Concat("Could not load mod ", fileInfo.Name, ":"));
						Debug.LogException(exception);
					}
				}
			}
        }

        public static void RegisterGameStateObserver(IGameStateObserver observer)
        {
            GameObserver.RegisterGameStateObserver(observer);
        }

    }

    class ModLoaderStats
    {
        private static readonly ModLoaderStats instance = new ModLoaderStats();

        public static ModLoaderStats Instance
        {
            get
            {
                return instance;
            }
        }

        private bool wasLoaded = false;
        public bool WasLoaded
        {
            get
            {
                return wasLoaded;
            }
            set
            {
                wasLoaded = value;
            }
        }

        private ModLoaderStats() { }
    }
}
