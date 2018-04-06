using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _gamePhases = new List<GameObject>();

    public delegate void CurrentGamePhaseEvent(GameEnums.GamePhase phase);
    public static CurrentGamePhaseEvent CurrentGamePhase;

    GameEnums.GamePhase _currentPhase;

    void Awake()
    {
        if (_gamePhases.Count == 0)
        {
            Debug.LogWarning("GameLogicController: Game phases added via code, might not be in correct order!");
            for (int i = 0; i < transform.childCount; i++)
            {
                _gamePhases.Add(transform.GetChild(i).gameObject);
            }
        }

        _currentPhase = GameEnums.GamePhase.CALIBRATION_PHASE;
    }

    void Start()
    {
        if (CurrentGamePhase != null)
            CurrentGamePhase(GameEnums.GamePhase.CALIBRATION_PHASE);
    }
}
