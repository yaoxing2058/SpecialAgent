﻿using NeoSaveGames;
using NeoSaveGames.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NeoFPS.WieldableTools
{
    [HelpURL("https://docs.neofps.com/manual/inputref-mb-flashlighttoolmodule.html")]
    public class FlashlightToolModule : BaseWieldableToolModule, IWieldableFlashlight, INeoSerializableComponent
    {
        [SerializeField, NeoObjectInHierarchyField(false, required = true), Tooltip("A child object with a light component attached.")]
        private GameObject m_LightObject = null;
        [SerializeField, Tooltip("Should the flashlight be enabled on start.")]
        private bool m_StartEnabled = false;

        [SerializeField, Tooltip("An event fired when the flashlight is switched on")]
        private UnityEvent m_OnToggleOn = null;
        [SerializeField, Tooltip("An event fired when the flashlight is switched off")]
        private UnityEvent m_OnToggleOff = null;

        private Light m_Light = null;
        private float m_FullIntensity = 0f;

        public bool on
        {
            get { return m_LightObject.activeSelf; }
            set
            {
                if (m_LightObject.activeSelf != value)
                {
                    m_LightObject.SetActive(value);
                    if (value)
                        m_OnToggleOn.Invoke();
                    else
                        m_OnToggleOff.Invoke();
                }
            }
        }

        private float m_Brightness = 1f;
        public float brightness
        {
            get { return m_Brightness; }
            set
            {
                m_Brightness = Mathf.Clamp01(value);
                m_Light.intensity = m_Brightness * m_FullIntensity;
            }
        }

        public override WieldableToolActionTiming timing
        {
            get { return k_TimingsStartOnly; }
        }

        public override bool isValid
        {
            get { return true; }
        }

        protected void OnValidate()
        {
            if (m_LightObject == gameObject)
            {
                Debug.LogError("Light object should be a child of the flashlight");
                m_LightObject = null;
            }
        }

        protected void OnDisable()
        {
            on = false;
        }

        public override void Initialise(IWieldableTool t)
        {
            base.Initialise(t);

            if (m_LightObject != null)
            {
                m_Light = m_LightObject.GetComponent<Light>();
                m_FullIntensity = m_Light.intensity;
                m_Light.intensity = m_FullIntensity * m_Brightness;
                m_LightObject.SetActive(m_StartEnabled);
            }
            else
                enabled = false;
        }

        public override void FireStart()
        {
            Toggle();
        }

        public override void FireEnd(bool success) { }

        public override bool TickContinuous() { return true; }

        public void Toggle()
        {
            on = !on;
        }

        static readonly NeoSerializationKey k_OnKey = new NeoSerializationKey("on");
        static readonly NeoSerializationKey k_BrightnessKey = new NeoSerializationKey("brightness");

        public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
            writer.WriteValue(k_OnKey, on);
            writer.WriteValue(k_BrightnessKey, m_Brightness);
        }

        public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            reader.TryReadValue(k_BrightnessKey, out m_Brightness, m_Brightness);
            reader.TryReadValue(k_OnKey, out m_StartEnabled, m_StartEnabled);
        }
    }
}