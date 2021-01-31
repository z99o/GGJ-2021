using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHealth : MonoBehaviour
{
    private Character_Movement_3D moveSystem;
    private float last_health = -1;
    private Image bar;

    // Start is called before the first frame update
    void Start()
    {
        moveSystem = GameObject.Find("Player").GetComponent<Character_Movement_3D>();
        bar = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (last_health != moveSystem.m_health)
        {
            last_health = moveSystem.m_health;

            float barLevel = (moveSystem.m_health / moveSystem.m_max_health);
            if (barLevel > 0.995) bar.fillAmount = 1;
            else bar.fillAmount = barLevel;

        }
    }
}
