using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Threading.Tasks;
#endif
namespace qb.Pattern
{
    /// <summary>
    /// Abstract scriptable object singleton pattern
    /// </summary>
    /// <typeparam paramName="T"></typeparam>
    /// <remarks>
    /// WARNING: THIS PATTERN DOES NOT SUPPORT INSTANTIATION AT RUNTIME!
    /// </remarks>
    public abstract class SOSingleton<T> : SOWithGUID where T : ScriptableObject
    {
        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            CheckUniqueInstance();
#endif
        }

        #region editor
#if UNITY_EDITOR

        private void CheckUniqueInstance()
        {
            __CheckUniqueInstance();
        }
        /// <summary>
        /// Check if this instance is unique in the project, if not delete this object from the project
        /// </summary>
        /// <returns>True if this object is unique, false if not</returns>
        private async void __CheckUniqueInstance()
        {
            string sFind = $"t:{typeof(T)}";
            //Debug.Log(sFind);

            while (!AssetDatabase.Contains(this))
                await Task.Yield();

            var guids = AssetDatabase.FindAssets(sFind);

            if (guids != null && guids.Length > 1)
            {

                var currentPath = AssetDatabase.GetAssetPath(this);
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path != currentPath)
                    {
                        var obj = AssetDatabase.LoadAssetAtPath<T>(path);
                        if (obj != null && obj.GetType() == this.GetType())
                        {
                            //There are an other one in the project so this one must be destroyed!
                            EditorUtility.DisplayDialog("Singleton error", $"An other instance of singleton type {typeof(T)}.\nThis one must be destroyed!", "Destroy");
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
                            AssetDatabase.Refresh();
                            return;
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Get the project instance.
        /// </summary>
        /// <returns>The instance of the object in the editor project if found or null</returns>
        public static SOSingleton<T> GetProjectInstance()
        {
            var type = typeof(T);
            var guids = AssetDatabase.FindAssets($"t:{type}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var result = AssetDatabase.LoadAssetAtPath<SOSingleton<T>>(path);
                if (result != null)
                    return result;
            }
            return null;
        }
#endif
        #endregion
    }
}
