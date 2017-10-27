using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Highlight(WorldSpaceCallout callout);
    void Unhighlight(WorldSpaceCallout callout);
    void Interact(WorldSpaceCallout callout);
}

public interface IUsable
{
    void Use();
}
