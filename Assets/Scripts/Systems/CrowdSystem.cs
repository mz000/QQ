﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Authro : Mh
// This manages spawning of enemies
public class CrowdSystem : MonoBehaviour
{
    public static int enemiesCount = 0, waveNumber = 1;
    public static float combo = 1;

    public GameObject SimpleEnemyPref, arrowPref;
    public GameObject[] enemyPrefs;
    public Transform[] doors;

    public float lvlUpRate = 0.1f;
    public float timeBetweenWaves = 5f, comboReset = 3f;

    [SerializeField]
    private float _chunckSize = 1, _betweenSpawns = 5, _betweenSteps = 0.5f, _hardEnemyProbability = 0.1f;
    private int _lvl, _kills, _killsTillNext = 3;

    private float _lvlSpeedDiff = 10;
    private float _temperatureMultiplier = 1;

    private Transform _target;
    private PlayerController _pc;

    private float _time = 0;

    private List<IndicatorArrow> usedArrow = new List<IndicatorArrow>();
    private List<IndicatorArrow> usefulArrow = new List<IndicatorArrow>();

    private bool restingBetweenWaves = false;

    public virtual void SpeedUp()
    {
        _hardEnemyProbability += _hardEnemyProbability * 1.5f;
        if (_hardEnemyProbability > 100) _hardEnemyProbability = 100;

        _betweenSpawns -= lvlUpRate * _betweenSpawns;
        _betweenSteps -= lvlUpRate / 2 * _betweenSteps;
        if (_betweenSpawns < 0.01f)
        {
            _betweenSpawns = 0.01f;
        }
        if(_betweenSteps < 0.2f)
        {
            _betweenSteps = 0.2f;
        }
    }

    public virtual void LevelUp()
    {
        _lvl++;
        _killsTillNext *= _killsTillNext;
        _chunckSize *= 1.2f;
        SpeedUp();
    }

    public virtual void GotKill()
    {
        enemiesCount--;

        if(enemiesCount <= 0)
        {
            enemiesCount = 0;
            restingBetweenWaves = true;
            LevelUp();
        }

        combo *= 1.2f;
        Instantiate(Statics.instance.scoreText, _pc.transform.position, Quaternion.identity).GetComponent<TextMesh>().text = "x" + CrowdSystem.combo;
        CancelInvoke(nameof(ResetCombo));
        Invoke(nameof(ResetCombo), comboReset);

        //_kills++;
        //if(_kills > _killsTillNext)
        //{
        //    _kills = 0;
        //    LevelUp();
        //}
    }

    public virtual void GotKill(IndicatorArrow arrow)
    {
        GotKill();

        arrow.working = false;
        arrow.SetRendering(false);
        usedArrow.Remove(arrow);
        usefulArrow.Add(arrow);
    }

    protected virtual void Start()
    {
        combo = 1;
        enemiesCount = 0;
        waveNumber = 0;
        AssignTarget();

        Invoke(nameof(Spawn), 2);
    }

    protected virtual void Update()
    {
        if (restingBetweenWaves)
        {
            TimerTick();
        }
    }

    private void ResetCombo()
    {
        combo = 1;
    }

    private void TimerTick()
    {
        //_time += Time.deltaTime;
        //if (_time >= _betweenSpawns && enemiesCount < _maxEnemies)
        //{
        //    _time = 0;
        //    Spawn();
        //}

        _time += Time.deltaTime;
        if (_time >= timeBetweenWaves)
        {
            restingBetweenWaves = false;
            _time = 0;
            Spawn();
        }
    }

    private void AssignTarget()
    {
        try
        {
            _pc = FindObjectOfType<PlayerController>();
            _target = _pc.transform;
        }
        catch
        {
            Debug.LogError("No player in the scene");
        }
    }

    private void Spawn()
    {
        Statics.instance.messageSystem.ShowMessage("Wave " + waveNumber++);
        for(int i = 0; i < _chunckSize; i++)
        {
            SpawnOne();
        }
    }

    private void SpawnOne()
    {
        Vector3 spawnPos = doors[Random.Range(0, doors.Length)].position;

        GameObject enemyType = SimpleEnemyPref;
        float betweenstps = _betweenSteps;
        if(Random.Range(0f, 100f) < _hardEnemyProbability)
        {
            enemyType = enemyPrefs[Random.Range(0, enemyPrefs.Length)];
            betweenstps *= 2;
        }

        Enemy enemy = Instantiate(enemyType, spawnPos, Quaternion.identity).GetComponent<Enemy>();
        enemy.AssignTarget(_target);
        enemy.AssignCS(this);
        enemy.timeBetweensteps = betweenstps;
        enemiesCount++;

        IndicatorArrow arrow;
        if (usefulArrow.Count > 0)
        {
            arrow = usefulArrow[0];
            usefulArrow.RemoveAt(0);
            arrow.working = true;
            arrow.SetRendering(true);
        }
        else
        {
            arrow = Instantiate(arrowPref, transform).GetComponent<IndicatorArrow>();
            arrow.working = true;
            arrow.SetRendering(true);
            usedArrow.Add(arrow);
        }

        arrow.target = enemy;
        enemy.indicator = arrow;
    }
}