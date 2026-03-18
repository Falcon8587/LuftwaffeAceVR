using UnityEngine;

public class missile : MonoBehaviour
{
    public class UnguidedMissile : MonoBehaviour
    {
        [SerializeField] float speed = 200f;
        [SerializeField] float maxDistance = 500f;

        Vector3 launchDirection;
        Vector3 startPosition;

        bool launched = false;

        public void Launch(float launchSpeed)
        {
            if (launched) return;

            launched = true;

            transform.parent = null; // detach from plane

            speed = launchSpeed;
            launchDirection = transform.forward;
            startPosition = transform.position;
        }

        private void Update()
        {
            if (!launched) return;

            transform.position += launchDirection * speed * Time.deltaTime;

            float distance = Vector3.Distance(startPosition, transform.position);

            if (distance >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}  


