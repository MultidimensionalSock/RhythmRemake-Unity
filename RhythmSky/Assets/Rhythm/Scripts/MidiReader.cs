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
    TempoMap tempoMap;
    float BPM;
    int songsegments;
    public List<List<NoteData>> songData;
    Coroutine songCoroutine;
    bool loaded = false;

    
    NoteName[] lane1Notes = { NoteName.G, NoteName.E, NoteName.B };
    NoteName[] lane2Notes = { NoteName.C, NoteName.GSharp, NoteName.ASharp };
    NoteName[] lane3Notes = { NoteName.D, NoteName.F, NoteName.FSharp, };
    NoteName[] lane4Notes = { NoteName.A, NoteName.CSharp, NoteName.DSharp };

    public event System.Action<List<NoteData>> NoteCall;
    int BeatValue = 1;

    public void StartSong() 
    { 
        songCoroutine = StartCoroutine(PlaySong()); 
    }
    public void RestartSong() { BeatValue = 1; StartSong(); }
    public void PauseSong() { StopCoroutine(songCoroutine); }

    public void SetMidiLocation(string location)
    {
        midiFile = MidiFile.Read(location);
        tempoMap = midiFile.GetTempoMap();
        //songAudio = song;
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
            Debug.Log("there is more than one tempo in this song, this cannot currently be handled");
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
            if (songData[songBeat] == null) { songData[songBeat] = new List<NoteData>(); }
            songData[songBeat].Add(new NoteData(
                    calculateLaneNumber(note),
                    note.NoteName,
                    convertMetricTimeToSeconds(TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap)),
                    convertMetricTimeToSeconds(TimeConverter.ConvertTo<MetricTimeSpan>(note.EndTime, tempoMap)),
                    note.Length));
        }

        //remove notes that would happen on the same lane at the same time 
        foreach (List<NoteData> beatData in songData)
        {
            if (beatData != null)
            {
                beatData.DistinctBy(note => note.LaneNo);
            }
        }
    }


    IEnumerator PlaySong()
    {
        while (loaded == false)
        {
            yield return new WaitForFixedUpdate();
        }
        if (songData == null || songData.Count == 0)
        {
            Debug.LogWarning("Song data is null or empty, check if the midi file has been properly passed in");
            yield break;
        }
        while (BeatValue < songsegments)
        {
            yield return new WaitForSeconds(60f / BPM);
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
