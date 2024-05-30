using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitAreaFunctionality : MonoBehaviour
{
    PlayerInput m_input;
    Queue<NoteFunctionality> m_lane1notes;
    Queue<NoteFunctionality> m_lane2notes;
    Queue<NoteFunctionality> m_lane3notes;
    Queue<NoteFunctionality> m_lane4notes;

    public event System.Action<Scores> ScoreEvent;

    private void OnEnable()
    {
        m_lane1notes = new Queue<NoteFunctionality>();
        m_lane2notes = new Queue<NoteFunctionality>();
        m_lane3notes = new Queue<NoteFunctionality>();
        m_lane4notes = new Queue<NoteFunctionality>();
        m_input = GetComponent<PlayerInput>();
        m_input.currentActionMap.FindAction("Lane 1").performed += ReactLane1;
        m_input.currentActionMap.FindAction("Lane 2").performed += ReactLane2;
        m_input.currentActionMap.FindAction("Lane 3").performed += ReactLane3;
        m_input.currentActionMap.FindAction("Lane 4").performed += ReactLane4;
    }

    void ReactLane1(InputAction.CallbackContext context)
    {
        //play sound here
        if (m_lane1notes.Count != 0)
        {
            ScoreEvent?.Invoke(m_lane1notes.Peek().React());
            Destroy(m_lane1notes.Peek().gameObject);
        }

    }

    void ReactLane2(InputAction.CallbackContext context)
    {
        //play sound here
        if (m_lane2notes.Count == 0) return;
        ScoreEvent?.Invoke(m_lane2notes.Peek().React());
        Destroy(m_lane2notes.Peek().gameObject);
    }

    void ReactLane3(InputAction.CallbackContext context)
    {
        //play sound here
        if (m_lane3notes.Count == 0) return;
        ScoreEvent?.Invoke(m_lane3notes.Peek().React());
        Destroy(m_lane3notes.Peek().gameObject);
    }

    void ReactLane4(InputAction.CallbackContext context)
    {
        //play sound here
        if (m_lane3notes.Count == 0) return;
        ScoreEvent?.Invoke(m_lane3notes.Peek().React());
        Destroy(m_lane3notes.Peek().gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        
        NoteFunctionality note;
        other.gameObject.TryGetComponent<NoteFunctionality>(out note);
        
        switch(note.GetLaneNo())
        {
            case 1:
                m_lane1notes.Enqueue(note);
                break;
            case 2:
                m_lane2notes.Enqueue(note);
                break;
            case 3:
                m_lane3notes.Enqueue(note);
                break;
            case 4:
                m_lane4notes.Enqueue(note);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        NoteFunctionality note;
        other.gameObject.TryGetComponent<NoteFunctionality>(out note);

        switch (note.GetLaneNo())
        {
            case 1:
                m_lane1notes.Dequeue();
                break;
            case 2:
                m_lane2notes.Dequeue();
                break;
            case 3:
                m_lane3notes.Dequeue();
                break;
            case 4:
                m_lane4notes.Dequeue();
                break;
        }
    }
}
