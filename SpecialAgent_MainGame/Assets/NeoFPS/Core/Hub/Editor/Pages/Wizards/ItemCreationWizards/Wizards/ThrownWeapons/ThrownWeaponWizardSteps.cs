﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using NeoFPS.ModularFirearms;
using NeoFPS;

namespace NeoFPSEditor.Hub.Pages.ItemCreationWizards.ThrownWeapons
{
    public static class ThrownWeaponWizardSteps
    {
        public const string root = "root";
        public const string viewModel = "viewModel";
        public const string existingAnim = "existingAnim";
        public const string createAnim = "createAnim";
        public const string audio = "audio";
    }
}

#endif