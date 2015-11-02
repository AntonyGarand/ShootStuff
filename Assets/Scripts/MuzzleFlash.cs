using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour {

    public GameObject flashHolder;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public float flashTime;

    void Start()
    {
        flashHolder.SetActive(false);
    }

    public void Activate() {
        flashHolder.SetActive(true);
        Invoke("Deactivate", flashTime);

        int flashSpriteIndex = Random.Range(0, flashSprites.Length);
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }
    }

    public void Deactivate() {
        flashHolder.SetActive(false);
    }

}
