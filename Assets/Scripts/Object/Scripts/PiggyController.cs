using UnityEngine;

namespace Object
{
    public class PiggyController : MonoBehaviour
    {
        [SerializeField] private float bounds = 5f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                print(transform.position.x);
                if (transform.position.x <= -bounds)
                    transform.position = new Vector3(bounds, transform.position.y, transform.position.z);
                else
                    transform.position += Vector3.left * 2;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                print(transform.position.x);
                if (transform.position.x >= bounds)
                    transform.position = new Vector3(-bounds, transform.position.y, transform.position.z);
                else
                    transform.position += Vector3.right * 2;
            }
        }
    }
}