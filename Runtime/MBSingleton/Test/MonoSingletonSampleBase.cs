using UnityEngine;
using qb.Pattern;
namespace qb.Test
{
    public class MonoSingletonSampleBase:MBSingleton<MonoSingletonSampleBase>
    {
        public override EDuplicatedSingletonInstanceAction DuplicatedInstanceAction => EDuplicatedSingletonInstanceAction.DestroyInstance;
        private void Start()
        {
            Debug.Log($"Singleton base {name} started!");
        }
    }
}
