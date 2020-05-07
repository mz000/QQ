﻿using MathNet.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAgents : Agent
{
    [Header("-----------------------------------------------------------------------")]
    public EnemyController controller;
    public Person enemy;

    public Transform[] doors;
    public float timeout = 30f;

    private int wallCollisions = 0;

    protected override void Start()
    {
        base.Start();

        controller.health.OnDamage += OnDamaged;
        controller.health.OnDie += OnDied;

        enemy.health.OnDie += OnKilled;
    }

    protected override void Update()
    {
        base.Update();

        if(wallCollisions > 0)
        {
            SetReward(-Time.deltaTime);
        }

        SetReward(-Time.deltaTime);
    }

    protected override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        foreach(Bullet b in FindObjectsOfType<Bullet>())
        {
            Destroy(b.gameObject);
        }

        controller.transform.position = doors[Random.Range(0, doors.Length)].transform.position;
        enemy.transform.position = doors[Random.Range(0, doors.Length)].transform.position;

        controller.health.amount = 1000;
        controller.health.isDead = false;

        enemy.health.amount = 100;
        enemy.health.isDead = false;

        Invoke(nameof(Timeout), timeout);
    }

    protected override void SetInputs(float[] inputs)
    {
        Vector2 diff = enemy.transform.position - transform.position;

        int i = 0;
        foreach(Vector2 v in controller.GetSensorsData())
        {
            inputs[i++] = (v - (Vector2)transform.position).magnitude;
        }

        inputs[i++] = diff.normalized.x;
        inputs[i++] = diff.normalized.y;
        inputs[i++] = diff.magnitude;
    }

    // if you changed these settings after you trained the model - delete your model
    protected override void OnActions(float[] actions)
    {
        Vector3 dir = new Vector3(
                Mathf.Clamp((actions[0] - 0.5f) * 3, -1, 1),
                Mathf.Clamp((actions[1] - 0.5f) * 3, -1, 1),
                0
            ).normalized;

        if(dir.x == 0 && dir.y == 0)
        {
            return;
        }

        controller.CheckFacingAgent(dir);
        controller.rightHand.Trigger(dir);
    }

    protected override void BeforeEndingEpisode()
    {
        CancelInvoke(nameof(Timeout));
    }

    private void Timeout()
    {
        SetReward(-2);
        EndEpisode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            wallCollisions++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            wallCollisions--;
        }
    }

    private void OnDamaged()
    {
        SetReward(-0.5f);
    }

    private void OnDied()
    {
        SetReward(-2);
        EndEpisode();
    }

    private void OnKilled()
    {
        SetReward(10);
        EndEpisode();
    }
}