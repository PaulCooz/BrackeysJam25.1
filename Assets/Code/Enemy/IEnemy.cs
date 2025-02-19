using UnityEngine;

namespace JamSpace
{
    public interface IEnemy
    {
        public void Attack(Vector3 worldPositionToAttack);
    }
}