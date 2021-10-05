using System;
using System.Collections.Generic;
using UnityEngine;
using Yusuf.AI.Pathfinding;
using Yusuf.Battle;
namespace Yusuf.AI.Behaviour
{
    public class Snipe : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;
        Transformnode transformnode;

        public int Run;
        public int Stop;
        public int Shot;
        void Start()
        {
            charactercontroller = GetComponent<CharacterController>();
            mov_stat = GetComponent<movement_stat>();
            animator = GetComponent<Animator>();
            transformnode = GetComponent<Transformnode>();
        }
        void Update()
        {
            if (SnipingNode)
            {
                if (!issearchcomplete)
                {
                    blindsearch();
                }
            }
        }
        void blindsearch()
        {
            SnipingNode.blindsearch();
        }
        node SnipingNode
        {
            get {if(battleground){return battleground.snipinggrounds;}return null;}
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
        node Currentnode
        {
            get { if (transformnode) return transformnode.Currentnode(); return null; }
        }
        Dictionary<node, int> fscore
        {
            get { return SnipingNode.Fscore(); }
        }
        bool issearchcomplete
        {
            get { return SnipingNode.IsSearchcomplete(); }
        }
        BattleGround battleground
        {
            get { return BattleGround.CurrentBattleGround; }
        }
        GameObject enemy
        {
            get { return battleground.Opponent(gameObject); }
        }
        public override void BeginBehaviour()
        {
            throw new NotImplementedException();
        }
        public override void UpdateBehaviour()
        {
            if (Currentnode && SnipingNode)
            {
                if (Currentnode != SnipingNode)
                {
                    if (issearchcomplete)
                    {
                        move();
                        animator.SetInteger("Behaviour", Run);//move to sniping position
                    }
                    else
                    {
                        animator.SetInteger("Behaviour", Stop);//wait
                    }
                }
                else
                {
                    lockon();
                    animator.SetInteger("Behaviour", Shot);//snip
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
