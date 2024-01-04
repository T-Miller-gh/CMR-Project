using System.Collections;
using UnityEngine;

namespace Polyphoria
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
        private Animator Animator;

		IEnumerator Start()
        {
            yield return null;
            yield return null;
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            Animator = GetComponent<ModularCharacter>().SkeletonAsset.GetComponent<Animator>();
        }

        public void Update()
        {
            if (Animator != null)
            {
                Animator.SetFloat("Forward", Input.GetAxis("Vertical"), 0.1f, Time.deltaTime);
                Animator.SetFloat("Turn", Input.GetAxis("Horizontal"), 0.1f, Time.deltaTime);
            }
        }
	}
}
