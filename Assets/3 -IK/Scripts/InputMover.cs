using UnityEngine;

public class InputMover : MonoBehaviour
{
   [SerializeField] private float moveSpeed;
   [SerializeField] private float rotationSpeed;

   private void Update()
   {
      transform.Translate(transform.InverseTransformDirection(transform.forward) * (Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime));
      transform.Rotate(0f, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0f);
   }

   // private void OnDrawGizmos()
   // {
   //    Gizmos.color = Color.yellow;
   //    Gizmos.DrawCube(transform.position + transform.InverseTransformDirection(transform.forward) * 5f, 4f * Vector3.one);
   // }
}
