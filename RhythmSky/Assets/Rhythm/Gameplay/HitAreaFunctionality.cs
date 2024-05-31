using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitAreaFunctionality : MonoBehaviour
{
    PlayerInput m_input;
    List<NoteFunctionality> m_lane1notes;
    List<NoteFunctionality> m_lane2notes;
    List<NoteFunctionality> m_lane3notes;
    List<NoteFunctionality> m_lane4notes;
    [SerializeField] AudioSource m_inputNoise;

    public event System.Action<Scores> ScoreEvent;

    private void OnEnable()
    {
        m_lane1notes = new();
        m_lane2notes = new();
        m_lane3notes = new();
        m_lane4notes = new();
        m_input = GetComponent<PlayerInput>();
        m_inputNoise = GetComponent<AudioSource>();
        m_input.currentActionMap.FindAction("Lane 1").performed += ReactLane1;
        m_input.currentActionMap.FindAction("Lane 2").performed += ReactLane2;
        m_input.currentActionMap.FindAction("Lane 3").performed += ReactLane3;
        m_input.currentActionMap.FindAction("Lane 4").performed += ReactLane4;
    }

    void ReactLane1(InputAction.CallbackContext context)
    {
        //play sound here
        m_inputNoise.Play();
        if (m_lane1notes.Count == 0) return;
        ScoreEvent?.Invoke(m_lane1notes[0].React());
        GameObject obj = m_lane1notes[0].gameObject;
        m_lane1notes.Remove(m_lane1notes[0]);
        Destroy(obj);

    }

    void ReactLane2(InputAction.CallbackContext context)
    {
        //play sound here
        m_inputNoise.Play();
        if (m_lane2notes.Count == 0) return;
        ScoreEvent?.Invoke(m_lane2notes[0].React());
        GameObject obj = m_lane2notes[0].gameObject;
        m_lane2notes.Remove(m_lane2notes[0]);
        Destroy(obj);
    }

    void ReactLane3(InputAction.CallbackContext context)
    {
        m_inputNoise.Play();
        if (m_lane3notes.Count == 0) return;
        ScoreEvent?.Invoke(m_lane3notes[0].React());
        GameObject obj = m_lane3notes[0].gameObject;
        m_lane3notes.Remove(m_lane3notes[0]);
        Destroy(obj);
    }

    void ReactLane4(InputAction.CallbackContext context)
    {
        m_inputNoise.Play();
        if (m_lane4notes.Count == 0) return;
        ScoreEvent?.Invoke(m_lane4notes[0].React());
        GameObject obj = m_lane4notes[0].gameObject;
        m_lane4notes.Remove(m_lane4notes[0]);
        Destroy(obj);

    }

    private void OnTriggerEnter(Collider other)
    {
        NoteFunctionality note;
        other.gameObject.TryGetComponent<NoteFunctionality>(out note);
        
        switch(note.GetLaneNo())
        {
            case 1:
                m_lane1notes.Add(note);
                break;
            case 2:
                m_lane2notes.Add(note);
                break;
            case 3:
                m_lane3notes.Add(note);
                break;
            case 4:
                m_lane4notes.Add(note);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.IsDestroyed()) return;
        ScoreEvent?.Invoke(Scores.Miss);
        NoteFunctionality note;
        other.gameObject.TryGetComponent<NoteFunctionality>(out note);
        

        switch (note.GetLaneNo())
        {
            case 1:
                m_lane1notes.Remove(note);
                break;
            case 2:
                m_lane2notes.Remove(note);
                break;
            case 3:
                m_lane3notes.Remove(note);
                break;
            case 4:
                m_lane4notes.Remove(note);
                break;
        }
    }
}
