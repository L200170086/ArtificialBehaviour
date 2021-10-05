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
    public class BackTacker : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;
        Transformnode transformnode;

        public int Run;
        public int Attack;
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
        void movetowardenemy()
        {
            Vector3 lookpos = enemy.transform.position - transform.position;
            lookpos.y = 0;
            Quaternion desirerotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, desirerotation, 10*mov_stat.rotation_speed * Time.deltaTime);
            //movetoward
            Vector3 movement = transform.TransformDirection(new Vector3(0, -1, 1 * mov_stat.speed * Time.deltaTime));
            charactercontroller.Move(movement);
        }
        void avoidenemylineofsign()
        {
            Vector3 forward = (transform.position - enemy.transform.position).normalized;
            Vector3 left = new Vector3(-forward.z, forward.y, forward.x);
            Vector3 right = new Vector3(forward.z, forward.y, -forward.x);
            Vector3 localtarget = (forward+left)*10f;
            Vector3 target = enemy.transform.position + localtarget;
            Vector3 lookpos = target-transform.position;

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
        GameObject enemy
        {
            get
            {
                return battleground.Opponent(gameObject);
            }
        }
        bool isinenemyback()
        {
            Vector3 diffence = enemy.transform.InverseTransformPoint(transform.position);
            return (diffence.z < 0 && diffence.x > -5f && diffence.x < 5f);
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
                    if(isinenemyback())
                    {
                        if ((enemy.transform.position - transform.position).magnitude < 2f)
                        {
                            lockon();
                            animator.SetInteger("Behaviour", Attack);
                        }
                        else
                        {
                            movetowardenemy();
                            animator.SetInteger("Behaviour", Run);
                        }
                    }
                    else
                    {
                        avoidenemylineofsign();
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
