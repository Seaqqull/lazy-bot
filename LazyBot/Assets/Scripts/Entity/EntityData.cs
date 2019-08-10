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
        [SerializeField] protected bool _isLock = false;
        /// <summary>
        /// Is value unlimited.
        /// </summary>
        [SerializeField] protected bool _isUnlimited = false;
        /// <summary>
        /// Is value can be regenerated.
        /// </summary>
        [SerializeField] protected bool _isRegenerate = true;

        [SerializeField] protected float _value;

        /// <summary>
        /// Startup value.
        /// </summary>
        [SerializeField] protected float _valueStart;
        /// <summary>
        /// Regenerate rate.
        /// </summary>
        [SerializeField] protected float _valueRegenerateRate;
        /// <summary>
        /// Low acceptable value.
        /// </summary>
        [SerializeField] protected float _valueLow;

        /// <summary>
        /// Maximum acceptable value.
        /// </summary>
        [SerializeField] protected float _valueMax;
        /// <summary>
        /// Minimum acceptable value.
        /// </summary>
        [SerializeField] protected float _valueMin;
        
        public float RegenerateRate
        {
            get { return this._valueRegenerateRate; }
        }
        public bool IsRegenerate
        {
            get { return this._isRegenerate; }
            set { this._isRegenerate = value; }
        }
        public bool IsUnlimited
        {
            get { return this._isUnlimited; }
            set { this._isUnlimited = value; }
        }
        public bool IsValueLow
        {
            get { return (_value <= _valueLow); }
        }
        public bool IsLock
        {
            get { return this._isLock; }
            set { this._isLock = value; }
        }
        public bool IsLow
        {
            get { return (this._value <= this._valueMin); }
        }
        public bool IsMax
        {
            get { return (this._value >= this._valueMax); }
        }
        /// <summary>
        /// Startup value.
        /// </summary>
        public float Start
        {
            get { return this._valueStart; }
        }
        public float Value
        {
            get { return this._value; }
        }
        /// <summary>
        /// Maximum acceptable value.
        /// </summary>
        public float Max
        {
            get { return this._valueMax; }
        }
        /// <summary>
        /// Minimum acceptable value.
        /// </summary>
        public float Min
        {
            get { return this._valueMin; }
        }
        /// <summary>
        /// Low acceptable value.
        /// </summary>
        public float Low
        {
            get { return this._valueLow; }
        }


        public FloatEntityData()
        {
            _value = _valueStart;
        }


        protected bool IsOnLimit()
        {
            return ((_value <= _valueMin) || (_value >= _valueMax));
        }

        /// <summary>
        /// Is value can be changed to the passed value.
        /// </summary>
        /// <param name="value">Change value.</param>
        public bool IsChangable(float value)
        {
            if (_isUnlimited) return true;
            else if (IsOnLimit()) return false;

            float newValue = _value + value;
            return (newValue <= _valueMax) && (newValue >= _valueMin);
        }

        /// <summary>
        /// Updates value based on regenerate rate and deltaTime.
        /// </summary>
        /// <param name="deltaTime">Time elapsed from previous frame.</param>
        /// <returns>Is regenerate was successful.</returns>
        public virtual bool Regenerate(float deltaTime = 1.0f)
        {
            if (!_isRegenerate) return false;

            Change(_valueRegenerateRate * deltaTime);

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
            if (_isUnlimited) return true;
            else if ((_isLock) ||
                ((!asPossible) && (!IsChangable(value)))) return false;

            _value += value;
            if (_value > _valueMax) _value = _valueMax;
            else if (_value < _valueMin) _value = _valueMin;

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
        [SerializeField] protected Slider _slider;


        /// <summary>
        /// Updates slider value based on float value.
        /// </summary>
        private void UpdateSlider()
        {
            if (!_slider) return;

            _slider.value = 
                LazyBot.Utility.Data.FloatHelper.Map(_value, _valueMin, _valueMax, _slider.minValue, _slider.maxValue);
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
        [SerializeField] private string _values;

        /// <summary>
        /// Separates possible value.
        /// </summary>
        private char _separator = '/';
        /// <summary>
        /// Values as mask of bit set.
        /// </summary>
        private ulong _mask;

        /// <summary>
        /// All possible values.
        /// </summary>
        public string Values
        {
            get { return this._values; }
        }
        /// <summary>
        /// Values as mask of bit set.
        /// </summary>
        public ulong Mask
        {
            get { return this._mask; }
        }


        /// <summary>
        /// Validates values.
        /// </summary>
        /// <returns>Validated values.</returns>
        private string[] Validate()
        {
            string[] values = _values.Trim(new char[] { ' ', _separator }).
                Split(_separator).Distinct().ToArray();

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
            string[] values = (_values.Trim(new char[] { ' ', _separator }) + _separator + number.ToString()).
                Trim(new char[] { ' ', _separator }).Split(_separator).Distinct().ToArray();

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
            _values = string.Join(_separator.ToString(), valuesS);

            _mask = 0;

            int[] valuesI = Array.ConvertAll<string, int>(valuesS, int.Parse);

            for (int i = 0; i < valuesI.Length; i++)
            {
                _mask |= (uint)Mathf.Pow(2, valuesI[i]);
            }

            return _mask;
        }

        /// <summary>
        /// Adds new number as possible.
        /// </summary>
        /// <param name="number">New value.</param>
        public void Add(uint number)
        {
            string[] values = Validate(number);
            _values = string.Join(_separator.ToString(), values);
        }

        /// <summary>
        /// Validates values.
        /// </summary>
        public void ValidatesValues()
        {
            string[] values = Validate();
            _values = string.Join(_separator.ToString(), values);
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
            _values = string.Join(_separator.ToString(), valuesI);
        }

        /// <summary>
        /// Validate values on its minimum bound.
        /// </summary>
        /// <param name="min">Minimum bound of value.</param>
        public void ValidatesValuesOnMin(uint min)
        {
            string[] values = Validate();
            values = values.Where((item) => int.Parse(item) >= min).ToArray();

            _values = string.Join(_separator.ToString(), values);
        }

        /// <summary>
        /// Validate values on its maximum bound.
        /// </summary>
        /// <param name="max">Maximum bound of value.</param>
        public void ValidatesValuesOnMax(uint max)
        {
            string[] values = Validate();
            values = values.Where((item) => int.Parse(item) <= max).ToArray();

            _values = string.Join(_separator.ToString(), values);
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

            _values = string.Join(_separator.ToString(), values);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<uint> GetEnumerator()
        {
            string[] valuesS = Validate();
            _values = string.Join(_separator.ToString(), valuesS);

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
            if (valueObj._mask == 0) return false;

            return ((valueObj._mask & (uint)Mathf.Pow(2, value)) != 0);
        }

        /// <summary>
        /// Compares object's mask with specific value.
        /// </summary>
        /// <param name="valueObj">Object to be compared.</param>
        /// <param name="value">Value used to compare.</param>
        /// <returns></returns>
        public static bool operator &(IntMask valueObj, uint value)
        {
            if (valueObj._mask == 0) return false;

            return ((valueObj._mask & (uint)Mathf.Pow(2, value)) != 0);
        }
    }

    /// <summary>
    /// Specific entity state.
    /// </summary>
    [System.Serializable]
    public class EntityState
    {
        /*[SerializeField]*/ private uint _id;
        [SerializeField] [Range(0, ushort.MaxValue)] private int _priority;
        /// <summary>
        /// Is state can be ended, when state in sleep mode.
        /// </summary>
        [SerializeField] private bool _isBlockingOnSleep;
        /// <summary>
        /// Is state can be ended, when state in active mode.
        /// </summary>
        [SerializeField] private bool _isBlocking;

        /// <summary>
        /// Mask of states, on which this state can be activated.
        /// </summary>
        [SerializeField] private IntMask _checkOnActive;
        /// <summary>
        /// State methods.
        /// </summary>
        [SerializeField] private EntityStateSO _state;

        private bool _isActive;


        /// <summary>
        /// Is state can be ended, when state in sleep mode.
        /// </summary>
        public bool IsBlockingOnSleep
        {
            get { return this._isBlockingOnSleep; }
        }
        /// <summary>
        /// State methods.
        /// </summary>
        public EntityStateSO State
        {
            get { return this._state; }
        }
        /// <summary>
        /// Mask of states, on which this state can be activated.
        /// </summary>
        public IntMask CheckOn
        {
            get { return this._checkOnActive; }
        }
        /// <summary>
        /// Is state can be ended, when state in active mode.
        /// </summary>
        public bool IsBlocking
        {
            get { return this._isBlocking; }
        }
        public bool IsActive
        {
            get { return this._isActive; }
            set { this._isActive = value; }
        }
        public int Priority
        {
            get { return this._priority; }
        }
        public uint Id
        {
            get { return this._id; }
            set { this._id = value; }
        }
    }
}