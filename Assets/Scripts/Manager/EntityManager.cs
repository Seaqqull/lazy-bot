using System.Collections.Generic;
using LazyBot.Target.Property;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace LazyBot.Manager
{
    public class EntityManager : MonoBehaviour
    {
        [SerializeField] private FloatReference m_updateRate;
        [SerializeField] private TargetPropertySO[] _properties;        

        private static EntityManager _entityManager;

        private Coroutine _entityChecker;
        private List<LazyBot.Entity.EntityController> _entities;

        public static EntityManager Instance
        {
            get
            {
                if (!_entityManager)
                {
                    GameObject gm = new GameObject("GameManager", typeof(EntityManager));
                    gm.transform.SetAsFirstSibling();
                    DontDestroyOnLoad(gm);

                    _entityManager = gm.GetComponent<EntityManager>();
                    _entityManager.SearchForEnities();
                }
                return _entityManager;
            }
        }


        private void Awake()
        {
            if (!_entityManager)
            {
                _entityManager = this;
                _entityManager.SearchForEnities();
            }
        }

        private void OnEnable()
        {
            SetIsActive(true);
        }

        private void OnDisable()
        {
            SetIsActive(false);
        }


        private void SearchForEnities()
        {
            _entities = FindObjectsOfType<LazyBot.Entity.EntityController>().
                OfType<LazyBot.Entity.EntityController>().ToList();
        }

        private int GetEntityPosition(uint id)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entities[i].Behaviour.Id == id) return i;
            }
            return -1;
        }

        private void SetIsActive(bool isActive)
        {
            if (isActive)
            {
                _entityChecker = StartCoroutine("CheckEntities", m_updateRate.Value);
            }
            else if ((!isActive) && (_entityChecker != null))
            {
                StopCoroutine(_entityChecker);
            }
            else
            {
                Debug.LogError("Can't stop coroutation that refers to null.");
            }
        }

        private int GetPropertyPosition(uint id)
        {
            for (int i = 0; i < _properties.Length; i++)
            {
                if (_properties[i].Id == id) return i;
            }
            return -1;
        }

        private IEnumerator CheckEntities(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);

                for (int i = _entities.Count - 1; i >= 0; i--)
                {
                    if (_entities[i].Behaviour.IsDeath) _entities.RemoveAt(i);
                }
            }
        }


        public bool IsPropertyExist(uint propertyId)
        {
            return (GetPropertyPosition(propertyId) == -1) ? false : true; 
        }

        public LazyBot.Entity.EntityController GetEntity(uint id)
        {
            int position = GetEntityPosition(id);
            return (position == -1) ? null : _entities[position];
        }

        public (uint, dynamic) GetProperty(uint entityid, uint propertyId)
        {
            int entityPosition = GetEntityPosition(entityid);
            if (entityPosition == -1) return (propertyId, null);

            int propertyPosition = GetPropertyPosition(propertyId);
            if (propertyPosition == -1) return (propertyId, null);

            return (propertyId,
                _properties[propertyPosition].GetProperty(_entities[entityPosition]));
        }

        public void ValidateArea(LazyBot.Area.Searching.SearchingArea area)
        {
            if (area.TargetType == null) return;

            area.TargetType.Mask.Bake();

            List<uint> wrongIndecies = new List<uint>();

            foreach (var propertyId in area.TargetType.Mask)
            {
                if (!IsPropertyExist(propertyId))
                {
                    wrongIndecies.Add(propertyId);
#if UNITY_EDITOR
                    Debug.LogError($"Id:{propertyId} of property you entered doesn't exist or you didn't add it to GameManager! It will be removed.", area.TargetType);
#endif
                }
            }

            foreach (var wrongId in wrongIndecies)
                area.TargetType.Mask.Remove(wrongId);
        }

        public List<(uint, dynamic)> GetProperties(uint entityid, LazyBot.Entity.Data.IntMask propertyMask)
        {
            int entityPosition = GetEntityPosition(entityid);
            if (entityPosition == -1) return null;

            var properties = new List<(uint, dynamic)>();

            //fill properties values
            foreach (var propertyId in propertyMask)
            {
                properties.Add(GetProperty(entityid, propertyId));
            }

            return properties;
        }

    }
}
