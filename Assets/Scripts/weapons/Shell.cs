using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {

    public Rigidbody myRigidBody;
    public float forceMin;
    public float forceMax;
    float lifetime = 2;
    float fadetime = 2;

	void Start () {

        float force = Random.Range(forceMin, forceMax);
        myRigidBody.AddForce(transform.right * force);
        myRigidBody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());

	}
	
	IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
