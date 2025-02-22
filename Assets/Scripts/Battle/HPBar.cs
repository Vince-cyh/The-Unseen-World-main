using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        if (float.IsNaN(hpNormalized) || hpNormalized < 0 || hpNormalized > 1)
        {
            Debug.LogError("Invalid hpNormalized value: " + hpNormalized);
            return;
        }
        health.transform.localScale = new Vector3(hpNormalized, 1f, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        float curHp = health.transform.localScale.x;
        float changeAmt = curHp -newHp;

        while(curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f);
    }
}
