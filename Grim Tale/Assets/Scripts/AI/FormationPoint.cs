using System.Linq;
using UnityEngine;

namespace AI
{
    public class FormationPoint : MonoBehaviour
    {
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private int formationCount = 4;
        [SerializeField] private float formationRadius = 2f;
        [SerializeField] private Enemy upgradePrefab;

        private bool ready;
        
        private void Update()
        {
            var sameEnemies = FindObjectsOfType<Enemy>().Where(e => e.Type == enemyType).ToArray(); // TODO Make this more efficient if necessary
            ready = sameEnemies.Length >= formationCount;
            if (!ready) return;
            
            var positionsSum = sameEnemies.Aggregate(Vector3.zero, (sum, e) => sum + e.transform.position);
            var formationCenter = positionsSum / sameEnemies.Length;
            transform.position = formationCenter;

            var closeEnemies = sameEnemies.Where(e => Vector3.Distance(e.transform.position, formationCenter) < formationRadius).ToArray();
            if (closeEnemies.Length < formationCount) return;
            
            for (var i = 0; i < formationCount; i++)
            {
                if(!closeEnemies[i].CanUpgrade) return;
            }
                
            for (var i = 0; i < formationCount; i++)
            {
                Destroy(closeEnemies[i].gameObject);
            }

            var enemy = Instantiate(upgradePrefab, transform.position, Quaternion.identity, FormationsManager.Instance.EnemiesParent);
            if (enemy.Type.Equals(EnemyType.MutantCharger))
            {
                enemy.StateBlocked = true;
            }
        }

        // Properties
        
        public bool Ready => ready;
    }
}
