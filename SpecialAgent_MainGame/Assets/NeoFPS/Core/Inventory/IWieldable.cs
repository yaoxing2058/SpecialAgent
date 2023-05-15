﻿using UnityEngine;
using UnityEngine.Events;

namespace NeoFPS
{
    public interface IWieldable
    {
        ICharacter wielder { get; }
        void Select();
        void DeselectInstant();
        Waitable Deselect();

        // isBusy
        bool isBlocked { get; }
        void AddBlocker(Object o);
        void RemoveBlocker(Object o);

        event UnityAction<bool> onBlockedChanged;
        event UnityAction<ICharacter> onWielderChanged;

        T GetComponent<T>();
    }
}