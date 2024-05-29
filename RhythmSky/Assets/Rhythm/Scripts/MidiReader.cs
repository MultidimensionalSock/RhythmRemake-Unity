using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;

//need a way to decide how many notes you want to be able to work at one time 

public struct NoteData
{
    public NoteData(int lane, NoteName name, float start, float end, float len)
    {
        LaneNo = lane;
        noteName = name;
        startTime = start;
        endTime = end;
        length = len;
    }
    //which lane the note should go into
    public int LaneNo;
    //name of the note
    public NoteName noteName;
    //what time in metric the note should start
    public float startTime;
    //what time in metric the note should end
    public float endTime;
    public float length;


}


public class MidiReader : MonoBehaviour
{
    MidiFile midiFile;
    AudioClip songAudio;
    TempoMap tempoMap;
    float BPM;
    int songsegments;
    public List<List<NoteData>> songData;
    Coroutine songCoroutine;
    bool loaded = false;
    
    NoteName[] lane1Notes = { NoteName.A, NoteName.ASharp, NoteName.B };
    NoteName[] lane2Notes = { NoteName.C, NoteName.CSharp, NoteName.D };
    NoteName[] lane3Notes = { NoteName.DSharp, NoteName.E, NoteName.F };
    NoteName[] lane4Notes = { NoteName.FSharp, NoteName.G, NoteName.GSharp };

    public event System.Action<List<NoteData>> NoteCall;
    int BeatValue = 1;

    public void StartSong() 
    { 
        songCoroutine = StartCoroutine(PlaySong()); 
    }
    public void RestartSong() { BeatValue = 1; StartSong(); }
    public void PauseSong() { StopCoroutine(songCoroutine); }


    void Start()
    {
        BeatValue = 0;
        songData = new();
        //SetMidiLocation("Assets/Midi/Vocaloid - 1925.mid", Resources.Load<AudioClip>("Assets/Midi/ytmp3free.cc_hatsune-miku-1925-english-subbed-youtubemp3free.org.mp3"));
    }

    public void SetMidiLocation(string location, AudioClip song)
    {
        midiFile = MidiFile.Read(location);
        tempoMap = midiFile.GetTempoMap();
        songAudio = song;
        ReadMidiFile();
    }

    void ReadMidiFile()
    {
        if (songData != null) songData.Clear();
        var tempoChanges = tempoMap.GetTempoChanges();
        int count = 0;
        foreach (var tempo in tempoChanges)
        {
            BPM = 60000000.0f / tempo.Value.MicrosecondsPerQuarterNote;
            count++;
        }
        if (count > 1)
        {
            Debug.Log("there is more than one tempo in this song");
            return;
        }
        else
        {
            var timedEvents = midiFile.GetTimedEvents();
            float songLength = (float)TimeConverter.ConvertTo<MetricTimeSpan>(timedEvents.Last().Time, tempoMap).TotalMinutes;
            songsegments = (int)(songLength * BPM);

            Debug.Log(BPM); //working correctly 
            
            GetNotes();
            
            loaded = true;
        }
    }

    void GetNotes()
    {
        
        songData = new List<NoteData>[songsegments].ToList();
        var notes = midiFile.GetNotes();
        foreach (var note in notes)
        {
            double NoteStartInSeconds = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds;
            int songBeat = (int)(NoteStartInSeconds / (60.0f / BPM));
            //i dont know if this will work
            //this is shwoing nullptr
            if (songData[songBeat] == null) { songData[songBeat] = new List<NoteData>(); }
            songData[songBeat].Add(new NoteData(
                    calculateLaneNumber(note),
                    note.NoteName,
                    convertMetricTimeToSeconds(TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap)),
                    convertMetricTimeToSeconds(TimeConverter.ConvertTo<MetricTimeSpan>(note.EndTime, tempoMap)),
                    note.Length));
        }

        Debug.Log(songData.Count);


        //go through the note array
        //find the time it happens at in seconds
        //assign each note to a bpm based on its time stamp 
        //go throgh and reduce the notes
    }


    IEnumerator PlaySong()
    {
        while (loaded == false)
        {
            Debug.Log("song data not loaded");
            yield return new WaitForFixedUpdate();
        }
        if (songData == null || songData.Count == 0)
        {
            Debug.LogWarning("songdata is null");
            yield break;
        }
        Debug.Log("the beat count is: " + BeatValue + ", and the songdata count is now: " + songData.Count);
        while (BeatValue < songsegments)
        {
            yield return new WaitForSeconds(60f / BPM);
            Debug.Log("after the wait, the beat count is: " + BeatValue + ", and the songdata count is now: " + songData.Count);
            if (songData[BeatValue] != null)
            {
                NoteCall?.Invoke(songData[BeatValue]);
            }
            BeatValue++;
            //event to say song has ended needs to go here 
        }
    }
    int calculateLaneNumber(Melanchall.DryWetMidi.Interaction.Note note)
    {
        if (lane1Notes.Contains(note.NoteName)) return 1;
        if (lane2Notes.Contains(note.NoteName)) return 2;
        if (lane3Notes.Contains(note.NoteName)) return 3;
        return 4;
    }

    float convertMetricTimeToSeconds(MetricTimeSpan timeToWait)
    {
        return timeToWait.Minutes * 60f + timeToWait.Seconds + (float)timeToWait.Milliseconds / 1000f;
    }
}
