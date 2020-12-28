using Godot;
using System;

public class Weapon : Spatial
{
    [Export]
    private float cooldown = 3;
    [Export]
    public bool inUse;

    protected float curCooldown;
    protected bool canUse = true;

    protected AnimationPlayer anim;


    public override void _PhysicsProcess(float delta)
    {
        base._Process(delta);
        if (curCooldown > 0)
        {
            curCooldown -= 0.1f;
        }
        else 
        {
            canUse = true;
        }
    }

    //----------Method Functions
    public void setAnimator(Node holder)
    {
        anim = GetParent().GetParent().GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
