using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is to be attached to an object and manipulated through the object's controller
public class Stats : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float health;
    [Space]
    [SerializeField] float maxMana;
    [SerializeField] float mana;
    [Space]
    [SerializeField] float attackDamage;
    [SerializeField] float attackSpeed;
    [SerializeField] float defenseResistance;

    //setters
    public void setMaxHealth(float _maxHealth)
    {
        maxHealth = _maxHealth;
    }
    public void setHealth(float _health)
    {
        health = _health;
    }
    public void setMaxMana(float _maxMana)
    {
        maxMana = _maxMana;
    }
    public void setMana(float _mana)
    {
        mana = _mana;
    }
    public void setAttackDamage(float _attackDamage)
    {
        attackDamage = _attackDamage;
    }
    public void setDefenseResistance(float _defenseResistance)
    {
        defenseResistance = _defenseResistance;
    }
    public void setAttackSpeed(float _attackSpeed)
    {
        attackSpeed = _attackSpeed;
    }

    //getters
    public float getMaxHealth()
    {
        return maxHealth;
    }
    public float getHealth()
    {
        return health;
    }
    public float getMaxMana() 
    { 
        return maxMana;
    }
    public float getMana()
    {
        return mana;
    }
    public float getAttackDamage()
    {
        return attackDamage;
    }
    public float getDefenseResistance()
    {
        return defenseResistance;
    }
    public float getAttackSpeed()
    {
        return attackSpeed;
    }

    //incrementing and decrementing functions
    public void incrementHealth(float incrementAmount)
    {
        if ((health + incrementAmount) > maxHealth)
            health = maxHealth;
        else
            health += incrementAmount;
    }
    public void decrementHealth(float decrementAmount)
    {
        if ((health - decrementAmount) < 0)
            health = 0;
        else
            health -= decrementAmount;
    }
    public void incrementMana(float incrementAmount)
    {
        if ((mana + incrementAmount) > maxMana)
            mana = maxMana;
        else
            mana += incrementAmount;
    }
    public void decrementMana(float decrementAmount)
    {
        if ((mana - decrementAmount) < 0)
            mana = 0;
        else
            mana -= decrementAmount;
    }
    public void incrementAttackDamage(float incrementAmount)
    {
        attackDamage += incrementAmount;
    }
    public void decrementAttackDamage(float decrementAmount)
    {
        if ((attackDamage - decrementAmount) < 0)
            attackDamage = 0;
        else
            attackDamage -= decrementAmount;
    }
    public void incrementDefense(float incrementAmount)
    {
        defenseResistance += incrementAmount;
    }
    public void decrementDefense(float decrementAmount)
    {
        if ((defenseResistance - decrementAmount) < 0)
            defenseResistance = 0;
        else
            defenseResistance -= decrementAmount;
    }
    public void incrementAttackSpeed(float incrementAmount)
    {
        attackSpeed += incrementAmount;
    }
    public void decrementAttackSpeed(float decrementAmount)
    {
        if ((attackSpeed - decrementAmount) < 0)
            attackSpeed = 0;
        else
            attackSpeed -= decrementAmount;
    }
}
