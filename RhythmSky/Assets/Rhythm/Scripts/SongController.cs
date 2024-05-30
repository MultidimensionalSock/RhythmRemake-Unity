using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        reader = GetComponent<MidiReader>();
        reader.SetMidiLocation("Assets/Midi/Vocaloid - 1925.mid");
        reader.NoteCall += CreateNote;
        StartSong();
        source = GetComponent<AudioSource>();
        source.clip = songAudio;
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
        Debug.Log("called");
        GameObject noteObject = null;
        foreach (NoteData note in notes)
        {
            //should change the below to not be hard coded
            switch (note.LaneNo)
            {
                case 1:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, 1.0f), Quaternion.identity);
                    noteObject.GetComponent<Renderer>().material = puckMaterials[0];
                    break;
                case 2:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, -1.5f), Quaternion.identity);
                    noteObject.GetComponent<Renderer>().material = puckMaterials[1];
                    break;
                case 3:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, -4.0f), Quaternion.identity);
                    noteObject.GetComponent<Renderer>().material = puckMaterials[2];
                    break;
                case 4:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, -6.5f), Quaternion.identity);
                    noteObject.GetComponent<Renderer>().material = puckMaterials[3];
                    break;
            }
        }
        if (noteObject != null)
        {
            noteObject.GetComponent<Rigidbody>().velocity = new Vector3(puckSpeed, 0, 0);
        }
    }

}
