using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;

// Class to handle questions, dispatch them to students and play them in order of arrival
public class StudentHandler : MonoBehaviourPunCallbacks
{
    private GameObject[] students; // array of all students in the room
    private Queue<SmartStudentController> questionsQueue; // queue to handle the order of the questions

    void Start()
    {
        questionsQueue = new Queue<SmartStudentController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayQuestion();
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // Wait for students to be instantiated
        StartCoroutine(GetStudents());
    }

    // Coroutine to wait for students to be instantiated
    private IEnumerator GetStudents()
    {
        yield return new WaitForSeconds(1);
        students = GameObject.FindGameObjectsWithTag("SmartStudent");
    }

    // Get a random student from the array
    private GameObject GetRandomStudent()
    {
        if (students.Length <= 0)
        {
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, students.Length);
        return students[randomIndex];
    }

    // Add a question to a random student, accessed by QuestionDispatcher once the audio is received
    [PunRPC]
    public void AddQuestion(byte[] audioBytes)
    {
        GameObject student = GetRandomStudent();

        if (student == null)
        {
            Debug.LogError("No students found!");
            return;
        }

        // Convert the byte array to a float array
        float[] audioDataResponse = new float[audioBytes.Length / 2];
        
        // Turn into correct format
        for (int i = 0; i < audioBytes.Length; i += 2)
        {
            short sample = BitConverter.ToInt16(audioBytes, i);
            audioDataResponse[i / 2] = sample / 32768.0f;
        }

        // Create a new AudioClip and set the audio data
        AudioClip audioClip = AudioClip.Create("ReceivedAudio", audioDataResponse.Length, 1, 24000, false);
        audioClip.SetData(audioDataResponse, 0);

        SmartStudentController studentController = student.GetComponent<SmartStudentController>();
        studentController.AddQuestion(audioClip);

        // Add the student to the queue if it's not already there
        // in case the student is already in the queue, it's order will be preserved with a new question
        if(!questionsQueue.Contains(studentController))
        {
            questionsQueue.Enqueue(studentController);
        }
    }

    // Play the question of the first student in the queue
    [PunRPC]
    private void PlayQuestion()
    {
        if (questionsQueue.Count <= 0)
        {
            return;
        }

        SmartStudentController studentController = questionsQueue.Dequeue();
        studentController.PlayQuestion();
    }


}
