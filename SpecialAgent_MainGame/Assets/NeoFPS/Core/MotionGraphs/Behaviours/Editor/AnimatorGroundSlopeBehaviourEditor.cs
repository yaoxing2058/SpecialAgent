﻿#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
using NeoFPS.CharacterMotion;
using NeoFPS.CharacterMotion.Behaviours;
using NeoFPS.CharacterMotion.Parameters;

namespace NeoFPSEditor.CharacterMotion.Behaviours
{
    [MotionGraphBehaviourEditor(typeof(AnimatorGroundSlopeBehaviour))]
    public class AnimatorGroundSlopeBehaviourEditor : MotionGraphBehaviourEditor
    {
        protected override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SlopeParamName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DirectionParamName"));
        }
    }
}

#endif
