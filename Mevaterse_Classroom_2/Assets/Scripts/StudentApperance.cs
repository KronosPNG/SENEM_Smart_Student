using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Class to handle the appearance of the smart students
public class StudentApperance : MonoBehaviourPun
{    
    private List<SkinnedMeshRenderer> haircuts,
                                    uniforms,
                                    eyes,
                                    brows,
                                    beards,
                                    glasses;

    private SkinnedMeshRenderer selectedHaircut,
                                selectedUniform,
                                selectedEyes,
                                selectedBrows,
                                selectedBeard,
                                selectedGlasses;
    
    public int selectedHaircutIndex,
                selectedUniformIndex,
                selectedEyesIndex,
                selectedBrowsIndex,
                selectedBeardIndex,
                selectedGlassesIndex;
    
    public Color32 uniformColor,
                    hairColor,
                    eyeColor,
                    skinColor,
                    tieColor,
                    lipsColor,
                    glassesColor;
                            
    private ColorData colorData;

    void Start()
    {
        colorData = GameObject.Find("ColorData").GetComponent<ColorData>();;

        haircuts = new List<SkinnedMeshRenderer>();
        uniforms = new List<SkinnedMeshRenderer>();
        eyes = new List<SkinnedMeshRenderer>();
        brows = new List<SkinnedMeshRenderer>();
        beards = new List<SkinnedMeshRenderer>();
        glasses = new List<SkinnedMeshRenderer>();

        LoadMeshes(GetComponentsInChildren<Transform>());

        if(photonView.IsMine)
        {
            selectedHaircutIndex = Random.Range(0, haircuts.Count);
            selectedUniformIndex = Random.Range(0, uniforms.Count);
            selectedEyesIndex = Random.Range(0, eyes.Count);
            selectedBrowsIndex = Random.Range(0, brows.Count);
            selectedBeardIndex = Random.Range(0, beards.Count);
            selectedGlassesIndex = Random.Range(0, glasses.Count); 
        }

        selectedHaircut = SelectMesh(haircuts, selectedHaircutIndex);
        selectedUniform = SelectMesh(uniforms, selectedUniformIndex);
        selectedEyes = SelectMesh(eyes, selectedEyesIndex);
        selectedBrows = SelectMesh(brows, selectedBrowsIndex);
        selectedBeard = SelectMesh(beards, selectedBeardIndex);
        selectedGlasses = SelectMesh(glasses, selectedGlassesIndex);

        if(photonView.IsMine)
        {
            LoadColors();
            SetColors();
        }

    }

    private SkinnedMeshRenderer SelectMesh(List<SkinnedMeshRenderer> list, int selected)
    {

        SkinnedMeshRenderer selectedMesh = null;

        for (int i = 0; i < list.Count; i++)
        {
            if (i == selected)
            {
                list[i].enabled = true;
                selectedMesh = list[i];
            }
            else
            {
                list[i].enabled = false;
            }
        }

        return selectedMesh;
    }
    
    private SkinnedMeshRenderer SelectRandomMesh(List<SkinnedMeshRenderer> list)
    {
        int random = Random.Range(0, list.Count);
        return SelectMesh(list, random);
    }


    private void SetColors(){
        foreach (Material m in selectedUniform.materials)
        {
            if (m.name.Equals("Trousers (Instance)"))
                m.color = uniformColor;

            else if (m.name.Equals("Eyecolor (Instance)"))
                m.color = eyeColor;

            else if (m.name.Equals("Skin (Instance)"))
                m.color = skinColor;

            else if (m.name.Equals("Tie (Instance)"))
                m.color = tieColor;

            else if (m.name.Equals("Lipstick (Instance)"))
                m.color = lipsColor;
        }

        selectedHaircut.material.color = hairColor;
        selectedBrows.material.color = hairColor;
        selectedBeard.material.color = hairColor;

        foreach (Material m in selectedGlasses.materials)
        {
            if (m.name.Equals("Glasses (Instance)"))
                m.color = glassesColor;
        }
    }

    private void LoadColors(){
        foreach (Material m in selectedUniform.materials)
        {
            if (m.name.Equals("Trousers (Instance)"))
            {
                uniformColor = colorData.GetUniformColors()[UnityEngine.Random.Range(0, colorData.GetUniformColors().Count)];
            }

            else if (m.name.Equals("Eyecolor (Instance)"))
            {
                eyeColor = colorData.GetEyeColors()[UnityEngine.Random.Range(0, colorData.GetEyeColors().Count)];
            }              

            else if (m.name.Equals("Skin (Instance)"))
            {
                skinColor = colorData.GetSkinTones()[UnityEngine.Random.Range(0, colorData.GetSkinTones().Count)];
            }
                
            else if (m.name.Equals("Tie (Instance)"))
            {
                tieColor = colorData.GetTieColors()[UnityEngine.Random.Range(0, colorData.GetTieColors().Count)];
            }
                

            else if (m.name.Equals("Lipstick (Instance)"))
            {
                lipsColor = colorData.GetLipsColors()[UnityEngine.Random.Range(0, colorData.GetLipsColors().Count)];
            }
        }

        hairColor = colorData.GetHairColors()[UnityEngine.Random.Range(0, colorData.GetHairColors().Count)];

        foreach (Material m in selectedGlasses.materials)
        {
            if (m.name.Equals("Glasses (Instance)"))
                glassesColor = colorData.GetGlassesColors()[UnityEngine.Random.Range(0, colorData.GetGlassesColors().Count)];
        }
    }

    private void LoadMeshes(Transform[] children)
    {
        foreach (Transform child in children)
        {
            if (child.CompareTag("Haircut"))
                haircuts.Add(child.GetComponent<SkinnedMeshRenderer>());

            else if (child.CompareTag("Beard"))
                beards.Add(child.GetComponent<SkinnedMeshRenderer>());

            else if (child.CompareTag("Eyes"))
                eyes.Add(child.GetComponent<SkinnedMeshRenderer>());

            else if (child.CompareTag("Brows"))
                brows.Add(child.GetComponent<SkinnedMeshRenderer>());

            else if (child.CompareTag("Glasses"))
                glasses.Add(child.GetComponent<SkinnedMeshRenderer>());

            else if (child.CompareTag("Uniform"))
                uniforms.Add(child.GetComponent<SkinnedMeshRenderer>());
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if (stream.IsWriting)
        // {
        //     // Send data to other clients
        //     stream.SendNext(selectedHaircutIndex);
        //     stream.SendNext(selectedUniformIndex);
        //     stream.SendNext(selectedEyesIndex);
        //     stream.SendNext(selectedBrowsIndex);
        //     stream.SendNext(selectedBeardIndex);
        //     stream.SendNext(selectedGlassesIndex);

        //     stream.SendNext((Color32)uniformColor);
        //     stream.SendNext((Color32)hairColor);
        //     stream.SendNext((Color32)eyeColor);
        //     stream.SendNext((Color32)skinColor);
        //     stream.SendNext((Color32)tieColor);
        //     stream.SendNext((Color32)lipsColor);
        //     stream.SendNext((Color32)glassesColor);
        // }
        // else
        // {
        //     // Receive data from other clients
        //     selectedHaircutIndex = (int)stream.ReceiveNext();
        //     selectedUniformIndex = (int)stream.ReceiveNext();
        //     selectedEyesIndex = (int)stream.ReceiveNext();
        //     selectedBrowsIndex = (int)stream.ReceiveNext();
        //     selectedBeardIndex = (int)stream.ReceiveNext();
        //     selectedGlassesIndex = (int)stream.ReceiveNext();

        //     uniformColor = (Color32)stream.ReceiveNext();
        //     hairColor = (Color32)stream.ReceiveNext();
        //     eyeColor = (Color32)stream.ReceiveNext();
        //     skinColor = (Color32)stream.ReceiveNext();
        //     tieColor = (Color32)stream.ReceiveNext();
        //     lipsColor = (Color32)stream.ReceiveNext();
        //     glassesColor = (Color32)stream.ReceiveNext();
        // }

        // SetColors();
    }

}
