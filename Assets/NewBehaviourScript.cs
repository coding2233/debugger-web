using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Button>().onClick.AddListener(() => {

        //    Debug.Log("asdasdasdfasfsdf");
        //});
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"test log {Random.Range(1000000, 9000000)}");
        }
    }
}
