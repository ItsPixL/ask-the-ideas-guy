using System.Collections.Generic;
using UnityEngine;
using System;

namespace CooldownManager {
    public class Cooldown_Manager : MonoBehaviour
    {
        // Couples the ability identifier (name) to the next time it will be ready (current time + cooldown duration).
        private Dictionary<string, float> nextReadyTime = new Dictionary<string, float>();
        // effectively a set that stores the names of abilities that are currently on cooldown.
        private HashSet<string> abilitiesOnCooldown = new HashSet<string>();
        public event Action<string> OnCooldownFinished;

        public bool CanUseAbility(string abilityName)
        { // Checks whether the ability is off cooldown and ready to use.
            if (!nextReadyTime.ContainsKey(abilityName))
            { // checking if the ability has a cooldown
                Debug.Log("Does not have cooldown for " + abilityName);
                return true;
            }
            return Time.time >= nextReadyTime[abilityName]; // if the ability DOES have a cooldown, return true if the current time is greater than or equal to the next ready time
        }

        // Starts the cooldown for the ability by adding the cooldown duration to the current time and adding the ability name to the event
        public void UseCooldown(string abilityName, float cooldownDuration)
        {
            nextReadyTime[abilityName] = Time.time + cooldownDuration;
            abilitiesOnCooldown.Add(abilityName);
        }

        // Returns how many seconds remain before the ability is ready. Returns 0 if ability is ready.
        public float GetRemainingCooldown(string abilityName)
        {
            if (!nextReadyTime.ContainsKey(abilityName))
            {
                return 0f;
            }
            return Mathf.Max(0f, nextReadyTime[abilityName] - Time.time);
        }

        private void Update()
        {
            // Check all abilities currently on cooldown
            var finished = new List<string>();
            foreach (var ability in abilitiesOnCooldown)
            {
                if (GetRemainingCooldown(ability) <= 0f)
                {
                    finished.Add(ability);
                }
            }
            // Fire event and remove from cooldown set
            foreach (var ability in finished)
            {
                abilitiesOnCooldown.Remove(ability);
                OnCooldownFinished?.Invoke(ability);
            }
        }
    }
}