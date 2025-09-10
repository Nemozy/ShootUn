using System;
using System.Threading;
using System.Threading.Tasks;
using Configs;
using Modules;
using UnityEngine;

public class Game
{
    public static GameData GameData { get; set; }

    private ModuleManager _moduleManager;

    public void Connect(GameData gameData)
    {
        GameData = gameData;
        _moduleManager = new ModuleManager();
    }

    public async Task Load()
    {
        await _moduleManager.LoadBattleModule(LoadBattle, QuitGame, GameData.BattleStages[0]);
    }

    public async void LoadBattle()
    {
        await _moduleManager.LoadBattleModule(LoadBattle, QuitGame, GameData.BattleStages[0]);
    }

    public async void Start()
    {
        await Task.CompletedTask;
    }

    public async void MainTick(float deltaTime)
    {
        await _moduleManager.MainTick(deltaTime);
    }

    public void OnApplicationQuit(CancellationToken ct)
    {
        _moduleManager.StopCurrentModule(ct).Wait(ct);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}