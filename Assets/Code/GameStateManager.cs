using Ilumisoft.VisualStateMachine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public class GameStateManager : MonoSingleton<GameStateManager>
{
    private StateMachine _StateMachine;
    public Level[] Levels;

    public void Start() 
    {
        _StateMachine = GetComponent<StateMachine>();
        _StateMachine.enabled = true;
    }

    public void GameState_OnStart()
    {
        StartCoroutine(ShowIntroduction());
    }

    public IEnumerator ShowIntroduction()
    {
        yield return new WaitForSeconds(1.0f);
        yield return Transitioner.Instance.TransitionInCoroutine();
        _StateMachine.TriggerByLabel("on_start_game");
        Debug.Log(TurnStateManager.GetInstance());
        TurnStateManager.GetInstance().TurnState_OnStart();
    }

    public void GameState_OnProgression()
    {
        foreach(var oldLevel in GameObject.FindGameObjectsWithTag("Level"))
        {
            Destroy(oldLevel.gameObject);
        }

        var levelPrefab = Levels[Random.Range(0, Levels.Length)];
        var levelInstance = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
        var level = levelInstance.GetComponent<Level>();
        
        var enemies = new List<CharacterTemplate>();
        CharacterCardConfiguration.CreateTeam(30, level.FoePositions.childCount, ref enemies);

        var enemySpawnPoints = new List<Vector3>();
        for (int i = 0; i < level.FoePositions.childCount; i++)
        {
            enemySpawnPoints.Add(level.FoePositions.GetChild(i).position);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            int spawnIndex = Random.Range(0, enemySpawnPoints.Count);
            var spawnPoint = enemySpawnPoints[spawnIndex];
            enemySpawnPoints.RemoveAt(spawnIndex);

            var instance = Instantiate(CharacterManager.GetInstance().CharacterPrefab, spawnPoint, Quaternion.identity);
            instance.GetComponent<Character>().Spawn(enemies[i], CombatSide.Foe);
        }
        StartCoroutine(ShowProgression());
    }

    public IEnumerator ShowProgression()
    {
        yield return new WaitForSeconds(1.0f);
        _StateMachine.TriggerByLabel("on_progression_complete");
    }
}

