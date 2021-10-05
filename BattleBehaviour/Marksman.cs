using System;
using System.Collections.Generic;
using UnityEngine;
using Yusuf.Battle;
using Yusuf.AI.Pathfinding;
namespace Yusuf.AI.Behaviour
{
    /// <summary>
    /// has capability short and range attack
    /// different from generic puncher add relocate behaviour
    /// </summary>
    public class Marksman : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;
        Transformnode transformnode;
        GameObject enemy;

        public int Run;
        public int RangeAttack;
        public int Idle;
        void Start()
        {
            charactercontroller = GetComponent<CharacterController>();
            mov_stat = GetComponent<movement_stat>();
            animator = GetComponent<Animator>();
            transformnode = GetComponent<Transformnode>();
        }
        void Update()
        {
            if (BattlegroundNode)
            {
                if (!issearchcomplete)
                {
                    blindsearch();
                }
            }
        }
        void blindsearch()
        {
            BattlegroundNode.blindsearch();
        }
        void move()
        {
            Vector3 lookpos = next().transform.position - transform.position;
            lookpos.y = 0;
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, mov_stat.rotation_speed * Time.deltaTime);
            //movetoward
            Vector3 movement = transform.TransformDirection(new Vector3(0, -1, 1 * mov_stat.speed * Time.deltaTime));
            charactercontroller.Move(movement);
        }
        void lockon()
        {
            Vector3 lookpos = enemy.transform.position - transform.position;
            lookpos.y = 0;
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, mov_stat.rotation_speed * Time.deltaTime);
        }
        void avoidenemy()
        {
            Vector3 lookpos = transform.position - enemy.transform.position;
            lookpos.y = 0;lookpos = lookpos.normalized;
            Vector3 futureposition = transform.position + lookpos;
            if(!BattlegroundNode.inarea(futureposition))
            {
                lookpos = new Vector3(lookpos.z, lookpos.y, -lookpos.x);
                futureposition = transform.position + lookpos;
                if(!BattlegroundNode.inarea(futureposition))
                {
                    lookpos = -lookpos;
                }
            }
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, mov_stat.rotation_speed * Time.deltaTime);
            //movetoward
            Vector3 movement = transform.TransformDirection(new Vector3(0, -1, 1 * mov_stat.speed * Time.deltaTime));
            charactercontroller.Move(movement);
        }
        BattleGround battleground
        {
            get { return BattleGround.CurrentBattleGround; }
        }
        node Currentnode
        {
            get { if (transformnode) return transformnode.Currentnode(); return null; }
        }
        node BattlegroundNode
        {
            get { if (battleground) return battleground.battleground; return null; }
        }
        node next()
        {
            node leastfscore = Currentnode;
            foreach (node neightbor in leastfscore.Neightbor)
            {
                if (fscore[neightbor] < fscore[leastfscore])
                {
                    leastfscore = neightbor;
                }
            }
            return leastfscore;
        }
        Dictionary<node, int> fscore
        {
            get { return BattlegroundNode.Fscore(); }
        }
        bool issearchcomplete
        {
            get { return BattlegroundNode.IsSearchcomplete(); }
        }
        bool isenemynearest()
        {
            foreach (GameObject i in battleground.Enemies)
            {
                if ((i.transform.position - transform.position).magnitude < 10f)
                {
                    enemy = i;
                    return true;
                }
            }
            enemy = battleground.Opponent(gameObject);
            return false;
        }
        public override void BeginBehaviour()
        {
            throw new NotImplementedException();
        }
        public override void UpdateBehaviour()
        {
            if (Currentnode && BattlegroundNode)
            {
                if (Currentnode != BattlegroundNode)
                {
                    if (issearchcomplete)
                    {
                        move();
                        animator.SetInteger("Behaviour", Run);//assaultbattleground
                    }
                    else
                    {
                        animator.SetInteger("Behaviour", Idle);//wait
                    }
                }
                else
                {
                    if (!isenemynearest())
                    {
                        lockon();
                        animator.SetInteger("Behaviour", RangeAttack);
                    }
                    else
                    {
                        avoidenemy();//getoutfrom hot zone
                        animator.SetInteger("Behaviour", Run);
                    }
                }
            }
            else
            {
                animator.SetInteger("Behaviour", Idle);
            }
        }
        public override void EndBehaviour()
        {
            throw new NotImplementedException();
        }

    }
}
