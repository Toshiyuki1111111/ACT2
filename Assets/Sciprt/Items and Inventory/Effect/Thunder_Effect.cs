using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item effect/Thunder")]
public class Thunder_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderPrefab;
    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunder = Instantiate(thunderPrefab, _enemyPosition.position, Quaternion.identity);

        Destroy(newThunder, .5f);
    }
}
