using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackGround : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float xPosition;
    private float yPosition;
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        xPosition = transform.position.x;
        
    }

    // Update is called once per frame
    void Update()
    {
        float xDistanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + xDistanceToMove, cam.transform.position.y - 2.52f);
    }
}
