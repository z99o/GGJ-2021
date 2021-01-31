using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSniffo : MonoBehaviour
{
    private Character_Kick_Sniff kickSniffSystem;
    private Image bar;

    // Start is called before the first frame update
    void Start()
    {
        kickSniffSystem = GameObject.Find("Player").GetComponent<Character_Kick_Sniff>();
        bar = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (now - kickSniffSystem.lastSniff < kickSniffSystem.sniffCooldownMs)
        {
            bar.fillAmount = ((now - kickSniffSystem.lastSniff) / kickSniffSystem.sniffCooldownMs);
        }
        else
        {
            bar.fillAmount = 1;
        }
        
    }
}
