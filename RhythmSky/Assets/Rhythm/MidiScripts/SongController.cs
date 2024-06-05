using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SongController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject hitObject;
    [SerializeField] Material[] puckMaterials;
    [SerializeField] float puckSpeed;
    public AudioClip songAudio;
    AudioSource source;
    bool paused = true;
    MidiReader reader;
    Vector3 forceDirection;
    [SerializeField] Transform[] puckTransforms;
    [SerializeField] GameObject moveTowards;

    void OnEnable()
    {
        reader = GetComponent<MidiReader>();
        float beforetime = Time.timeSinceLevelLoad;

        float distanceToTravel = (transform.position - moveTowards.transform.position).magnitude;
        reader.SetNoteDelay(distanceToTravel / puckSpeed);
        reader.SetSongData("Assets/Midi/no name(2).mid", songAudio);
        
        reader.NoteCall += CreateNote;

        forceDirection = (moveTowards.transform.position - transform.position).normalized;
    }

    public void PauseSong()
    {
        paused = true;
        reader.PauseSong();
    }

    public void StartSong()
    {
        paused = false;
        reader.StartSong();
    }

    private void FixedUpdate()
    {
        if (!paused)
        {
            if (!source.isPlaying)
            {
                Debug.Log("song has ended");
                //end song and show score UI
            }
        }
    }

    public void CreateNote(List<NoteData> notes)
    {
        foreach (NoteData note in notes)
        {
            GameObject noteObject = null;
            switch (note.LaneNo)
            {
                case 1:
                    noteObject = Instantiate(hitObject, puckTransforms[0].position, puckTransforms[0].rotation);
                    break;
                case 2:
                    noteObject = Instantiate(hitObject, puckTransforms[1].position, puckTransforms[1].rotation);
                    break;
                case 3:
                    noteObject = Instantiate(hitObject, puckTransforms[2].position, puckTransforms[2].rotation);
                    break;
                case 4:
                    noteObject = Instantiate(hitObject, puckTransforms[3].position, puckTransforms[3].rotation);
                    break;
            }
            if (noteObject != null)
            {
                noteObject.GetComponent<Rigidbody>().velocity = forceDirection * puckSpeed;
                noteObject.GetComponent<Renderer>().material = puckMaterials[note.LaneNo - 1];
                noteObject.GetComponent<NoteFunctionality>().SetLaneNo(note.LaneNo);
            }
        }
        
    }


}
