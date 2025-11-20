using UnityEngine;
namespace qb.Test
{
    public class SingletonTest : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            MonoSingletonSampleExtention instance = MonoSingletonSampleExtention.GetInstance<MonoSingletonSampleExtention>();
            Debug.Log($"MonoSingletonSampleExtention instance is on {instance.name} object");
        }

       
    }
}
