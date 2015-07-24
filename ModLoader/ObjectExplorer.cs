using System.Collections.Generic;
using UnityEngine;

using System;

#if DEV_BUILD
namespace spaar
{
    /// <summary>
    /// In-game object explorer. Only enabled in developer builds.
    /// </summary>
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
                    if (obj.name.ToLower().Contains(searchText.ToLower()))
                    {
                        visibleGameObjects.Add(obj);
                    }
                }
                searchingComponents = false;
            }
        }

       private  void Start()
        {
            searchText = "";
            UpdateGameObjectList();

            windowRect = new Rect(50, 50, 300, 600);

            openWindows = new Dictionary<GameObject, Window>();
        }

        private void Update()
        {
            if (Input.GetKey(Keys.getKey("ObjectExplorer").Modifier) && Input.GetKeyDown(Keys.getKey("ObjectExplorer").Trigger))
            {
                visible = !visible;
            }
        }

        private void OnGUI()
        {
            if (visible)
            {
                windowRect = GUI.Window(1001, windowRect, OnWindow, "Object explorer");

                foreach (var window in new List<Window>(openWindows.Values))
                {
                    if (window.go == null)
                    {
                        // GameObject was destroyed while the window was open
                        openWindows.Remove(window.go);
                        continue;
                    }
                    window.rect = GUILayout.Window(window.id, window.rect, OnPopupWindow, window.go.name);
                }
            }
        }

        private void OnWindow(int windowId)
        {
            float lineHeight = GUI.skin.box.lineHeight;

            GUILayout.BeginArea(new Rect(5f, lineHeight + 20f, 290f, 600f));

            GUILayout.Label("Search for objects:");
            searchText = GUILayout.TextField(searchText);
            UpdateGameObjectList();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < visibleGameObjects.Count; i++)
            {
                bool display = false;
                if (searchingComponents)
                {
                    foreach (var comp in visibleGameObjects[i].GetComponents<Component>())
                    {
                        if (comp.GetType().Name.ToLower().Contains(searchText.ToLower().Split(':')[1]))
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

            GUILayout.EndScrollView();

            GUILayout.EndArea();
            GUI.DragWindow();
        }

        private void OnPopupWindow(int windowId)
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

            if (win.go.tag != "Untagged")
            {
                GUILayout.Label("Tag: " + win.go.tag);
            }
            if (win.go.layer != 0)
            {
                GUILayout.Label("Layer: " + win.go.layer);
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
