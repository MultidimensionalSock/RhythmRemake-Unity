using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SongController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject hitObject;
    [SerializeField] Material[] puckMaterials;
    [SerializeField] float puckSpeed;

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
        GameObject noteObject = null;
        foreach (NoteData note in notes)
        {
            switch(note.LaneNo)
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
            noteObject.GetComponent<Rigidbody>().AddForce(new Vector3(puckSpeed, 0, 0));
        }
    }

}
