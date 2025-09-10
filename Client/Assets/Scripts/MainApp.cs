using System;
using Configs;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MainApp : MonoBehaviour
{
    private Game _game;
    private GameData _gameData;
    private ApplicationTickUpdater _mainTickUpdater;
    private bool _isAppPaused;
    
    private void Awake()
    {
        _isAppPaused = false;
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 120;
        Application.runInBackground = false;
        QualitySettings.vSyncCount = 0;
        _mainTickUpdater = gameObject.AddComponent<ApplicationTickUpdater>();
    }

    private void Start()
    {
        Launch();
    }

    private void Launch()
    {
        LaunchAsync();
        _mainTickUpdater.OnPause += Paused;
        _mainTickUpdater.OnResume += Resumed;
        _mainTickUpdater.OnTick += OnUpdaterTick;
    }

    private async void LaunchAsync()
    {
        try
        {
            _gameData = await Addressables.LoadAssetAsync<GameData>("Configs/GameData").Task;
            Game.GameData = _gameData;
            var game = new Game();
            game.Connect(_gameData);
            await game.Load();
            _game = game;
            _game.Start();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _game = null;
        }
    }

    private void Paused()
    {
        _isAppPaused = true;
    }

    private void Resumed()
    {
        _isAppPaused = false;
    }

    private void OnUpdaterTick(float deltaTime)
    {
        _game?.MainTick(deltaTime);
    }
}