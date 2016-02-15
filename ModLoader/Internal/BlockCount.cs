using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class BlockCount : MonoBehaviour
  {
    private Machine machine;
    private TextMesh blockCountMesh;

    private void Update()
    {
      if (machine == null)
      {
        machine = Machine.Active();
      }
      if (blockCountMesh == null)
      {
        blockCountMesh = transform.FindChild("BlockCounter")
          .GetComponent<TextMesh>();
      }

      blockCountMesh.text = (machine.Blocks.Count - 1).ToString();
    }
  }
}