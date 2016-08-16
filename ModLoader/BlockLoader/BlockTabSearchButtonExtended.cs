using UnityEngine;
using System.Collections;

public class BlockTabSearchButtonExtended : BlockTabButton
{
    public SearchField searchField;

    public override void SetVis(bool state)
    {
        base.SetVis(state);
        searchField.gameObject.SetActive(state);
        if (state) StartCoroutine(SetFocus());
    }

    private IEnumerator SetFocus()
    {
        yield return null;
        searchField.SetIsFocused(true);
        yield break;
    }
}