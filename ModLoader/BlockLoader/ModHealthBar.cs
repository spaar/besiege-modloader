using System;
using System.Collections.Generic;
using UnityEngine;

public class ModHealthBar : BlockHealthBar
{
    private float modHealth = 4f;
    public float ModHealth { get { return modHealth; } }
    private float maxHealth;
    public new Joint myJoint;
    private float startBreakForce;
    private float startBreakTorque;
    private float healthNormalised = 1f;
    public List<Renderer> myRenderers = new List<Renderer>();

    public override void DamageBlock(float amount)
    {
        if (this.myJoint != null)
        {
            this.modHealth = this.modHealth - amount;
            this.healthNormalised = this.modHealth / this.maxHealth;
            float single = this.startBreakForce * (amount / this.maxHealth);
            if (this.myJoint.breakForce - single <= (float)0)
            {
                this.myJoint.breakForce = (float)0;
            }
            else
            {
                this.myJoint.breakForce = this.myJoint.breakForce - single;
            }
            foreach (var renderer in myRenderers)
            {
                renderer.material.SetFloat("_DamageAmount", 1f - this.healthNormalised);
            }
        }
        else if (this.explodeCode != null)
        {
            this.explodeCode.Explodey();
        }
        this.SendDamageMessage();
    }
    public override void SendDamageMessage()
    {
        for (int i = 0; i < this.sendDamageMessage.Length; i++)
        {
            this.sendDamageMessage[i].SendMessage("DamageBlock", 1);
        }
    }
    public override void Start()
    {
        if (StatMaster.isSimulating)
        {
            this.maxHealth = this.modHealth;
            if (this.myJoint != null)
            {
                this.startBreakForce = this.myJoint.breakForce;
                this.startBreakTorque = this.myJoint.breakTorque;
            }
        }
    }
}