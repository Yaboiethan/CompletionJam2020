using Godot;
using System;

enum bossState { resting, firing}
public class Boss : EnemyAgent
{
    //Components
    private Timer timer;

    //Tracking
    private bossState mState = bossState.firing;
    private int curTime;
    private int shootTime = 8;
    private int restTime = 4;
    private float origDis;

    [Export]
    private int maxHp = 15;
    private int curHp;
    public override void _Ready()
    {
        base._Ready();
        //Get Components
        timer = GetNode<Timer>("Timer");
        //Set Components
        curHp = maxHp;
        timer.WaitTime = 1;
        timer.Start();

        origDis = engageDistance;
    }

    //Overrides
    public override void Hit()
    {
        if(isAlive && Visible)
        {
            curHp--;
            if(curHp <= 0)
            {
                anim.Stop();
                RotateX(90);
                base.Hit();
                return;
            }
            anim.Stop();
            anim.Play("Boss_Hurt");
            audio.Play();
        }
    }

    public override void _on_Timer_timeout()
    {
        base._on_Timer_timeout();
        if (isAlive)
        {
            //Work with states
            curTime--;
            if (curTime <= 0)
            {
                if (mState == bossState.firing)
                {
                    mGun.canShoot(false);
                    mState = bossState.resting;
                    engageDistance = 50;
                    curTime = restTime;
                }
                else
                {
                    mGun.canShoot(true);
                    mState = bossState.firing;
                    engageDistance = origDis;
                    curTime = shootTime;
                }
            }
        }
    }
}
