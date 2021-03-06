using System.Collections.Generic;
using UnityEngine;

public class EnemyLandMovement : EnemyMovement
{
    Vector3 target;
    List<Vector3> towersPoints = new List<Vector3>();
    EnemyAnimationController animControl;
    EnemyParticleController particleControl;
    void Start()
    {
        animControl = GetComponent<EnemyAnimationController>();
        particleControl = GetComponent<EnemyParticleController>();
        // InvokeRepeating("UpdateState", 0.0f, 1 / speed);
        target = GameObject.FindGameObjectWithTag("EnemyTarget").transform.position - (GameObject.FindGameObjectWithTag("EnemyTarget").transform.position - transform.position).normalized * distance;
        UpdateState();
    }

    void UpdateState() //every time then playr bulid new tower
    {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Tower");
        towersPoints.RemoveRange(0, towersPoints.Count);
        //Collider[] towers = Physics.OverlapSphere(transform.position, 3, towerMask);
        foreach (GameObject k in tmp)
        {
            towersPoints.Add(k.transform.position);
        }
    }
    void Update()
    {
        if (TowerHP.instance != null)
        {
            if (Vector3.Distance(transform.position, TowerHP.instance.transform.position) > distance + 0.2f)
            {
                Vector3 missTowers = Vector3.zero;
                for (int i = 0; i < towersPoints.Count; i++)
                {
                    if (transform.position.magnitude + 1.5f < towersPoints[i].magnitude) continue;

                    float tmp = 1.0f / Mathf.Max(Vector3.Magnitude(transform.position - towersPoints[i]) - 1.5f, 0.001f);

                    Vector3 additive = Vector3.Cross((transform.position - towersPoints[i]).normalized, Vector3.up) * tmp;

                    if (Vector3.Distance(transform.position + additive, TowerHP.instance.transform.position) < Vector3.Distance(transform.position - additive, TowerHP.instance.transform.position))
                    { missTowers += additive; }
                    else { missTowers -= additive; }
                }
                Vector3 direction = (target - transform.position).normalized + missTowers;
                direction.y = 0;
                transform.Translate(ShortNormal(direction) * speed * Time.deltaTime, Space.World);

                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else
            {
                animControl.SwitchToState("Attack");
                particleControl.EnableParticle();
                Vector3 direction = TowerHP.instance.transform.position - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }

    Vector3 ShortNormal(Vector3 vec)
    {
        if (vec.magnitude < 1.0f)
            return vec;
        else return vec.normalized;
    }
}
