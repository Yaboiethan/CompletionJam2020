using Godot;
using System;

public class LevelManager : Node
{
    [Export]
    private AudioStream[] clips;

    private static RigidBody player;
    public Navigation nav;
    private AudioStreamPlayer musicBox;

    public override void _Ready()
    {
        base._Ready();
        player = GetNode<RigidBody>("Player");
        nav = GetNode<Navigation>("Level_Geometry");
        musicBox = GetNode<AudioStreamPlayer>("Music Box");
    }

    //--------Member Functions
    public static Vector3 getPlayerPos()
    {
        return player.Translation;
    }

    public virtual void onItemCollect(Item item)
    {
        
    }

    public void changeMusic(int clip)
    {
        musicBox.Stream = clips[clip];
        musicBox.Play();
    }

    public Navigation getNavigation()
    {
        return nav;
    }
}
