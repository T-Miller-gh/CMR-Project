using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class SimpleFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _distance = 3;
    [SerializeField] private float _height = 3;
    [SerializeField] private float _angle = 30;

    [SerializeField] private float _editorDistance = 3;
    [SerializeField] private float _editorHeight = 3;
    [SerializeField] private float _editorAngle = 30;

    public bool EditorIsActive;

    [ContextMenu("Execute")]
    private void LateUpdate()
    {
        if (EditorIsActive)
        {
            transform.position = _target.position - _target.forward * _editorDistance + _target.up * _editorHeight;
            transform.LookAt(_target);
            transform.rotation = Quaternion.Euler(_editorAngle, transform.rotation.eulerAngles.y, 0);
        }
        else
        {
            transform.position = _target.position - _target.forward * _distance + _target.up * _height;
            transform.LookAt(_target);
            transform.rotation = Quaternion.Euler(_angle, transform.rotation.eulerAngles.y, 0);
        }
    }
}