using UnityEngine;

namespace Polyphoria
{
    public enum PartSlot
    {
        Hat = 1 << 0,
        Hair = 1 << 1,
        Beard = 1 << 2,
        Head = 1 << 3,
        FaceAccessoire = 1 << 4,
        Chest = 1 << 5,
        Leg = 1 << 6,
        Hand = 1 << 7,
        Arm = 1 << 8,
        Feet = 1 << 9, 
        PrimaryHand = 1 << 10, 
        SecondaryHand = 1 << 11,
    }


    /// <summary>
    /// Identifies a GameObject as a modular character part. Contains additional information
    /// that is used to adjust special BlendShapes when this part is added or removed
    /// </summary>
    public class ModularCharacterElement : MonoBehaviour
    {
        public PartSlot Slot;
        [EnumFlags] public PartSlot BlockedSlots;
        public string[] BlendShapeTriggers;
        public string[] BlendShapeReceivers;
        public Texture2D[] SkinMasks;
    }
}