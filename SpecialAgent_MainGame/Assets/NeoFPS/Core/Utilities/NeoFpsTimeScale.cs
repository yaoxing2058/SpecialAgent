﻿using NeoSaveGames;
using NeoSaveGames.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace NeoFPS
{
    public class NeoFpsTimeScale : MonoBehaviour, INeoSerializableComponent
    {
        private const float k_MaxTimeScale = 10f;
        private const float k_MinTimeScale = 0.001f;

        public static event UnityAction<float> onTimeScaleChanged;

        private static float m_ResumeTimeScale = 1f;
        private static float m_FixedDeltaTime = Time.fixedDeltaTime;

        public static bool isPaused
        {
            get { return Time.timeScale == 0f; }
        }

        public static float timeScale
        {
            get { return Time.timeScale; }
            set
            {
                value = Mathf.Clamp(value, k_MinTimeScale, k_MaxTimeScale);
                if (Time.timeScale != value)
                {
                    Time.timeScale = value;
                    Time.fixedDeltaTime = m_FixedDeltaTime * value;
                    if (onTimeScaleChanged != null)
                        onTimeScaleChanged(value);
                }
            }
        }

        public static void FreezeTime()
        {
            if (Time.timeScale != 0f)
            {
                m_ResumeTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
        }

        public static void ResumeTime()
        {
            if (Time.timeScale == 0f)
                Time.timeScale = m_ResumeTimeScale;
        }

        #region INeoSerializableComponent IMPLEMENTATION

        private static readonly NeoSerializationKey k_TimeScaleKey = new NeoSerializationKey("timeScale");
        private static readonly NeoSerializationKey k_ResumeScaleKey = new NeoSerializationKey("resumeScale");

        public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
            if (Time.timeScale != 1f && Time.timeScale != 0f)
            {
                writer.WriteValue(k_TimeScaleKey, Time.timeScale);
                writer.WriteValue(k_ResumeScaleKey, m_ResumeTimeScale);
            }
        }

        public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            float ts;
            if (reader.TryReadValue(k_TimeScaleKey, out ts, 1f) && ts != 1f)
            {
                reader.TryReadValue(k_ResumeScaleKey, out m_ResumeTimeScale, m_ResumeTimeScale);
                Time.timeScale = ts;
            }
        }

        #endregion
    }
}