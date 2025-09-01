using UnityEngine;

namespace Common.scripts
{
    public class StaircasePlatform : MonoBehaviour
    {
        [SerializeField] private bool isUpperTrigger;

        private void Update()
        {
            var downKeyDown =
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.DownArrow);

            if (downKeyDown)
                transform.parent.GetComponent<Collider2D>().enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                transform.parent.GetComponent<Collider2D>().enabled = isUpperTrigger;
        }
        
         
    }
}