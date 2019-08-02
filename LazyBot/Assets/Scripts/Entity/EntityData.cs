using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;


namespace LazyBot.Entity.Data
{
    /// <summary>
    /// Used to determine the state action
    /// </summary>
    public enum StateAction { unknown, Validate, OnEnter, Execute, OnExit }

    /// <summary>
    /// Float data.
    /// Contains min, max value and regenerate rate.
    /// Also other data, to perform control of float data.
    /// </summary>
    [System.Serializable]
    public class FloatEntityData 
    {
        /// <summary>
        /// Is value unchangeable.
        /// </summary>
        [SerializeField] protected bool m_isLock = false;
        /// <summary>
        /// Is value unlimited.
        /// </summary>
        [SerializeField] protected bool m_isUnlimited = false;
        /// <summary>
        /// Is value can be regenerated.
        /// </summary>
        [SerializeField] protected bool m_isRegenerate = true;

        [SerializeField] protected float m_value;

        /// <summary>
        /// Startup value.
        /// </summary>
        [SerializeField] protected float m_valueStart;
        /// <summary>
        /// Regenerate rate.
        /// </summary>
        [SerializeField] protected float m_valueRegenerateRate;
        /// <summary>
        /// Low acceptable value.
        /// </summary>
        [SerializeField] protected float m_valueLow;

        /// <summary>
        /// Maximum acceptable value.
        /// </summary>
        [SerializeField] protected float m_valueMax;
        /// <summary>
        /// Minimum acceptable value.
        /// </summary>
        [SerializeField] protected float m_valueMin;
        
        public float RegenerateRate
        {
            get { return this.m_valueRegenerateRate; }
        }
        public bool IsRegenerate
        {
            get { return this.m_isRegenerate; }
            set { this.m_isRegenerate = value; }
        }
        public bool IsUnlimited
        {
            get { return this.m_isUnlimited; }
            set { this.m_isUnlimited = value; }
        }
        public bool IsValueLow
        {
            get { return (m_value <= m_valueLow); }
        }
        public bool IsLock
        {
            get { return this.m_isLock; }
            set { this.m_isLock = value; }
        }
        public bool IsLow
        {
            get { return (this.m_value <= this.m_valueMin); }
        }
        public bool IsMax
        {
            get { return (this.m_value >= this.m_valueMax); }
        }
        /// <summary>
        /// Startup value.
        /// </summary>
        public float Start
        {
            get { return this.m_valueStart; }
        }
        public float Value
        {
            get { return this.m_value; }
        }
        /// <summary>
        /// Maximum acceptable value.
        /// </summary>
        public float Max
        {
            get { return this.m_valueMax; }
        }
        /// <summary>
        /// Minimum acceptable value.
        /// </summary>
        public float Min
        {
            get { return this.m_valueMin; }
        }
        /// <summary>
        /// Low acceptable value.
        /// </summary>
        public float Low
        {
            get { return this.m_valueLow; }
        }


        public FloatEntityData()
        {
            m_value = m_valueStart;
        }


        protected bool IsOnLimit()
        {
            return ((m_value <= m_valueMin) || (m_value >= m_valueMax));
        }

        /// <summary>
        /// Is value can be changed to the passed value.
        /// </summary>
        /// <param name="value">Change value.</param>
        public bool IsChangable(float value)
        {
            if (m_isUnlimited) return true;
            else if (IsOnLimit()) return false;

            float newValue = m_value + value;
            return (newValue <= m_valueMax) && (newValue >= m_valueMin);
        }

        /// <summary>
        /// Updates value based on regenerate rate and deltaTime.
        /// </summary>
        /// <param name="deltaTime">Time elapsed from previous frame.</param>
        /// <returns>Is regenerate was successful.</returns>
        public virtual bool Regenerate(float deltaTime = 1.0f)
        {
            if (!m_isRegenerate) return false;

            Change(m_valueRegenerateRate * deltaTime);

            return true;
        }        

        /// <summary>
        /// Changes value to the passed value.
        /// </summary>
        /// <param name="value">Change value.</param>
        /// <param name="asPossible">Change value on maximum allowable value.</param>
        /// <returns>Is value was successful changed.</returns>
        public virtual bool Change(float value, bool asPossible = false)
        {
            if (m_isUnlimited) return true;
            else if ((m_isLock) ||
                ((!asPossible) && (!IsChangable(value)))) return false;

            m_value += value;
            if (m_value > m_valueMax) m_value = m_valueMax;
            else if (m_value < m_valueMin) m_value = m_valueMin;

            return true;
        }


        public static FloatEntityData operator -(FloatEntityData valueObj, float value)
        {
            valueObj.Change(-value);
            return valueObj;
        }

        public static FloatEntityData operator +(FloatEntityData valueObj, float value)
        {
            valueObj.Change(value);
            return valueObj;
        }

    }

    /// <summary>
    /// Float data with a slider attached to it.
    /// </summary>
    [System.Serializable]    
    public class SliderFloatEntityData : FloatEntityData
    {
        [SerializeField] protected Slider m_slider;


        /// <summary>
        /// Updates slider value based on float value.
        /// </summary>
        private void UpdateSlider()
        {
            if (!m_slider) return;

            m_slider.value = 
                LazyBot.Utility.Data.FloatHelper.Map(m_value, m_valueMin, m_valueMax, m_slider.minValue, m_slider.maxValue);
        }


        /// <summary>
        /// Updates value based on regenerate rate and deltaTime and updates slider.
        /// </summary>
        /// <param name="deltaTime">Time elapsed from previous frame.</param>
        /// <returns>Is regenerate was successful.</returns>
        public override bool Regenerate(float deltaTime = 1.0f)
        {
            if (!base.Regenerate(deltaTime)) return false;

            UpdateSlider();
            return true;
        }

        /// <summary>
        /// Changes value to the passed value and updates slider.
        /// </summary>
        /// <param name="value">Change value.</param>
        /// <param name="asPossible">Change value on maximum allowable value.</param>
        /// <returns>Is value was successful changed.</returns>
        public override bool Change(float value, bool asPossible = false)
        {
            if (!base.Change(value, asPossible)) return false;

            UpdateSlider();
            return true;
        }
    }

    /// <summary>
    /// Integer mask.
    /// </summary>
    [System.Serializable]
    public class IntMask : IEnumerable<uint>
    {
        /// <summary>
        /// Represents all possible values.
        /// </summary>
        [SerializeField] private string m_values;

        /// <summary>
        /// Separates possible value.
        /// </summary>
        private char m_separator = '/';
        /// <summary>
        /// Values as mask of bit set.
        /// </summary>
        private ulong m_mask;

        /// <summary>
        /// All possible values.
        /// </summary>
        public string Values
        {
            get { return this.m_values; }
        }
        /// <summary>
        /// Values as mask of bit set.
        /// </summary>
        public ulong Mask
        {
            get { return this.m_mask; }
        }


        /// <summary>
        /// Validates values.
        /// </summary>
        /// <returns>Validated values.</returns>
        private string[] Validate()
        {
            string[] values = m_values.Trim(new char[] { ' ', m_separator }).
                Split(m_separator).Distinct().ToArray();

            values = values.Where((item) => int.TryParse(item, out int result)).ToArray();
            return values;
        }

        /// <summary>
        /// Validates values with new value.
        /// </summary>
        /// <param name="number">New value.</param>
        /// <returns>Validated values.</returns>
        private string[] Validate(uint number)
        {
            string[] values = (m_values.Trim(new char[] { ' ', m_separator }) + m_separator + number.ToString()).
                Trim(new char[] { ' ', m_separator }).Split(m_separator).Distinct().ToArray();

            values = values.Where((item) => int.TryParse(item, out int result)).ToArray();
            return values;
        }


        /// <summary>
        /// Bakes possible values in integer mask.
        /// </summary>
        /// <returns>Integer mask.</returns>
        public ulong Bake()
        {
            string[] valuesS = Validate();
            m_values = string.Join(m_separator.ToString(), valuesS);

            m_mask = 0;

            int[] valuesI = Array.ConvertAll<string, int>(valuesS, int.Parse);

            for (int i = 0; i < valuesI.Length; i++)
            {
                m_mask |= (uint)Mathf.Pow(2, valuesI[i]);
            }

            return m_mask;
        }

        /// <summary>
        /// Adds new number as possible.
        /// </summary>
        /// <param name="number">New value.</param>
        public void Add(uint number)
        {
            string[] values = Validate(number);
            m_values = string.Join(m_separator.ToString(), values);
        }

        /// <summary>
        /// Validates values.
        /// </summary>
        public void ValidatesValues()
        {
            string[] values = Validate();
            m_values = string.Join(m_separator.ToString(), values);
        }

        /// <summary>
        /// Removes value from possible.
        /// </summary>
        /// <param name="number"></param>
        public void Remove(uint number)
        {
            string[] valuesS = Validate();
            int[] valuesI = Array.ConvertAll<string, int>(valuesS, int.Parse);

            valuesI = valuesI.Where((el) => el != number).ToArray();
            m_values = string.Join(m_separator.ToString(), valuesI);
        }

        /// <summary>
        /// Validate values on its minimum bound.
        /// </summary>
        /// <param name="min">Minimum bound of value.</param>
        public void ValidatesValuesOnMin(uint min)
        {
            string[] values = Validate();
            values = values.Where((item) => int.Parse(item) >= min).ToArray();

            m_values = string.Join(m_separator.ToString(), values);
        }

        /// <summary>
        /// Validate values on its maximum bound.
        /// </summary>
        /// <param name="max">Maximum bound of value.</param>
        public void ValidatesValuesOnMax(uint max)
        {
            string[] values = Validate();
            values = values.Where((item) => int.Parse(item) <= max).ToArray();

            m_values = string.Join(m_separator.ToString(), values);
        }

        /// <summary>
        /// Validate values on its minimum and maximum bound.
        /// </summary>
        /// <param name="min">Minimum bound of value.</param>
        /// <param name="max">Maximum bound of value.</param>
        public void ValidatesValues(uint min, uint max)
        {
            string[] values = Validate();
            values = values.Where((item) => int.Parse(item) >= min).ToArray();
            values = values.Where((item) => int.Parse(item) <= max).ToArray();

            m_values = string.Join(m_separator.ToString(), values);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<uint> GetEnumerator()
        {
            string[] valuesS = Validate();
            m_values = string.Join(m_separator.ToString(), valuesS);

            uint[] valuesI = Array.ConvertAll<string, uint>(valuesS, uint.Parse);

            foreach (var item in valuesI)
                yield return item;
        }


        /// <summary>
        /// Compares object's mask with specific value.
        /// </summary>
        /// <param name="valueObj">Object to be compared.</param>
        /// <param name="value">Value used to compare.</param>
        /// <returns></returns>
        public static bool operator &(IntMask valueObj, int value)
        {
            if (valueObj.m_mask == 0) return false;

            return ((valueObj.m_mask & (uint)Mathf.Pow(2, value)) != 0);
        }

        /// <summary>
        /// Compares object's mask with specific value.
        /// </summary>
        /// <param name="valueObj">Object to be compared.</param>
        /// <param name="value">Value used to compare.</param>
        /// <returns></returns>
        public static bool operator &(IntMask valueObj, uint value)
        {
            if (valueObj.m_mask == 0) return false;

            return ((valueObj.m_mask & (uint)Mathf.Pow(2, value)) != 0);
        }
    }

    /// <summary>
    /// Specific entity state.
    /// </summary>
    [System.Serializable]
    public class EntityState
    {
        /*[SerializeField]*/ private uint m_id;
        [SerializeField] [Range(0, ushort.MaxValue)] private int m_priority;
        /// <summary>
        /// Is state can be ended, when state in sleep mode.
        /// </summary>
        [SerializeField] private bool m_isBlockingOnSleep;
        /// <summary>
        /// Is state can be ended, when state in active mode.
        /// </summary>
        [SerializeField] private bool m_isBlocking;

        /// <summary>
        /// Mask of states, on which this state can be activated.
        /// </summary>
        [SerializeField] private IntMask m_checkOnActive;
        /// <summary>
        /// State methods.
        /// </summary>
        [SerializeField] private EntityStateSO m_state;

        private bool m_isActive;


        /// <summary>
        /// Is state can be ended, when state in sleep mode.
        /// </summary>
        public bool IsBlockingOnSleep
        {
            get { return this.m_isBlockingOnSleep; }
        }
        /// <summary>
        /// State methods.
        /// </summary>
        public EntityStateSO State
        {
            get { return this.m_state; }
        }
        /// <summary>
        /// Mask of states, on which this state can be activated.
        /// </summary>
        public IntMask CheckOn
        {
            get { return this.m_checkOnActive; }
        }
        /// <summary>
        /// Is state can be ended, when state in active mode.
        /// </summary>
        public bool IsBlocking
        {
            get { return this.m_isBlocking; }
        }
        public bool IsActive
        {
            get { return this.m_isActive; }
            set { this.m_isActive = value; }
        }
        public int Priority
        {
            get { return this.m_priority; }
        }
        public uint Id
        {
            get { return this.m_id; }
            set { this.m_id = value; }
        }
    }
}