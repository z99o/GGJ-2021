using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayInstructions : MonoBehaviour
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
    public GameObject controlParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if(now - lastTextShow > textShowDelayMs)
        {

            if(textIndex >= textElements.Length)
            {
                if (!fadingOut)
                {

                    controlParent.SetActive(true);
                    foreach (Transform child in controlParent.transform)
                    {
                        StartCoroutine(FadeTextToFullAlpha(fadeTime, child.GetComponent<TextMeshProUGUI>()));
                    }

                    StartCoroutine(TriggerFadeOut(1.5f));
                    fadingOut = true;
                }
            }
            else
            {
                textElements[textIndex].SetActive(true);
                StartCoroutine(FadeTextToFullAlpha(fadeTime, textElements[textIndex].GetComponent<TextMeshProUGUI>()));
            }

            textIndex++;
            lastTextShow = now;
        }
    }

    public IEnumerator ShowControls(float t, GameObject controlParent)
    {
        //setting text to invis
        foreach (Transform child in controlParent.transform)
        {
            TextMeshProUGUI tmp = child.GetComponent<TextMeshProUGUI>();
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0);
        }
        controlParent.SetActive(true);

        float lastAlpha = -1;
        while(lastAlpha < 1)
        {
            

            foreach (Transform child in controlParent.transform)
            {
                TextMeshProUGUI tmp = child.GetComponent<TextMeshProUGUI>();
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, tmp.color.a + (Time.deltaTime / t));
                lastAlpha = tmp.color.a;
            }
           

            yield return null;
        }

        yield break;
    }

    public IEnumerator TriggerFadeOut(float t)
    {
        while (fadeOutWait > 0)
        {
            fadeOutWait -= (Time.deltaTime / t);
            yield return null;

        }


        float lastAlpha = 1;
        while (lastAlpha > 0)
        {
            foreach (Transform child in controlParent.transform)
            {
                TextMeshProUGUI tmp = child.GetComponent<TextMeshProUGUI>();
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, tmp.color.a - (Time.deltaTime / t));
               
            }

            foreach (GameObject g in textElements)
            {
                TextMeshProUGUI i = g.GetComponent<TextMeshProUGUI>();
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
                lastAlpha = i.color.a - (Time.deltaTime / t);

            }
            
            yield return null;
        }

        gameObject.GetComponent<DisplayInstructions>().enabled = false;
    }

    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

}
