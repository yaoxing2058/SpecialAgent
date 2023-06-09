﻿using NeoSaveGames;
using NeoSaveGames.Serialization;
using UnityEngine;

namespace NeoFPS.WieldableTools
{
    [HelpURL("https://docs.neofps.com/manual/inputref-mb-consumeinventoryitemtoolaction.html")]
    public class ConsumeInventoryItemToolAction : BaseWieldableToolModule, INeoSerializableComponent
    {
        [SerializeField, FlagsEnum, Tooltip("When should the items be consumed.")]
        private WieldableToolActionTiming m_Timing = WieldableToolActionTiming.Start;
        [SerializeField, FpsInventoryKey(required = true), Tooltip("The inventory ID of the object to consume.")]
        private int m_ItemKey = 0;
        [SerializeField, Tooltip("How many of the item should be consumed when triggered or ticked.")]
        private int m_ConsumeAmount = 1;
        [SerializeField, Tooltip("The items should be consumed every nth fixed update frame in continuous mode.")]
        private int m_ConsumeInterval = 1;
        [SerializeField, Tooltip("Should the item be consumed on the first frame of the continuous action, or should it wait for the first interval to elapse.")]
        private bool m_Instant = true;

        private IInventory m_Inventory = null;
        private IInventoryItem m_ToolItem = null;
        private IInventoryItem m_ConsumeItem = null;
        private int m_CountDown = 0;

        public override bool isValid
        {
            get { return m_ItemKey != 0 && m_Timing != 0; }
        }

        public override WieldableToolActionTiming timing
        {
            get { return m_Timing; }
        }

        protected void OnValidate()
        {
            m_ConsumeAmount = Mathf.Clamp(m_ConsumeAmount, 1, 1000);
            m_ConsumeInterval = Mathf.Clamp(m_ConsumeInterval, 1, 500);
        }

        protected void Start()
        {
            m_ToolItem = GetComponent<IInventoryItem>();
            if (m_ToolItem != null)
            {
                m_ToolItem.onAddToInventory += OnInventoryChanged;
                m_ToolItem.onRemoveFromInventory += OnInventoryChanged;
                OnInventoryChanged();
            }
        }

        void OnInventoryChanged()
        {
            if (m_Inventory != null)
            {
                m_Inventory.onItemAdded -= OnItemAdded;
                m_Inventory.onItemRemoved -= OnItemRemoved;
            }

            m_Inventory = m_ToolItem.inventory;

            if (m_Inventory == null)
                m_ConsumeItem = null;
            else
            {
                m_Inventory.onItemAdded += OnItemAdded;
                m_Inventory.onItemRemoved += OnItemRemoved;
                m_ConsumeItem = m_Inventory.GetItem(m_ItemKey);
            }
        }

        protected void OnEnable()
        {
            // Set interval for continuous
            if (m_Instant)
                m_CountDown = 1;
            else
                m_CountDown = m_ConsumeInterval;
        }

        protected void OnDisable()
        {
            m_CountDown = 0;
        }

        private void OnItemAdded(IInventoryItem item)
        {
            if (item.itemIdentifier == m_ItemKey)
                m_ConsumeItem = item;
        }

        private void OnItemRemoved(IInventoryItem item)
        {
            if (item.itemIdentifier == m_ItemKey)
            {
                m_ConsumeItem = null;
                if (m_CountDown > 0)
                    tool.Interrupt();
            }
        }

        public override void FireStart()
        {
            Debug.Log("Fire start");
            if (m_ConsumeItem as Component != null)
                m_ConsumeItem.quantity -= m_ConsumeAmount;
        }

        public override void FireEnd(bool success)
        {
            Debug.Log("Fire end");
            if (m_ConsumeItem as Component != null)
                m_ConsumeItem.quantity -= m_ConsumeAmount;

            // Set interval for continuous
            if (m_Instant)
                m_CountDown = 1;
            else
                m_CountDown = m_ConsumeInterval;
        }

        public override bool TickContinuous()
        {
            var consumeComponent = m_ConsumeItem as Component;
            if (consumeComponent != null) 
            {
                if (--m_CountDown <= 0)
                {
                    Debug.Log("Consume");

                    m_CountDown = m_ConsumeInterval;

                    bool remaining = m_ConsumeItem.quantity > m_ConsumeAmount;
                    m_ConsumeItem.quantity -= m_ConsumeAmount;

                    return remaining;
                }
                else
                    return true;
            }
            else
                return false;
        
        }

        public override void Interrupt()
        {
            Debug.Log("Interrupted");
            base.Interrupt();
        }

        private static readonly NeoSerializationKey k_CountDownKey = new NeoSerializationKey("countdown");

        public void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
            writer.WriteValue(k_CountDownKey, m_CountDown);
        }

        public void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            reader.TryReadValue(k_CountDownKey, out m_CountDown, m_CountDown);
        }
    }
}