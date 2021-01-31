using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpdateStamina : MonoBehaviour
{
    private Character_Movement_3D moveSystem;
    private float last_stamina = -1;
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
        if(last_stamina != moveSystem.m_sprint_cur_level)
        {
            last_stamina = moveSystem.m_sprint_cur_level;

            float barLevel = (moveSystem.m_sprint_cur_level / moveSystem.m_sprint_max_level);
            if (barLevel > 0.99) bar.fillAmount = 1;
            else bar.fillAmount = barLevel;

        }
    }
}
