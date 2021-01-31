using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Kick_Sniff : MonoBehaviour {

    [Header("Sniffing Settings")]
    public float sniffTimer;
    public float maxIndicatorAlpha; //Max value of alpha
    public float distMult; //How far away you're able to get max alpha
    public AnimationCurve m_tintCurve;
    public float m_curveDuration;

    public GameObject Canvas;

    private GameObject cereal;

    // Start is called before the first frame update
    void Start() {
        cereal = GameObject.Find("Cereal");
    }

    // Update is called once per frame
    void Update() {
        
        if (Input.GetButtonDown("Kick")) {
            Kick();
        }

        if (Input.GetButtonDown("Sniff")) {
            Sniff();
        }
    }

    void Sniff() {
        float dist = Vector3.Distance(transform.position, cereal.transform.position);
        float alpha = (maxIndicatorAlpha/dist) * distMult;
        alpha = Mathf.Clamp(alpha, 0, maxIndicatorAlpha);
        Transform tintObject = Canvas.transform.Find("Tint");

        StartCoroutine(EvalCurve(alpha, tintObject));
    }

    void Kick() {

    }

    IEnumerator EvalCurve(float alpha, Transform tintObject) {
        float time = 0;
        while (time <= m_curveDuration) {
            float value = m_tintCurve.Evaluate(time/m_curveDuration);
            time += Time.deltaTime;

            Color tint = new Color(0, 1, 0, value * (alpha / 255));
            tintObject.GetComponent<Image>().color = tint;
            yield return null;
        }
    }
}