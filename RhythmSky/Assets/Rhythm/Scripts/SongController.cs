using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject hitObject;
    void Start()
    {
        MidiReader reader = GetComponent<MidiReader>();
        reader.SetMidiLocation("Assets/Midi/Vocaloid - 1925.mid", null);
        reader.NoteCall += CreateNote;
        reader.StartSong();
        
    }

    void CreateNote(List<NoteData> notes)
    {
        Debug.Log("called");
        GameObject noteObject;
        foreach (NoteData note in notes)
        {
            switch(note.LaneNo)
            {
                case 1:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, 1.0f), Quaternion.identity);
                    break;
                case 2:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, -1.5f), Quaternion.identity);
                    break;
                case 3:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, -4.0f), Quaternion.identity);
                    break;
                case 4:
                    noteObject = Instantiate(hitObject, new Vector3(-49.0f, -0.2f, -6.5f), Quaternion.identity);
                    break;
            }
        }
    }

}
