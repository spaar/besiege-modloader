using spaar.ModLoader.UI;
using UnityEngine;

#if DEV_BUILD
namespace spaar.ModLoader.Internal.Tools
{
  /// <summary>
  /// In-game object explorer. Only enabled in developer builds.
  /// </summary>
  public class ObjectExplorer : SingleInstance<ObjectExplorer>
  {
    public override string Name
    {
      get { return "spaar's Mod Loader: Object Explorer"; }
    }

    public string WindowTitle = "Object Explorer";

    public HierarchyPanel HierarchyPanel { get; private set; }
    public InspectorPanel InspectorPanel { get; private set; }

    private readonly int windowID = Util.GetWindowID();
    private Rect windowRect = new Rect(20, 20, 800, 600);

    private bool isVisible = false;

    private void Start()
    {
      ModLoader.MakeModule(this);

      HierarchyPanel = gameObject.AddComponent<HierarchyPanel>();
      InspectorPanel = gameObject.AddComponent<InspectorPanel>();
    }

    private void Update()
    {
      if (Keys.K["ObjectExplorer"])
      {
        isVisible = !isVisible;
      }
    }

    private void OnGUI()
    {
      GUI.skin = ModGUI.Skin;

      if (isVisible)
      {
        windowRect = GUILayout.Window(windowID, windowRect, DoWindow,
          WindowTitle);
      }
    }

    private void DoWindow(int id)
    {
      GUILayout.BeginHorizontal();

      HierarchyPanel.Display();
      InspectorPanel.Display();

      GUILayout.EndHorizontal();

      GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
    }
  }
}
#endif
