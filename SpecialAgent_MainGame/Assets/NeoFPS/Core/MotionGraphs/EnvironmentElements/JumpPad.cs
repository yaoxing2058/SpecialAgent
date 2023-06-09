﻿using NeoCC;
using NeoFPS.CharacterMotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoFPS
{
    [HelpURL("https://docs.neofps.com/manual/motiongraphref-mb-jumppad.html")]
    public class JumpPad : MonoBehaviour, INeoCharacterControllerHitHandler
    {
        [SerializeField, Tooltip("The transform parameter on the character's motion graph that should be set when they collide with this jump pad")]
        private string m_ParameterName = "jumpPad";
        [SerializeField, Tooltip("The velocity boost vector to be applied to the character's motion")]
        private Vector3 m_BoostVector = new Vector3(0f, 10f, 0f);
        [SerializeField, Tooltip("The coordinate space for the boost vector")]
        private Space m_BoostRelativeTo = Space.Self;
        [SerializeField, Tooltip("The amount of time between boosts")]
        private float m_Cooldown = 1f;

        private int m_ParameterHash = 0;

        private List<Jumper> m_Jumpers = new List<Jumper>();

        struct Jumper
        {
            public INeoCharacterController controller;
            public float activeTime;

            public Jumper(INeoCharacterController c, float t)
            {
                controller = c;
                activeTime = t;
            }

            public void Update()
            {
                activeTime -= Time.deltaTime;
            }
        }

        protected void Awake()
        {
            m_ParameterHash = Animator.StringToHash(m_ParameterName);
        }

        protected void Update()
        {
            float t = Time.time;
            while (m_Jumpers.Count > 0 && (m_Jumpers[0].activeTime + m_Cooldown) < t)
            {
                m_Jumpers.RemoveAt(0);
            }
        }

        bool CanJump(INeoCharacterController c)
        {
            for (int i = 0; i < m_Jumpers.Count; ++i)
            {
                if (m_Jumpers[i].controller == c)
                    return false;
            }
            return true;
        }

        public void OnNeoCharacterControllerHit(NeoCharacterControllerHit hit)
        {
            if (CanJump(hit.controller))
            {
                var m = hit.controller.GetComponent<MotionController>();
                if (m != null)
                {
                    m_Jumpers.Add(new Jumper(hit.controller, Time.time));

                    if (m_BoostRelativeTo == Space.World)
                        m.motionGraph.SetVector(m_ParameterHash, m_BoostVector);
                    else
                        m.motionGraph.SetVector(m_ParameterHash, transform.rotation * m_BoostVector);
                }
            }
        }
    }
}