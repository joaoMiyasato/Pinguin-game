using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Inimigo {inicio, ponto1, ponto2, atacando, voltando}

public class EnemyUI : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent enemy;
    private Transform player;
    public LayerMask whatIsPlayer;
    public Inimigo inimigo;

    public bool playerInRange;
    public float attackRange;

    public Vector3 point1;
    public Vector3 point2;
    public Vector3 startPoint;

    public GameObject patrolArea;

    void Start()
    {
        enemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;


        point1 = new Vector3 (transform.position.x + 5, transform.position.y, transform.position.y + 2);
        point2 = new Vector3 (transform.position.x - 5, transform.position.y, transform.position.y - 2);
        startPoint = transform.position;
        inimigo = Inimigo.inicio;

        playerInRange = Physics.CheckSphere(transform.position, attackRange);

        //AleatoryWalk();
        
        Debug.Log(point1);
        //inimigo = Inimigo.parado;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(patrolArea.GetComponent<PatrolArea>().onPatrolArea);


        //if(!playerInRange && inimigo == Inimigo.atacando) inimigo = Inimigo.voltando; AleatoryWalk();



        if(patrolArea.GetComponent<PatrolArea>().onPatrolArea == true)
        {
            enemy.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Player")
        {
            Debug.Log("Matou");
        }
    }

    /*void AleatoryWalk()
    {
        switch(inimigo)
        {
            case Inimigo.inicio:
                enemy.SetDestination(point1);
                inimigo = Inimigo.ponto1;
                Invoke(nameof(AleatoryWalk), 5f);
                break;
            case Inimigo.ponto1:
                enemy.SetDestination(point2);
                inimigo = Inimigo.ponto2;
                Invoke(nameof(AleatoryWalk), 5f);
                break;
            case Inimigo.ponto2:
                enemy.SetDestination(point1);
                inimigo = Inimigo.ponto1;
                Invoke(nameof(AleatoryWalk), 5f);
                break;
            /*case Inimigo.atacando:
                enemy.SetDestination(player.position);
                //inimigo = Inimigo.inicio;
                break;
            case Inimigo.voltando:
                enemy.SetDestination(startPoint);
                inimigo = Inimigo.inicio;
                Invoke(nameof(AleatoryWalk), 15f);
                break;
        }
    }*/
}
