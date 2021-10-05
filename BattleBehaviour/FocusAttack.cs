using System;
using System.Collections.Generic;
using UnityEngine;
using Yusuf.AI.Pathfinding;
using Yusuf.Battle;
namespace Yusuf.AI.Behaviour
{
    public class FocusAttack : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;
        Transformnode transformnode;

        public int Run;
        public int Stop;
        public int Attack;

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
        void movetowardenemy()
        {
            Vector3 lookpos = enemy.transform.position - transform.position;
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
        node Currentnode
        {
            get { if (transformnode) return transformnode.Currentnode(); return null; }
        }
        node BattlegroundNode
        {
            get { if (battleground) return battleground.battleground; return null; }
        }
        bool issearchcomplete
        {
            get { return BattlegroundNode.IsSearchcomplete(); }
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
        BattleGround battleground
        {
            get { return BattleGround.CurrentBattleGround; }
        }
        GameObject enemy
        {
            get{return battleground.Opponent(gameObject);}
        }
        Dictionary<node, int> fscore
        {
            get { return BattlegroundNode.Fscore(); }
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
                        animator.SetInteger("Behaviour", Stop);//wait
                    }
                }
                else
                {
                    if ((enemy.transform.position-transform.position).magnitude<2f)
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
            }
            else
            {
                animator.SetInteger("Behaviour", Stop);
            }
        }
        public override void EndBehaviour()
        {
            throw new NotImplementedException();
        }

    }
}
