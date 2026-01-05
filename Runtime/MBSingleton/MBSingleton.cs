using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace qb.Pattern
{
    /// <summary>
    /// Action enumerator done in case of duplicated singleton instance found in scene
    /// </summary>
    public enum EDuplicatedSingletonInstanceAction
    {

        Exception,          // Throw an exception  
        DestroyInstance,    // Destroy the monobehaviour instance
        DestroyGameObject   // Destroy the entire gameobject instance owner
                            // and all attached behaviours
    }

    /// <summary>
    /// Monobehaviour singleton pattern
    /// </summary>
    /// <typeparam paramName="T"></typeparam>
    public abstract class MBSingleton<T> : MonoBehaviour where T : MBSingleton<T>
    {
        private static ConcurrentDictionary<Type, MonoBehaviour> instances = new ConcurrentDictionary<Type, MonoBehaviour>();

        /// <summary>
        /// Virtual getter can be override by the concrete class.
        /// If is setted to true, the transform will be place at the root of the scene and 
        /// the DontDestroyOnLoad(gameobject) will be called by the OnAwake method
        /// to ensure that the object will be persistant accross all scenes
        /// </summary>
        public virtual bool IsPersistent => false;

        /// <summary>
        /// Virtual getter can be override by the concrete class.
        /// This property define the action will be done when a monobehaviour singleton 
        /// Awake method called on duplicated instance:
        ///     Exception           -> Throw an exception
        ///     DestroyInstance     -> Destroy the monobehaviour singleton instance but keep the gameobject owner
        ///     DestroyGameObject   -> Destroy the entire gameobject instance owner
        /// </summary>
        public virtual EDuplicatedSingletonInstanceAction DuplicatedInstanceAction =>  EDuplicatedSingletonInstanceAction.Exception;

        /// <summary>
        /// Get the singleton instance if exist or create one if createInstance 
        /// parameter is set to true.
        /// </summary>
        /// <typeparam paramName="TT">The monobehavior singleton type</typeparam>
        /// <param paramName="createInstanceIfNotFound">
        /// Create instance parameter can be set to true to instantiate a singleton
        /// in case or no instance found!
        /// </param>
        /// <returns>The singleton instance or null</returns>
        public static TT GetInstance<TT>(bool createInstanceIfNotFound = false) where TT : MBSingleton<T>
        {
            var type = typeof(TT);
            var instance = GetInstance(type) as TT;
            if (instance == null && createInstanceIfNotFound)
            {
                GameObject singleton = new GameObject();
                singleton.name = $"<SINGLETON> {typeof(TT)}";

                instance = singleton?.AddComponent<TT>();
                if(instance)
                    instances.TryAdd(type, instance);
            }
            return instance;
        }
        static T GetInstance(Type type)
        {
            MonoBehaviour monoInstance = null;
            if(instances.TryGetValue(type,out monoInstance))
            {
                if (monoInstance == null || monoInstance.gameObject == null)
                {
                    //Remove the invalid instance reference
                    instances.TryRemove(type,out monoInstance);
                    monoInstance = null;
                }
            }
            if(monoInstance != null)
            {
                return monoInstance as T;
            }
            else
            {
                T target = null;
                var targets = FindObjectsByType<T>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
                if(targets != null)
                {
                    for(int i = targets.Length - 1;i>=0;i--) // This reverse loop preserve the top bottom transform order in scene
                    //foreach(var entry in targets)
                    {
                        var entry = targets[i];
                        if (entry.GetType() == type && entry.enabled == true)
                        {
                            target = entry;
                            break;
                        }
                    }
                }
                if (target != null)
                {
                    var instance = (T)target;
                    instances.TryAdd(type, instance);
                    return instance;
                }
            }
            return null;
        }
        
        protected virtual void Awake()
        {
            var type = GetType();
            var instance = GetInstance(type);
            if (instance)
            {
                if (instance != this)
                {
                    switch (DuplicatedInstanceAction)
                    {
                        case EDuplicatedSingletonInstanceAction.DestroyInstance:
#if !NO_DEBUG_LOG_WARNING
                            Debug.LogWarning($"Dupplicated singleton instance of type {type} from object {name} was destroyed!");
#endif
                            DestroyImmediate(this);
                            break;
                        case EDuplicatedSingletonInstanceAction.DestroyGameObject:
#if !NO_DEBUG_LOG_WARNING
                            Debug.LogWarning($"Owner object ({name}) of singleton instance of type {type} was destroyed!");
#endif
                            DestroyImmediate(this.gameObject);
                            break;
                        case EDuplicatedSingletonInstanceAction.Exception:
                            throw new System.Exception($"Type {GetType()} of object {name} is allready instantiated by {instance.name} ");

                    }
                }
                else if (IsPersistent)
                {
                    var root = transform;
                    while (transform.parent != null)
                    {
                        root = transform.parent;
                    }
                    if(root.gameObject.scene.name != "DontDestroyOnLoad")
                    {
                        transform.parent = null;
                        DontDestroyOnLoad(gameObject);
                    }
                }
            }
        }
        protected virtual void OnDestroy()
        {
            var type = GetType();
            if (instances.TryGetValue(type, out var value))
            {
                if (value == this)
                {
                    instances.TryRemove(type,out var monoBehaviour);
                }
            }
        }        
    }
}
