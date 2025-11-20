using qb.Pattern;
using UnityEngine;
namespace qb.Test
{
    public class MonoSingletonSampleExtention:MonoSingletonSampleBase
    {
        public override EDuplicatedSingletonInstanceAction DuplicatedInstanceAction => EDuplicatedSingletonInstanceAction.DestroyGameObject;
        private void Start()
        {
            Debug.Log($"Singleton extention {name} started!");
        }
    }
}