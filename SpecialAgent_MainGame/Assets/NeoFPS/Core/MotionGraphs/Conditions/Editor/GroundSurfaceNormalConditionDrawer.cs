﻿#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NeoFPS.CharacterMotion.Conditions;

namespace NeoFPSEditor.CharacterMotion.Conditions
{
    [MotionGraphConditionDrawer(typeof(GroundSurfaceNormalCondition))]
    [HelpURL("http://docs.neofps.com/manual/motiongraphref-mgc-groundsurfacenormalcondition.html")]
    public class GroundSurfaceNormalConditionDrawer : MotionGraphConditionDrawer
    {
        protected override void Inspect (Rect line1)
        {
            Rect r1 = line1;
            Rect r2 = r1;
            Rect r3 = r1;
            r1.width *= 0.5f;
            r2.width *= 0.2f;
            r3.width *= 0.3f;
            r2.x += r1.width;
            r3.x += r1.width + r2.width + 2f;
            r3.width -= 2f;
            r1.width -= 2f;

            EditorGUI.LabelField(r1, "Slope Angle");

            // Draw the comparison type dropdown
            var comparisonTypeString = GetComparisonTypeString(serializedObject.FindProperty("m_Comparison").enumValueIndex);
            if (EditorGUI.DropdownButton(r2, new GUIContent(comparisonTypeString), FocusType.Passive))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < 5; ++i)
                    gm.AddItem(new GUIContent(GetComparisonTypeString(i)), false, OnComparisonTypeDropdownSelect, i);
                gm.ShowAsContext();
            }

            // Draw the compare value property
            EditorGUI.PropertyField(r3, serializedObject.FindProperty("m_SlopeAngle"), new GUIContent());
        }

        void OnComparisonTypeDropdownSelect (object o)
        {
            int index = (int)o;
            serializedObject.FindProperty("m_Comparison").enumValueIndex = index;
            serializedObject.ApplyModifiedProperties();
        }

        string GetComparisonTypeString (int i)
        {
            switch (i)
            {
                case 0:
                    return ">";
                case 1:
                    return "> or =";
                case 2:
                    return "<";
                case 3:
                    return "< or =";
                case 4:
                    return "=";
            }
            return "=";
        }
    }
}

#endif