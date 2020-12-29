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
    public UI uiManager;

    public override void _Ready()
    {
        base._Ready();
        uiManager = GetTree().Root.GetChild(0).GetNode<UI>("UI");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._Process(delta);
        if (curCooldown > 0)
        {
            curCooldown -= 0.1f;
            canUse = false;
        }
        else 
        {
            canUse = true;
        }
    }

    //----------Method Functions
    public virtual void setAnimator(Node holder)
    {
        anim = GetParent().GetParent().GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
    }

}
