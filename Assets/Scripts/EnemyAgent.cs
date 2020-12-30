using Godot;
using System;

/* Enemy State Reference
 * Patroling: Moving along the patrolPath set
 * Alert: On guard, watching player and waiting for them to get closer
 * Combat: Actively seeking out player and attacking
 */
public enum EnemyState { Patroling, Alert, Combat}
public class EnemyAgent : KinematicBody
{
    [Export]
    private float regSpeed = 3;
    [Export]
    private float runSpeed = 5;
    [Export]
    public int damage = 10;
    [Export]
    private PackedScene gunOverride;
    [Export]
    private Mesh meshOverride;
    [Export]
    private EnemyState curState = EnemyState.Patroling;
    [Export]
    public float engageDistance = 15;
    [Export]
    private Vector3[] patrolPath;
    [Export]
    private bool canChangeStates = true;
    Vector3[] path;
    private int pathNode = 0;
    private int pathId;
    Vector3 target;

    public bool isAlive = true;
    private bool inShootingRange = false;

    //Components
    public AnimationPlayer anim;
    private LevelManager level;
    public AudioStreamPlayer3D audio;
    public Gun mGun;
    private Timer timer;

    public override void _Ready()
    {
        base._Ready();
        level = (LevelManager)GetParent();
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        audio = GetNode<AudioStreamPlayer3D>("Audio");
        timer = GetNode<Timer>("Timer");
        //Check for gun override
        mGun = (Gun)GetNode<Spatial>("Gun_Holder").GetChild(0);
        if (gunOverride != null)
        {
            //Delete old gun
            GetNode<Spatial>("Gun_Holder").GetChild(0).QueueFree();
            //Spawn new gun
            Gun g = (Gun)gunOverride.Instance();
            g.Translation = Vector3.Zero;
            GetNode("Gun_Holder").AddChild(g);
            mGun = g;
        }
        //Check for mesh override --different skin
        if (meshOverride != null)
        {
            GetNode<MeshInstance>("MeshInstance").Mesh = meshOverride;
        }
        mGun.usedByEnemy = true;
        mGun.onEnemySelection(anim);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if(isAlive && Visible)
        {
            float dis;
            //Act on task, based on state
            switch(curState)
            {
                case EnemyState.Patroling:
                    //Check if null
                    if (path == null)
                    {
                        moveTo(patrolPath[0]);
                    }
                    Vector3 disToPoint = patrolPath[pathNode] - Translation;
                    //Check if done and need to move to next path node
                    if (disToPoint.Length() < 0.4f)
                    {
                        //Move to next or start
                        if (pathNode >= patrolPath.Length - 1)
                        {
                            pathNode = 0;
                        }
                        else
                        {
                            pathNode++;
                        }
                        moveTo(patrolPath[pathNode]);
                    }
                    //Move to point
                    if (pathId < path.Length)
                    {
                        Vector3 moveVec = (path[pathId] - GlobalTransform.origin);
                        if (moveVec.Length() < 0.1)
                        {
                            pathId += 1;
                            target = path[pathId];
                        }
                        else
                        {
                            MoveAndSlide(moveVec.Normalized() * regSpeed, Vector3.Up);
                            LookAt(target, Vector3.Up);
                        }
                    }
                    break;

                case EnemyState.Alert:
                    //Standing in place, waiting for player to approach
                    target = LevelManager.getPlayerPos();
                    LookAt(target, Vector3.Up);
                    //Get distance
                    dis = (target - Translation).Length();
                    if (dis <= engageDistance)
                    {
                        changeEnemyState(EnemyState.Combat);
                    }
                    break;

                case EnemyState.Combat:
                    //Move closer to target
                    target = LevelManager.getPlayerPos();
                    dis = (target - Translation).Length();
                    //Check if in range
                    if (dis <= engageDistance - 3)
                    {
                        inShootingRange = true;
                    }
                    else
                    {
                        inShootingRange = false;
                    }
                    LookAt(target, Vector3.Up);
                    Rotation = new Vector3(Mathf.Clamp(Rotation.x, -10, 10), Rotation.y, Rotation.z);
                    if (inShootingRange)
                    {
                        //Attack Player if possible
                        if (mGun.canShoot())
                        {
                            mGun.Shoot();
                        }
                    }
                    else
                    {
                        //Move closer to player
                        if (pathId < path.Length)
                        {
                            Vector3 moveVec = (path[pathId] - GlobalTransform.origin);
                            if (moveVec.Length() < 0.1)
                            {
                                pathId += 1;
                            }
                            else
                            {
                                MoveAndSlide(moveVec.Normalized() * runSpeed, Vector3.Up);
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void moveTo(Vector3 pos)
    {
        path = level.getNavigation().GetSimplePath(Translation, pos);
        pathId = 0;
    }

    public virtual void Hit()
    {
        if (isAlive && Visible)
        {
            anim.Play("Agent_Dead");
            audio.Play();
            isAlive = false;
            //Disable hitbox
            GetNode<CollisionShape>("CollisionShape").Disabled = true;
            //Trigger despawning thing
            timer.Stop();
            timer.WaitTime = 7;
            timer.Start();
        }
    }

    public bool getAlive()
    {
        return isAlive;
    }

    public void changeEnemyState(EnemyState s)
    {
        curState = s;
        if (s == EnemyState.Combat)
        {
            moveTo(LevelManager.getPlayerPos());
        }
    }
    //----------Signals
    public virtual void _on_Timer_timeout()
    {
        if (!isAlive)
        {
            timer.Stop();
            QueueFree();
            return;
        }

        if (curState == EnemyState.Combat)
        {
            if(!inShootingRange)
            {
                moveTo(LevelManager.getPlayerPos());
                timer.Start();
            }
        }
    }
}
