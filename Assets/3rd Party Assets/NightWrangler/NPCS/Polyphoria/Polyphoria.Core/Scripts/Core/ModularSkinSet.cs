using UnityEngine;

namespace Polyphoria
{
    [CreateAssetMenu(fileName = "collection.asset", menuName = "Polyphoria/Skin Set")]
    public class ModularSkinSet : ScriptableObject
    {
        public GameObject Head;
        public GameObject Torso;
        public GameObject Hands;
        public GameObject Legs;
        public GameObject Feet;
        public GameObject Arm;
    }
}