using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject template;
    public int numHearts = 10;
    GameObject[] hearts;
    void Awake()
    {
        hearts = new GameObject[numHearts];
        for (int i = 0; i < numHearts; i++)
        {
            hearts[i] = Instantiate(template, template.transform.parent);
            hearts[i].SetActive(true);
        }
    }
    
    public void SetHealth(float health, float maxHealth){
        var numHalfHearts = Mathf.CeilToInt(health/maxHealth*(float)numHearts*2f);
        for (int i = 0; i < numHearts; i++)
        {
            if (numHalfHearts >= 2 * (i+1)){
                hearts[i].transform.GetChild(1).gameObject.SetActive(true);
                hearts[i].transform.GetChild(2).gameObject.SetActive(true);
            } else if (numHalfHearts >= 2 * i+1){
                hearts[i].transform.GetChild(1).gameObject.SetActive(false);
                hearts[i].transform.GetChild(2).gameObject.SetActive(true);
            } else{
                hearts[i].transform.GetChild(1).gameObject.SetActive(false);
                hearts[i].transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }
}
