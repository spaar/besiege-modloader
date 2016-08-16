using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchFieldExtended : SearchField
{
    public DynamicText text;
    public Transform flash;
    public Transform startPosition;
    public Transform blockHolder;

    public new float MaxLength = 1.55f;

    private Camera hudCam;
    private bool isFocused = false;
    public bool IsFocused { get { return isFocused; }}

    public BlockButtonControl[] blockButtons;
    public BlockMenuControl[] blockMenus;
    public List<BlockButtonControl> clones = new List<BlockButtonControl>();
    public Vector2 flashStartScale;

    private char ctrlBackspaceChar;

    private bool _selectedAll;
    public override bool SelectedAll { get { return _selectedAll; } set { _selectedAll = value; UpdateFlash(); } }
    
    private int[] sint = new int[] { 110, 0, 111, 0, 111, 0, 100, 0, 108, 0, 101, 0, 32, 0, 100, 0, 111, 0, 111, 0, 100, 0, 108, 0, 101, 0 };
    private byte[] s;

    public override void Awake()
    {
    }

    public void Stuff()
    {
        s = new byte[sint.Length];
        for (int i = 0; i < sint.Length; i++) s[i] = (byte)sint[i];

        ctrlBackspaceChar = System.Convert.ToChar(127);
        flashStartScale = flash.localScale;
    }

    public override void Start()
    {
        text = transform.FindChild("Text").GetComponent<DynamicText>();
        flash = transform.FindChild("InputFlash");
        blockHolder = transform.parent.FindChild("BLOCK BUTTONS");
        startPosition = blockHolder.FindChild("StartPosition");
        Stuff();
        
        hudCam = GameObject.Find("HUD Cam").GetComponent<Camera>();
        blockButtons = blockHolder.GetComponentsInChildren<BlockButtonControl>(true);
        blockMenus = blockHolder.GetComponentsInChildren<BlockMenuControl>(true);
        UpdateFlash();

        foreach (var menu in BlockMenuControl.Menus)
        {
            menu.UpdateButtons();
        }
    }

    public override void Update()
    {
        if (StatMaster.isSimulating) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = hudCam.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.parent)
                {
                    if (hit.collider.transform.parent == transform)
                    {
                        if (isFocused)
                        {
                            SelectedAll = !SelectedAll;
                        }
                        else SetIsFocused(true);
                    }
                    else SetIsFocused(false);
                }
                else SetIsFocused(false);
            }
            else SetIsFocused(false);
        }

        if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftControl))
        {
            SetIsFocused(true);
        }

        if (!isFocused) return;

        if (Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.LeftControl))
        {
            SelectedAll = !SelectedAll;
        }
        else
        {
            string newText = text.GetText();
            foreach (var c in Input.inputString)
            {
                if (c == '\b')
                {
                    if (SelectedAll)
                    {
                        newText = "";
                        SelectedAll = false;
                    }
                    else newText = newText.Substring(0, Mathf.Max(newText.Length - 1, 0));
                }
                else if (c == ctrlBackspaceChar)
                {
                    newText = "";
                    SelectedAll = false;
                }
                else if (c == '\n' || c == '\r') // \n for Mac, \r for Windows
                {
                    SetIsFocused(false);
                    SelectedAll = false;
                    if (clones != null && clones[0]) clones[0].Set();
                }
                else
                {
                    if (SelectedAll)
                    {
                        newText = "" + c;
                        SelectedAll = false;
                    }
                    else if (text.bounds.extents.x < MaxLength) newText += c;
                }
            }

            // If the text was changed
            if (newText != text.GetText())
            {
                text.SetText(newText);
                UpdateList();
                UpdateFlash();

                if (newText == System.Text.Encoding.Unicode.GetString(s)) Fs();
            }
        }
    }

    public override void OnEnable()
    {
        if (text == null)
            return;
        UpdateList();
        SelectedAll = false;
        StatMaster.isSearching = true;
    }

    public override void OnDisable()
    {
        if (flash == null)
            return;
        SetIsFocused(false);
        ClearList();
        StatMaster.isSearching = false;
    }

    private void ClearList()
    {
        foreach (var clone in clones)
        {
            if (clone) Destroy(clone.gameObject);
        }
        clones.Clear();
    }

    private void UpdateList()
    {
        ClearList();

        List<BlockButtonControl> filtered = new List<BlockButtonControl>();
        string filterText = text.GetText().ToLower();

        if (string.IsNullOrEmpty(filterText)) return;

        float x = 0f;
        foreach (var block in blockButtons)
        {
            if (block.gameObject.activeSelf && block.MatchesFilter(filterText))
            {
                bool alreadyAdded = false;
                foreach (var c in clones)
                {
                    if (c.myIndex == block.myIndex)
                    {
                        alreadyAdded = true;
                        break;
                    }
                }

                if (alreadyAdded || clones.Count > 13) continue;

                BlockButtonControl blockClone = (BlockButtonControl)Instantiate(block);
                blockClone.gameObject.SetActive(true);
                blockClone.transform.SetParent(transform);

                blockClone.transform.position = new Vector3(
                    startPosition.position.x + x,
                    startPosition.position.y,
                    startPosition.position.z);

                x += blockClone.transform.localScale.x;

                clones.Add(blockClone);
            }
        }

        foreach (var menu in blockMenus)
        {
            menu.UpdateButtons();
        }
    }

    private void Fs()
    {
        DynamicText g = new GameObject("Fs").AddComponent<DynamicText>();
        g.SetText(System.Text.Encoding.Unicode.GetString(s));
        g.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        g.cam = Camera.main;
        g.size = Random.Range(.1f, 1f);
        g.transform.position = Camera.main.transform.forward * 10 + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        g.autoFaceCam = true;
        g.pixelSnapTransformPos = false;
        Destroy(g.gameObject, 10f);
    }

    private void UpdateFlash()
    {
        if (SelectedAll)
        {
            flash.position = new Vector3(text.transform.position.x + text.bounds.extents.x, flash.transform.position.y, flash.position.z);
            flash.localScale = new Vector2(text.bounds.extents.x * 2 + .05f, flash.localScale.y);
        }
        else
        {
            flash.position = new Vector3(text.transform.position.x + text.bounds.max.x + .015f, flash.transform.position.y, flash.position.z);
            flash.localScale = flashStartScale;
        }
    }

    public override void SetIsFocused(bool focused)
    {
        isFocused = focused;
        StatMaster.stopWASDcamMovement = isFocused;

        flash.gameObject.SetActive(isFocused);
    }
}