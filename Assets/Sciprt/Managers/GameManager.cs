using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    [SerializeField] private CheckPoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        checkpoints = FindObjectsOfType<CheckPoint>();

    }
    private void Start()
    {
    }
    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (CheckPoint checkPoint in checkpoints)
            {
                if (checkPoint.id == pair.Key && pair.Value == true)
                {
                    checkPoint.ActivateCheckpoint();
                }
            }
        }

        closestCheckpointId = _data.loadedCheckpointId;
        Invoke("PlacePlayerAtClosestCheckpoint",.1f);

    }

    private void PlacePlayerAtClosestCheckpoint()
    {
        foreach (CheckPoint checkPoint in checkpoints)
        {
            if (closestCheckpointId == checkPoint.id)
            {
                PlayerManager.instance.player.transform.position = checkPoint.transform.position;
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.loadedCheckpointId = FindCloestCheckpoint().id;
        _data.checkpoints.Clear();
        foreach (CheckPoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activated);
        }
    }

    private CheckPoint FindCloestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;

        foreach (CheckPoint checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position,
                checkpoint.transform.position);
            if (distanceToCheckpoint < closestDistance && checkpoint.activated == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;
    }
}
