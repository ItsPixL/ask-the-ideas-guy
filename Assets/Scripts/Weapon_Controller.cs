using System.Collections;
using UnityEngine;

public class Weapon_Controller : MonoBehaviour {
    public GameObject Sword;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (CanAttack) {
                SwordAttack();
            }
        }
    }

    public void SwordAttack()
    {
        CanAttack = false;
        Animator anim = Sword.GetComponent<Animator>();
        anim.SetTrigger("Attack");
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
    }
}