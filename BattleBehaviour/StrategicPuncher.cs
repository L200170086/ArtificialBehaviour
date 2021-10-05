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
    public class StrategicPuncher : ArtificialBehaviour
    {
        CharacterController charactercontroller;
        movement_stat mov_stat;
        Animator animator;
        Transformnode transformnode;
        GameObject[] enemiesinmeleerange;
        int count;

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
            enemiesinmeleerange = new GameObject[2];
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
        void avoidenemy()
        {
            Vector3 lookpos = transform.position - enemy.transform.position;
            lookpos.y = 0; lookpos = lookpos.normalized;
            Vector3 futureposition = transform.position + lookpos;
            if (!BattlegroundNode.inarea(futureposition))
            {
                lookpos = new Vector3(lookpos.z, lookpos.y, -lookpos.x);
                futureposition = transform.position + lookpos;
                if (!BattlegroundNode.inarea(futureposition))
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
            get {
                if (count > 0) return enemiesinmeleerange[0];
                return battleground.Opponent(gameObject);
            }
        }
        bool isenemyinmeleerange()
        {
            count = 0;
            foreach (GameObject i in battleground.Enemies)
            {
                if ((i.transform.position - transform.position).magnitude < 1f)
                {
                    enemiesinmeleerange[count] = i;
                    count++; if (count == enemiesinmeleerange.Length) break;
                }
            }
            return count > 0;
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
                    if (isenemyinmeleerange())
                    {
                        if (count == enemiesinmeleerange.Length)
                        {
                            avoidenemy();
                            animator.SetInteger("Behaviour", Run);
                        }
                        else
                        {
                            lockon();
                            animator.SetInteger("Behaviour", Attack);
                        }
                    }
                    else
                    {
                        lockon();
                        animator.SetInteger("Behaviour", RangeAttack);
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
