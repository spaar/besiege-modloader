using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spaar
{
    class ObjectExplorer : MonoBehaviour
    {

        class Window
        {
            public int id;
            public Rect rect;
        }

        List<GameObject> gameObjectList;
        List<Window> openWindows;

        Vector2 scrollPosition = Vector2.zero;
        Rect windowRect;
        string searchText;
        bool searchingComponents;

        private bool visible = false;

        void UpdateGameObjectList()
        {
            GameObject[] objs = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
            gameObjectList = new List<GameObject>();
            if (searchText.ToLower().StartsWith("comp:"))
            {
                gameObjectList.AddRange(objs);
                searchingComponents = true;
            }
            else
            {
                foreach (var obj in objs)
                {
                    if (obj.name.ToLower().StartsWith(searchText.ToLower()))
                    {
                        gameObjectList.Add(obj);
                    }
                    if (obj.name == "BUILDER")
                    {
                        ModLoader.AddPiece = obj.GetComponent<AddPiece>();
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

            openWindows = new List<Window>();
        }

        void Update()
        {
            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.U))
            //{
            //    if (ModLoader.AddPiece != null)
            //    {
            //        ModLoader.AddPiece.Undoy();
            //    }
            //}
            //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            //{
            //    if (ModLoader.AddPiece != null)
            //    {
            //        ModLoader.AddPiece.Redoy();
            //    }
            //}

            if (Input.GetKey(KeyCode.LeftControl) &&  Input.GetKeyDown(KeyCode.O))
            {
                visible = !visible;
            }

            ////DEBUG
            //if (Input.GetKeyDown(KeyCode.I))
            //{
            //    MouseOrbit mo = Camera.main.GetComponent<MouseOrbit>();
            //    Debug.Log(mo.target.gameObject.name);
            //}
            ////DEBUG
            //if (Input.GetKeyDown(KeyCode.B))
            //{
            //    var addPiece = ModLoader.AddPiece;
            //    var blockTypes = addPiece.blockTypes;
            //    for (int i = 0; i < blockTypes.Length; i++)
            //    {
            //        Debug.Log(blockTypes[i].gameObject.name + " -> " + blockTypes[i].gameObject.GetComponent<Rigidbody>().mass);
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    AddPiece.currentBlockType = 45;
            //}
        }

        void OnGUI()
        {
            UpdateGameObjectList();
            if (visible)
            {
                windowRect = GUI.Window(1001, windowRect, OnWindow, "Object explorer");

                for (int i = 0; i < openWindows.Count; i++)
                {
                    openWindows[i].rect = GUILayout.Window(openWindows[i].id, openWindows[i].rect, OnPopupWindow, gameObjectList[openWindows[i].id - 1002].name);
                }
            }
        }

        void OnWindow(int windowId)
        {
            float lineHeight = GUI.skin.box.lineHeight;

            GUILayout.BeginArea(new Rect(5f, lineHeight + 20f, 290f, 600f));

            GUILayout.Label("Search for objects:");
            searchText = GUILayout.TextField(searchText);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < gameObjectList.Count; i++)
            {
                bool display = false;
                if (searchingComponents)
                {
                    foreach (var comp in gameObjectList[i].GetComponents<Component>())
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
                    if (GUILayout.Button(gameObjectList[i].name))
                    {
                        Window win = new Window();
                        win.id = 1002 + i;
                        win.rect = new Rect(windowRect.xMax + 10f, (windowRect.yMin + windowRect.height / 2) - 20f, 200f, 300f);
                        openWindows.Add(win);

                        ////DEBUG
                        ////if (gameObjectList[i].name.StartsWith("FullKnight"))
                        ////{
                        //foreach (Transform child in gameObjectList[i].transform)
                        //{
                        //    Debug.Log("Child: " + child.gameObject.name);
                        //    foreach (var comp in child.gameObject.GetComponents<Component>())
                        //    {
                        //        Debug.Log(" > " + comp.GetType().Name);
                        //        if (comp is Rigidbody)
                        //        {
                        //            Debug.Log("(Mass: " + ((Rigidbody)comp).mass + ")");
                        //            Debug.Log("(Drag: " + ((Rigidbody)comp).mass + ")");
                        //            Debug.Log("(Angular Drag: " + ((Rigidbody)comp).angularDrag + ")");
                        //        }
                        //    }
                        //    foreach (Transform child2 in child)
                        //    {
                        //        Debug.Log(" ->Child2: " + child2.gameObject.name);
                        //        foreach (var comp in child2.gameObject.GetComponents<Component>())
                        //        {
                        //            Debug.Log("    > " + comp.GetType().Name);
                        //            if (comp is Rigidbody)
                        //            {
                        //                Debug.Log("(Mass: " + ((Rigidbody)comp).mass + ")");
                        //                Debug.Log("(Drag: " + ((Rigidbody)comp).mass + ")");
                        //                Debug.Log("(Angular Drag: " + ((Rigidbody)comp).angularDrag + ")");
                        //            }
                        //        }
                        //        foreach (Transform child3 in child2)
                        //        {
                        //            Debug.Log("   ->Child3: " + child3.gameObject.name);
                        //        }
                        //    }
                        //}
                        ////}
                    }
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
            GUI.DragWindow();
        }

        void OnPopupWindow(int windowId)
        {
            GameObject go = gameObjectList[windowId - 1002];

            var components = go.GetComponents<Component>();
            foreach (var comp in components)
            {
                if (GUILayout.Button(comp.GetType().Name))
                {
                    MouseOrbit mo = Camera.main.GetComponent<MouseOrbit>();
                    mo.target = comp.transform;

                    ////DEBUG
                    //if (comp is Rigidbody)
                    //{
                    //    Debug.Log("Mass: " + ((Rigidbody)comp).mass);
                    //    Debug.Log("Drag: " + ((Rigidbody)comp).drag);
                    //    Debug.Log("(Angular Drag: " + ((Rigidbody)comp).angularDrag + ")");
                    //}
                }
            }
            if (GUILayout.Button("Close"))
            {
                int index = openWindows.FindIndex((Window w) => { return w.id == windowId; });
                openWindows.RemoveAt(index);
            }
            GUI.DragWindow();
        }

    }
}
