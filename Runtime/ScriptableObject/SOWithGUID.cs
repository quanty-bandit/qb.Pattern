using TriInspector;
using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Threading.Tasks;
#endif

namespace qb.Pattern
{
    /// <summary>
    /// Scriptable object abstract class with guid serialization 
    /// to avoid reference serialization issue from addressable object.
    /// The issue append when a scriptable object is referenced 
    /// in an addressable scene or asset, at addressable loading 
    /// a new instance of scriptable object is created in place of the original
    /// scriptable object referenced.
    /// The following class implement a pattern to retreive the original referenced scriptable object.
    /// </summary>
    [DefaultExecutionOrder(-2)]
    [DeclareFoldoutGroup("GUID")]
    public abstract class SOWithGUID : ScriptableObject
    {
        /// <summary>
        /// The asset guid value 
        /// </summary>
        [SerializeField, ReadOnly,GUIColor("EDD3ED"),PropertyOrder(-9999)]
        [Group("GUID")]
        protected string _guid;
        public string GUID=>_guid;

#if UNITY_EDITOR
        [NonSerialized]
        bool updateGuidPending;
        [Button("Refresh"), GUIColor("EDD3ED"), PropertyOrder(-9998), PropertySpace(SpaceBefore = 0)]
        /// <summary>
        /// Update the guid with the unity asset guid
        /// </summary>
        [Group("GUID")]
        public async virtual void UpdateGUID()
        {
            if (updateGuidPending) return;
            await Task.Yield();
            try
            {
                updateGuidPending = true;   
                //Waiting from the object AssetDatabase insertion 
                while (!AssetDatabase.Contains(this))
                    await Task.Yield();

                var path = AssetDatabase.GetAssetPath(this);
                var id = AssetDatabase.AssetPathToGUID(path);
                if (id != _guid)
                {
                    _guid = AssetDatabase.AssetPathToGUID(path);

                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssetIfDirty(this);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                updateGuidPending = false;
            }
        }
#endif
        [NonSerialized]
        private static Dictionary<string, SOWithGUID> sources = new Dictionary<string, SOWithGUID>();
        [NonSerialized]
        private static Dictionary<string, int> referencesCount = new Dictionary<string, int>();

        /// <summary>
        /// Get the scriptable object source from a reference
        /// </summary>
        /// <param name="reference">A scriptable object refence with a valid GUID</param>
        /// <returns>The scriptable object source or null if no source found!</returns>
        public static SOWithGUID GetSourceFromGUID(SOWithGUID reference)
        {
            Debug.Assert(reference != null && !string.IsNullOrEmpty(reference._guid));

            if (reference == null) return null;
            sources.TryGetValue(reference._guid, out var source);
            return source;
        }
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            UpdateGUID();
#endif
        }
        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if(EditorApplication.isPlayingOrWillChangePlaymode)
#endif
            {
                if (string.IsNullOrEmpty(_guid)) return;
                if (!sources.ContainsKey(_guid))
                {
                    sources.Add(_guid, this);
                    referencesCount.Add(_guid, 1);
                }
                else
                {
                    referencesCount[_guid]++; 
                }
            }
        }
        protected virtual void OnDisable()
        {
            if (sources.ContainsKey(_guid))
            {
                referencesCount[_guid]--;
                if(referencesCount[_guid] == 0)
                {
                    sources.Remove(_guid);
                    referencesCount.Remove(_guid);
                }
            }
        }
    }
}
