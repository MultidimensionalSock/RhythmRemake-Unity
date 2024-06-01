using System.Collections;
using System.Collections.Generic;
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
    PlayerInput m_input;
    Vector3 forceDirection;
    [SerializeField] Transform[] puckTransforms;
    [SerializeField] GameObject moveTowards;

    void Start()
    {
        m_input = GetComponent<PlayerInput>();
        reader = GetComponent<MidiReader>();
        reader.SetMidiLocation("Assets/Midi/Vocaloid - 1925.mid");
        reader.NoteCall += CreateNote;
        StartSong();
        source = GetComponent<AudioSource>();
        source.clip = songAudio;

        forceDirection = (moveTowards.transform.position - transform.position).normalized;
    }

    public void PauseSong()
    {
        paused = true;
        reader.PauseSong();
    }

    public void StartSong()
    {
        reader.StartSong();
        StartCoroutine(startSongAudio());
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

    IEnumerator startSongAudio()
    {
        float beforetime = Time.timeSinceLevelLoad;
        float hitPointPos = transform.parent.GetChild(5).position.x;
        //should change this to not be hard coded
        float spawnPosition = -49;

        float distanceToTravel = Mathf.Abs(spawnPosition - hitPointPos);
        float time = distanceToTravel / puckSpeed;

        yield return new WaitForSeconds(time - (Time.timeSinceLevelLoad - beforetime));
        source.Play();
        paused = false;
    }

    void CreateNote(List<NoteData> notes)
    {
        foreach (NoteData note in notes)
        {
            GameObject noteObject = null;
            //should change the below to not be hard coded
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
