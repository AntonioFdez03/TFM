using UnityEngine;

[CreateAssetMenu(menuName = "Animals/Data")]
public class AnimalData : ScriptableObject
{
    public string id;
    public float speed;
    public float damage;
    public float maxHealth;
    public bool alwaysHostile;
    public float chaseDistance;
    public float attackCooldown;
    public float attackDistance;
}