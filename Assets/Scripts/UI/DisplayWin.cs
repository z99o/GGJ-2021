using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayWin : MonoBehaviour
{
    public int textShowDelayMs = 2500;
    public float fadeTime = 2.5f;
    public float fadeOutWait = 5f;

    private long lastTextShow = -1;
    private bool fadingOut = false;
    
    /// <summary>
    /// Displayed in the order of the array. 
    /// </summary>
    public GameObject[] textElements;
    private int textIndex = 0;

    /// <summary>
    /// The controls to show after all the text has been displayed. 
    /// </summary>
    public TextMeshProUGUI tex;
    public GameObject player;
    public bool isRan = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerInteractions>().holdingWin && !isRan) {
            tex.gameObject.SetActive(true);
            StartCoroutine(FadeTextToFullAlpha(fadeTime, tex));
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i)
    {
        isRan = true;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

}
