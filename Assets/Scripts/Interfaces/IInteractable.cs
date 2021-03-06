﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Highlight(WorldSpaceCallout callout);
    void Unhighlight(WorldSpaceCallout callout);
    void Interact(WorldSpaceCallout callout, int characterSaveSlot);
}

public interface IUsable
{
    void Use(Character target);
}

public interface IEquippable
{
    void Equip();
    void Unequip();
}