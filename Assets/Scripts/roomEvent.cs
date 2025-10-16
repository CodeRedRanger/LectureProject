using UnityEngine;

public class roomEvent : MonoBehaviour
{
    [SerializeField] GameObject door1, door2;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            doorState(true); 
        }
    }

    void doorState(bool state)
    {
        door1.SetActive(state);
        door2.SetActive(state);
    }
}
