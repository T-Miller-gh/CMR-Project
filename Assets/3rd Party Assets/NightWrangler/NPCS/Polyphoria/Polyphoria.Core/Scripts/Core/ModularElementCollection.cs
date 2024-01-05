using UnityEngine;

namespace Polyphoria
{
    [CreateAssetMenu(fileName = "collection.asset", menuName = "Polyphoria/Element Collection")]
    public class ModularElementCollection : ScriptableObject
    {
        public GameObject[] Contents;
    }
}