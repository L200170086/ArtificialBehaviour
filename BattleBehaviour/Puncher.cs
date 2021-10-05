using System;
using System.Collections.Generic;
using UnityEngine;
using Yusuf.Battle;
using Yusuf.AI.Pathfinding;
namespace Yusuf.AI.Behaviour
{
    /// <summary>
    /// has capability short and range attack
    /// </summary>
    public class Puncher : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;
        Transformnode transformnode;
        GameObject enemy;
        public int Run;
        public int Attack;
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
        void blindsearch()
        {
            BattlegroundNode.blindsearch();
        }
        void lockon()
        {
            Vector3 lookpos = enemy.transform.position - transform.position;
            lookpos.y = 0;
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, mov_stat.rotation_speed * Time.deltaTime);
        }
        bool isenemyinmeleerange()
        {
            foreach (GameObject i in battleground.Enemies)
            {
                if ((i.transform.position - transform.position).magnitude < 2f)
                {
                    enemy = i;
                    return true;
                }
            }
            enemy = battleground.Opponent(gameObject);
            return false;
        }
        node Currentnode
        {
            get { if (transformnode) return transformnode.Currentnode(); return null; }
        }
        node BattlegroundNode
        {
            get { if (battleground) return battleground.battleground; return null; }
        }
        BattleGround battleground
        {
            get { return BattleGround.CurrentBattleGround; }
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
        public override void BeginBehaviour()
        {
            throw new NotImplementedException();
        }

        public override void UpdateBehaviour()
        {
            if (Currentnode && BattlegroundNode)
            {
                if(Currentnode!=BattlegroundNode)
                {
                    if(issearchcomplete)
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
                    if(!isenemyinmeleerange())
                    {
                        lockon();
                        animator.SetInteger("Behaviour", RangeAttack);
                    }
                    else
                    {
                        lockon();
                        animator.SetInteger("Behaviour", Attack);
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
