using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

    public float rotateSpeed;
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color crosshairColor;
    public Color enemyHoveringColor;
    public Color regularDotColor;
    void Start()
    {
        Cursor.visible = false;
        GetComponent<SpriteRenderer>().color = crosshairColor;
    }
    void Update () {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
	}

    public void DetectTargets(Ray ray)
    {
        dot.color = Physics.Raycast(ray, 100, targetMask) ? enemyHoveringColor : regularDotColor;
    }
}
