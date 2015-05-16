using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if DEV_BUILD
namespace spaar
{
    public class ObjectExplorer : MonoBehaviour
    {
        // Represents one of the pop-up windows that shows an object in more detail.
        class Window
        {
            public int id;
            public Rect rect;
            public GameObject go;
        }

        List<GameObject> visibleGameObjects;
        Dictionary<GameObject, Window> openWindows;

        Vector2 scrollPosition = Vector2.zero;
        Rect windowRect;
        string searchText;
        bool searchingComponents;

        private bool visible = false;

        /// <summary>
        /// Updates the internal list of game objects, also performing the actual search.
        /// </summary>
        private void UpdateGameObjectList()
        {
            GameObject[] objs = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
            visibleGameObjects = new List<GameObject>();
            if (searchText.ToLower().StartsWith("comp:"))
            {
                visibleGameObjects.AddRange(objs);
                searchingComponents = true;
            }
            else
            {
                foreach (var obj in objs)
                {
                    if (obj.name.ToLower().StartsWith(searchText.ToLower()))
                    {
                        visibleGameObjects.Add(obj);
                    }
                }
                searchingComponents = false;
            }
        }

        void Start()
        {
            searchText = "";
            UpdateGameObjectList();

            windowRect = new Rect(50, 50, 300, 600);

            openWindows = new Dictionary<GameObject, Window>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) &&  Input.GetKeyDown(KeyCode.O))
            {
                visible = !visible;
            }
        }

        void OnGUI()
        {
            if (visible)
            {
                windowRect = GUI.Window(1001, windowRect, OnWindow, "Object explorer");

                foreach (var window in openWindows.Values)
                {
                    window.rect = GUILayout.Window(window.id, window.rect, OnPopupWindow, window.go.name);
                }
            }
        }

        void OnWindow(int windowId)
        {
            UpdateGameObjectList();

            float lineHeight = GUI.skin.box.lineHeight;

            GUILayout.BeginArea(new Rect(5f, lineHeight + 20f, 290f, 600f));

            GUILayout.Label("Search for objects:");
            searchText = GUILayout.TextField(searchText);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            try
            {
                for (int i = 0; i < visibleGameObjects.Count; i++)
                {
                    bool display = false;
                    if (searchingComponents)
                    {
                        foreach (var comp in visibleGameObjects[i].GetComponents<Component>())
                        {
                            if (comp.GetType().Name.ToLower().StartsWith(searchText.ToLower().Split(':')[1]))
                            {
                                display = true;
                            }
                        }
                    }
                    else
                    {
                        display = true;
                    }
                    if (display)
                    {
                        if (GUILayout.Button(visibleGameObjects[i].name))
                        {
                            Window win;
                            if (openWindows.ContainsKey(visibleGameObjects[i]))
                            {
                                win = openWindows[visibleGameObjects[i]];
                            }
                            else
                            {
                                win = new Window();
                                openWindows[visibleGameObjects[i]] = win;
                                win.go = visibleGameObjects[i];
                            }
                            win.id = 1002 + i;
                            win.rect = new Rect(windowRect.xMax + 10f, (windowRect.yMin + windowRect.height / 2) - 20f, 200f, 300f);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                // TODO: For some reason whenever you have 'comp:' left in the search box and then remove the ':',
                // one IndexOutOfRangeException is thrown. Have to figure out why and fix it, luckily it doesn't impact
                // any functionality of the object explorer.
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
            GUI.DragWindow();
        }

        void OnPopupWindow(int windowId)
        {
            Window win = null;

            var values = openWindows.Values;
            foreach (var val in values)
            {
                if (val.id == windowId)
                {
                    win = val;
                }
            }
            if (win == null)
            {
                Debug.LogError("[ObjectExplorer] OnPopupWindow for window without entry in openWindows!");
                return;
            }

            var components = win.go.GetComponents<Component>();
            foreach (var comp in components)
            {
                if (GUILayout.Button(comp.GetType().Name))
                {
                    MouseOrbit mo = Camera.main.GetComponent<MouseOrbit>();
                    Debug.Log(mo);
                    mo.target = comp.transform;
                }
            }
            if (GUILayout.Button("Close"))
            {
                openWindows.Remove(win.go);
            }
            GUI.DragWindow();
        }

    }
}
#endif