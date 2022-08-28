using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TM.Entity.Player
{
    class PlayerAttackAction
    {
        const int ATTACK_FRAME = 25;

        private int _currentTime = ATTACK_FRAME;

        public bool FixedUpdate()
        {
            //突き刺し中（もとに戻る待機中）なら
            if (--_currentTime > 0)
            {
                return false;
            }

            return true;
        }

        public void ResetTime()
        {
            _currentTime = ATTACK_FRAME;
        }
    }
}